// Application/Features/Payments/Requests/Queries/GetWalletPaymentStatusQuery.cs
namespace Application.Features.Payments.Requests.Queries;

public class GetWalletPaymentStatusQuery
{
    public Guid PaymentId { get; set; }
    public long UserId { get; set; }
}