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

    public async Task<PaymentTransaction?> Handle(GetTransactionByAuthorityQuery query)
    {
        return await _paymentTransactionRepository.GetQuery().SingleOrDefaultAsync(t => t.Authority == query.Authority);
    }
}