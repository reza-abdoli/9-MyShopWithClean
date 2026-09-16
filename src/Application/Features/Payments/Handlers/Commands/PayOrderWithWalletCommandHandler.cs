// Application/Features/Payments/Handlers/Commands/PayOrderWithWalletCommandHandler.cs
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

public class PayOrderWithWalletCommandHandler
{
    private readonly IMessageBus _bus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PayOrderWithWalletCommandHandler> _logger;

    public PayOrderWithWalletCommandHandler(IMessageBus bus, IUnitOfWork unitOfWork, ILogger<PayOrderWithWalletCommandHandler> logger)
    {
        _bus = bus;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(PayOrderWithWalletCommand command)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {

            // ساخت رکورد تراکنش Pending/Success مستقیم برای audit log
            var pendingTx = await _bus.InvokeAsync<PaymentTransaction>(new CreatePendingTransactionCommand
            {
                UserId = command.UserId,
                OrderId = command.OrderId,
                Amount = command.TotalCartPrice,
                Authority = $"WALLET_{Guid.NewGuid():N}"
            });


            await _bus.InvokeAsync(new WithdrawBalanceCommand
            {
                UserId = command.UserId,
                Amount = command.TotalCartPrice
            });


            await _bus.InvokeAsync(new UpdateTransactionStatusCommand
            {
                TransactionId = pendingTx.Id,
                Status = PaymentStatus.Success
            });

            await _bus.InvokeAsync(new UpdatePaidOrderCommand
            {
                OrderId = command.OrderId,
                UserId = command.UserId,
                PaymentTransactionId = pendingTx.Id
            });

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Wallet payment processed successfully for UserId: {UserId}, OrderId: {OrderId}",
                command.UserId, command.OrderId);
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to process wallet payment for UserId: {UserId}, OrderId: {OrderId}. Error: {Message}",
                command.UserId, command.OrderId, ex.Message);
            return false;
        }
    }
}