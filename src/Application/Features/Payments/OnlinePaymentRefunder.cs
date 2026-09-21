// Application/Features/Payments/OnlinePaymentRefunder.cs
using System;
using System.Threading.Tasks;
using KafkaContracts.Notifications;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments;

public class OnlinePaymentRefunder : IOnlinePaymentRefunder
{
    private readonly IRepository<PaymentTransaction> _paymentRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IOutboxUnitOfWork _outbox;
    private readonly ILogger<OnlinePaymentRefunder> _logger;

    public OnlinePaymentRefunder(
        IRepository<PaymentTransaction> paymentRepository,
        IRepository<User> userRepository,
        IOutboxUnitOfWork outbox,
        ILogger<OnlinePaymentRefunder> logger)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _outbox = outbox;
        _logger = logger;
    }

    /// true = پول برگشت خورد. false = تراکنش دیگه Pending نیست (قبلاً پردازش شده).
    /// اگه خطای دیتابیس بده، exception پرتاب می‌شه تا صدا زننده دوباره تلاش کنه.
    public async Task<bool> RefundToWalletAsync(long paymentTransactionId)
    {
        var paymentTx = await _paymentRepository.GetEntityById(paymentTransactionId);
        if (paymentTx == null || paymentTx.Status != PaymentStatus.Pending)
            return false;

        var user = await _userRepository.GetEntityById(paymentTx.UserId)
            ?? throw new InvalidOperationException($"User {paymentTx.UserId} was not found.");

        user.Balance += paymentTx.Amount;
        _userRepository.UpdateEntity(user);

        paymentTx.Status = PaymentStatus.Success; // پول از بانک دریافت شده و به کیف‌پول رفته
        paymentTx.UpdatedAt = DateTime.UtcNow;
        _paymentRepository.UpdateEntity(paymentTx);

        if (!string.IsNullOrWhiteSpace(user.MobileNumber)) // اسم واقعی پراپرتی موبایل
        {
            await _outbox.PublishAsync(new SendSmsCommand
            {
                PhoneNumber = user.MobileNumber,
                Message = "مشکلی در پردازش سفارش شما رخ داد؛ مبلغ پرداختی به کیف پول شما برگردانده شد."
            });
        }

        await _outbox.CommitAsync();

        _logger.LogInformation("Refunded to wallet. UserId: {UserId}, Amount: {Amount}",
            paymentTx.UserId, paymentTx.Amount);
        return true;
    }
}