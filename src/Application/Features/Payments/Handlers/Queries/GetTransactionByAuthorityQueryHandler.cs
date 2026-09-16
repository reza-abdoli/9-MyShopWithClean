// Application/Features/Payments/Handlers/Queries/GetTransactionByAuthorityQueryHandler.cs (اصلاح‌شده)
using System.Threading.Tasks;
using Application.Features.Payments.Requests.Queries;
using Application.Features.Payments.Responses;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Handlers.Queries;

public class GetTransactionByAuthorityQueryHandler
{
    private readonly IRepository<PaymentTransaction> _paymentTransactionRepository;

    public GetTransactionByAuthorityQueryHandler(IRepository<PaymentTransaction> paymentTransactionRepository)
    {
        _paymentTransactionRepository = paymentTransactionRepository;
    }

    public async Task<PaymentTransactionDto?> Handle(GetTransactionByAuthorityQuery query)
    {
        var tx = await _paymentTransactionRepository.GetQuery().SingleOrDefaultAsync(t => t.Authority == query.Authority);
        if (tx == null) return null;

        return new PaymentTransactionDto
        {
            Id = tx.Id,
            UserId = tx.UserId,
            OrderId = tx.OrderId,
            Amount = tx.Amount,
            Authority = tx.Authority,
            Status = tx.Status
        };
    }
}