// PaymentService/CreateOnlinePendingPaymentCommandHandler.cs
using KafkaContracts.Payments;
using PaymentService.Data;

namespace PaymentService;

public class CreateOnlinePendingPaymentCommandHandler
{
    public async Task Handle(CreateOnlinePendingPaymentCommand cmd, PaymentDbContext db)
    {
        var existing = await db.PaymentTransactions.FindAsync(cmd.Id);
        if (existing != null) return;   // Idempotency

        db.PaymentTransactions.Add(new PaymentTransaction
        {
            PaymentId = cmd.Id,
            OrderId = cmd.OrderId,
            UserId = cmd.UserId,
            Amount = cmd.Amount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync(); 
    }
}