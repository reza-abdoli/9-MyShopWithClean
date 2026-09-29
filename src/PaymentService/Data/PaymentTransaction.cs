// PaymentService/Data/PaymentTransaction.cs
namespace PaymentService.Data;

public class PaymentTransaction
{
    public Guid PaymentId { get; set; }
    public long OrderId { get; set; }
    public long UserId { get; set; }
    public int Amount { get; set; }
    public string Status { get; set; } = "Registered";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}