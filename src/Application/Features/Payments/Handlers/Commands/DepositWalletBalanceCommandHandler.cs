// Application/Features/Payments/Handlers/Commands/DepositWalletBalanceCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Features.Notifications.Requests.Commands;
using Application.Features.Payments.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Handlers.Commands;

public class DepositWalletBalanceCommandHandler
{
    private readonly IRepository<PaymentTransaction> _paymentRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IOutboxUnitOfWork _outbox;
    private readonly ILogger<DepositWalletBalanceCommandHandler> _logger;

    public DepositWalletBalanceCommandHandler(
        IRepository<PaymentTransaction> paymentRepository,
        IRepository<User> userRepository,
        IOutboxUnitOfWork outbox,
        ILogger<DepositWalletBalanceCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task<bool> Handle(DepositWalletBalanceCommand command)
    {
        try
        {
            var paymentTx = await _paymentRepository.GetEntityById(command.PaymentTransaction.Id);

            // فقط تراکنش شارژ (بدون سفارش) که هنوز Pending است
            if (paymentTx == null || paymentTx.Status != PaymentStatus.Pending || paymentTx.OrderId != null)
            {
                _logger.LogWarning("Deposit ignored: transaction missing, not pending, or tied to an order.");
                return false;
            }

            var user = await _userRepository.GetEntityById(paymentTx.UserId);
            if (user == null)
                return false;

            user.Balance += paymentTx.Amount;
            _userRepository.UpdateEntity(user);

            paymentTx.Status = PaymentStatus.Success;
            paymentTx.UpdatedAt = DateTime.UtcNow;
            _paymentRepository.UpdateEntity(paymentTx);

            if (!string.IsNullOrWhiteSpace(user.MobileNumber)) // اسم واقعی پراپرتی موبایل
            {
                await _outbox.PublishAsync(new SendSmsCommand
                {
                    PhoneNumber = user.MobileNumber,
                    Message = $"کیف پول شما به مبلغ {paymentTx.Amount:N0} تومان شارژ شد."
                });
            }

            await _outbox.CommitAsync();

            _logger.LogInformation("Wallet deposited. UserId: {UserId}, Amount: {Amount}",
                paymentTx.UserId, paymentTx.Amount);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deposit wallet balance. TransactionId: {Id}",
                command.PaymentTransaction.Id);
            return false;
        }
    }
}