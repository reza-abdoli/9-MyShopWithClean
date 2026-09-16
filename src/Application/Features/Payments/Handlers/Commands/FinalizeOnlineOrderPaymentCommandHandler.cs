// Application/Features/Payments/Handlers/Commands/FinalizeOnlineOrderPaymentCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.DTOs.Account;
using Application.Features.Orders.Requests.Commands;
using Application.Features.Payments.Requests.Commands;
using Application.Features.Users.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Application.Features.Payments.Handlers.Commands;

public class FinalizeOnlineOrderPaymentCommandHandler
{
    private readonly IMessageBus _bus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FinalizeOnlineOrderPaymentCommandHandler> _logger;

    public FinalizeOnlineOrderPaymentCommandHandler(IMessageBus bus, IUnitOfWork unitOfWork, ILogger<FinalizeOnlineOrderPaymentCommandHandler> logger)
    {
        _bus = bus;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(FinalizeOnlineOrderPaymentCommand command)
    {
        var paymentTx = command.PaymentTransaction;
        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            await _bus.InvokeAsync(new UpdateTransactionStatusCommand
            {
                TransactionId = paymentTx.Id,
                Status = PaymentStatus.Success
            });

            await _bus.InvokeAsync(new UpdatePaidOrderCommand
            {
                OrderId = paymentTx.OrderId!.Value,
                UserId = paymentTx.UserId,
                PaymentTransactionId = paymentTx.Id
            });

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Online payment processed successfully for UserId: {UserId}, OrderId: {OrderId}",
                paymentTx.UserId, paymentTx.OrderId);
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            _logger.LogWarning(ex,
                "Online payment order finalization failed for OrderId: {OrderId}, UserId: {UserId}. Refunding amount to user wallet.",
                paymentTx.OrderId, paymentTx.UserId);

            // Fallback: در صورت شکست پردازش سفارش بعد از پرداخت بانکی، مبلغ به کیف پول کاربر برگردونده می‌شه
            try
            {
                await using var refundTransaction = await _unitOfWork.BeginTransactionAsync();

                await _bus.InvokeAsync(new UpdateTransactionStatusCommand
                {
                    TransactionId = paymentTx.Id,
                    Status = PaymentStatus.Success
                });

                await _bus.InvokeAsync(new DepositBalanceCommand
                {
                    DepositBalanceDto = new DepositBalanceDto
                    {
                        Id = paymentTx.UserId,
                        Amount = paymentTx.Amount
                    }
                });

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "Successfully refunded paid order amount to wallet for UserId: {UserId}, Amount: {Amount}",
                    paymentTx.UserId, paymentTx.Amount);
            }
            catch (Exception refundEx)
            {
                _logger.LogCritical(refundEx,
                    "CRITICAL: Failed to refund paid amount to user wallet after order processing failure. UserId: {UserId}, OrderId: {OrderId}",
                    paymentTx.UserId, paymentTx.OrderId);
            }

            return false;
        }
    }
}