// Application/Features/Payments/Handlers/Commands/FinalizeOnlineOrderPaymentCommandHandler.cs
using System;
using System.Threading.Tasks;
using KafkaContracts.Notifications;
using Application.Features.Orders;
using Application.Features.Payments.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Handlers.Commands;

public class FinalizeOnlineOrderPaymentCommandHandler
{
    private const int MaxRefundAttempts = 3;

    private readonly IRepository<PaymentTransaction> _paymentRepository;
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IOrderHelper _orderHelper;
    private readonly IOutboxUnitOfWork _outbox;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FinalizeOnlineOrderPaymentCommandHandler> _logger;

    public FinalizeOnlineOrderPaymentCommandHandler(
        IRepository<PaymentTransaction> paymentRepository,
        IRepository<Domain.Entities.Order.Order> orderRepository,
        IRepository<User> userRepository,
        IOrderHelper orderHelper,
        IOutboxUnitOfWork outbox,
        IServiceScopeFactory scopeFactory,
        ILogger<FinalizeOnlineOrderPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _orderHelper = orderHelper;
        _outbox = outbox;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<bool> Handle(FinalizeOnlineOrderPaymentCommand command)
    {
        var paymentTxId = command.PaymentTransaction.Id;

        try
        {
            // ---- تلاش اول: نهایی‌سازی سفارش با یک commit ----
            var paymentTx = await _paymentRepository.GetEntityById(paymentTxId);

            // جلوگیری از پردازش دوباره‌ی callback تکراری
            if (paymentTx == null || paymentTx.Status != PaymentStatus.Pending || paymentTx.OrderId == null)
            {
                _logger.LogWarning("Online payment ignored: transaction {Id} missing or not pending.", paymentTxId);
                return false;
            }

            var order = await _orderHelper.GetOrderWithDetailsOrThrow(paymentTx.OrderId.Value, paymentTx.UserId);
            _orderHelper.EnsurePaymentMatchesOrder(paymentTx, order);

            var now = DateTime.UtcNow;
            paymentTx.Status = PaymentStatus.Success;
            paymentTx.UpdatedAt = now;
            _paymentRepository.UpdateEntity(paymentTx);

            await _orderHelper.ApplyPaymentAsync(order, paymentTx, now);
            _orderRepository.UpdateEntity(order);

            var user = await _userRepository.GetEntityById(paymentTx.UserId);
            if (!string.IsNullOrWhiteSpace(user?.MobileNumber)) 
            {
                await _outbox.PublishAsync(new SendSmsCommand
                {
                    PhoneNumber = user.MobileNumber,
                    Message = $"سفارش شما با موفقیت پرداخت شد. مبلغ: {paymentTx.Amount:N0} تومان"
                });
            }

            await _outbox.CommitAsync();

            _logger.LogInformation("Online payment finalized. UserId: {UserId}, OrderId: {OrderId}",
                paymentTx.UserId, paymentTx.OrderId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Online payment finalization failed. TransactionId: {Id}. Refunding to wallet.", paymentTxId);
        }

        // ---- تلاش دوم: برگشت پول با scope و DbContext تازه ----
        // (DbContext فعلی پر از تغییرات نیمه‌کاره‌ست و نباید دوباره save بشه)
        for (var attempt = 1; attempt <= MaxRefundAttempts; attempt++)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var refunder = scope.ServiceProvider.GetRequiredService<IOnlinePaymentRefunder>();
                await refunder.RefundToWalletAsync(paymentTxId);
                return false; // برگشت پول انجام شد (یا تراکنش دیگه Pending نبود)، ولی سفارش نهایی نشد
            }
            catch (Exception refundEx)
            {
                _logger.LogWarning(refundEx, "Refund attempt {Attempt} failed. TransactionId: {Id}",
                    attempt, paymentTxId);
            }
        }

        _logger.LogCritical(
            "CRITICAL: Refund failed after {Max} attempts. Manual action needed. TransactionId: {Id}",
            MaxRefundAttempts, paymentTxId);
        return false;
    }
}