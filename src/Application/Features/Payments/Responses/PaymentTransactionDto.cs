// Application/Features/Payments/Responses/PaymentTransactionDto.cs
using Domain.Entities.Order;

namespace Application.Features.Payments.Responses;

public class PaymentTransactionDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? OrderId { get; set; }
    public int Amount { get; set; }
    public string Authority { get; set; }
    public PaymentStatus Status { get; set; }
}