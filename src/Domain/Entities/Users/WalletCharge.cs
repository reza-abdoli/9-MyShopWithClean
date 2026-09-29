// Domain/Entities/Users/WalletCharge.cs
namespace Domain.Entities.Users;

public class WalletCharge : BaseEntity
{
    public long PaymentId { get; set; }   // یکتا؛ کلید Idempotency
    public long UserId { get; set; }
    public int Amount { get; set; }
}