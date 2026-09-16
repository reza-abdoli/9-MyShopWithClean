// Application/Features/Payments/Handlers/Commands/DepositWalletBalanceCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.DTOs.Account;
using Application.Features.Payments.Requests.Commands;
using Application.Features.Users.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Application.Features.Payments.Handlers.Commands;

public class DepositWalletBalanceCommandHandler
{
    private readonly IMessageBus _bus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DepositWalletBalanceCommandHandler> _logger;

    public DepositWalletBalanceCommandHandler(IMessageBus bus, IUnitOfWork unitOfWork, ILogger<DepositWalletBalanceCommandHandler> logger)
    {
        _bus = bus;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(DepositWalletBalanceCommand command)
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

            await _bus.InvokeAsync(new DepositBalanceCommand
            {
                DepositBalanceDto = new DepositBalanceDto
                {
                    Id = paymentTx.UserId,
                    Amount = paymentTx.Amount
                }
            });

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Wallet balance deposited successfully for UserId: {UserId}, Amount: {Amount}",
                paymentTx.UserId, paymentTx.Amount);

            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            _logger.LogError(ex, "Failed to deposit wallet balance for UserId: {UserId}, Amount: {Amount}. Error: {Message}",
                paymentTx.UserId, paymentTx.Amount, ex.Message);

            return false;
        }
    }
}