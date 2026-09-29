// Application/Features/Payments/Handlers/Queries/GetWalletPaymentStatusQueryHandler.cs
using Application.Features.Payments.Requests.Queries;
using Application.Features.Payments.Responses;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Handlers.Queries;

public class GetWalletPaymentStatusQueryHandler
{
    private readonly IRepository<WalletPaymentAttempt> _attemptRepository;

    public GetWalletPaymentStatusQueryHandler(IRepository<WalletPaymentAttempt> attemptRepository)
        => _attemptRepository = attemptRepository;

    public async Task<WalletPaymentStatusDto> Handle(GetWalletPaymentStatusQuery query)
    {
        // AsNoTracking لازمه: این query تکراری صدا زده می‌شه و نباید نتیجه‌ی قدیمی از حافظه بیاد
        var result = await _attemptRepository.GetQuery()
            .AsNoTracking()
            .Where(a => a.PaymentId == query.PaymentId && a.UserId == query.UserId)  // هر کاربر فقط پرداخت خودش
            .Select(a => new WalletPaymentStatusDto { Status = a.Status, Message = a.Message })
            .FirstOrDefaultAsync();

        return result ?? new WalletPaymentStatusDto { Status = "Processing" };
    }
}