// Domain/Entities/Order/WalletPaymentAttempt.cs
namespace Domain.Entities.Order;

public class WalletPaymentAttempt : BaseEntity
{
    public Guid PaymentId { get; set; }                  // همون Id ساگا
    public long UserId { get; set; }
    public long OrderId { get; set; }
    public string Status { get; set; } = "Processing";   // Processing | Succeeded | Failed
    public string? Message { get; set; }
}