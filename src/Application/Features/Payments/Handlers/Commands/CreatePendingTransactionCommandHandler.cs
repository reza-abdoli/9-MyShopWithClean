// Application/Features/Payments/Handlers/Commands/CreatePendingTransactionCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Features.Payments.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Handlers.Commands;

public class CreatePendingTransactionCommandHandler
{
    private readonly IRepository<PaymentTransaction> _paymentTransactionRepository;
    private readonly ILogger<CreatePendingTransactionCommandHandler> _logger;

    public CreatePendingTransactionCommandHandler(
        IRepository<PaymentTransaction> paymentTransactionRepository,
        ILogger<CreatePendingTransactionCommandHandler> logger)
    {
        _paymentTransactionRepository = paymentTransactionRepository;
        _logger = logger;
    }

    public async Task<PaymentTransaction> Handle(CreatePendingTransactionCommand command)
    {
        // فقط برای پرداخت سفارش، رکورد قبلی همون سفارش دوباره استفاده می‌شه (رابطه‌ی یک‌به‌یک)
        PaymentTransaction? transaction = null;
        if (command.OrderId != null)
        {
            transaction = await _paymentTransactionRepository.GetQuery().FirstOrDefaultAsync(
                x => x.UserId == command.UserId && x.OrderId == command.OrderId);
        }

        if (transaction != null)
        {
            transaction.Status = PaymentStatus.Pending;
            transaction.Amount = command.Amount;
            transaction.UpdatedAt = DateTime.UtcNow;
            transaction.Authority = command.Authority;

            _paymentTransactionRepository.UpdateEntity(transaction);
            await _paymentTransactionRepository.SaveChanges();

            _logger.LogInformation(
                "Existing transaction set to Pending. Id: {Id}, UserId: {UserId}, OrderId: {OrderId}",
                transaction.Id, command.UserId, command.OrderId);

            return transaction;
        }

        transaction = new PaymentTransaction
        {
            UserId = command.UserId,
            OrderId = command.OrderId,
            Amount = command.Amount,
            Authority = command.Authority,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _paymentTransactionRepository.AddEntity(transaction);
        await _paymentTransactionRepository.SaveChanges();

        _logger.LogInformation(
            "Pending transaction created. Id: {Id}, UserId: {UserId}, OrderId: {OrderId}, Amount: {Amount}",
            transaction.Id, command.UserId, command.OrderId, command.Amount);

        return transaction;
    }
}