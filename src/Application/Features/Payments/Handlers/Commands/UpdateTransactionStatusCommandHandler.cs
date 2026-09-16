// Application/Features/Payments/Handlers/Commands/UpdateTransactionStatusCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Features.Payments.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Handlers.Commands;

public class UpdateTransactionStatusCommandHandler
{
    private readonly IRepository<PaymentTransaction> _paymentTransactionRepository;
    private readonly ILogger<UpdateTransactionStatusCommandHandler> _logger;

    public UpdateTransactionStatusCommandHandler(IRepository<PaymentTransaction> paymentTransactionRepository, ILogger<UpdateTransactionStatusCommandHandler> logger)
    {
        _paymentTransactionRepository = paymentTransactionRepository;
        _logger = logger;
    }

    public async Task Handle(UpdateTransactionStatusCommand command)
    {
        var transaction = await _paymentTransactionRepository.GetEntityById(command.TransactionId);
        if (transaction != null)
        {
            transaction.Status = command.Status;
            transaction.UpdatedAt = DateTime.UtcNow;

            _paymentTransactionRepository.UpdateEntity(transaction);
            await _paymentTransactionRepository.SaveChanges();

            _logger.LogInformation(
                "Transaction status updated. Id: {Id}, NewStatus: {Status}",
                transaction.Id, command.Status);
        }
    }
}