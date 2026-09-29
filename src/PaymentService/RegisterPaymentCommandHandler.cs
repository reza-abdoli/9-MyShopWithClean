using KafkaContracts.Payments;
using Microsoft.EntityFrameworkCore;
using Wolverine;                    // ← برای OutgoingMessages
using Wolverine.EntityFrameworkCore;
using PaymentService.Data;          // ← برای PaymentDbContext و PaymentTransaction

namespace PaymentService;

public class RegisterPaymentCommandHandler
{
    private const int MaxAmount = 1_00_000_000;

    public async Task<OutgoingMessages> Handle(RegisterPaymentCommand cmd, PaymentDbContext db)
    {
        var existing = await db.PaymentTransactions.FindAsync(cmd.Id);
        if (existing != null)
            return new() { new PaymentRegisteredEvent(cmd.Id) };

        if (cmd.Amount > MaxAmount)
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                PaymentId = cmd.Id,
                OrderId = cmd.OrderId,
                UserId = cmd.UserId,
                Amount = cmd.Amount,
                Status = "Rejected",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
            return new() { new PaymentRegistrationFailedEvent(cmd.Id, "مبلغ بیش از حد مجاز است") };
        }

        db.PaymentTransactions.Add(new PaymentTransaction
        {
            PaymentId = cmd.Id,
            OrderId = cmd.OrderId,
            UserId = cmd.UserId,
            Amount = cmd.Amount,
            Status = "Registered",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
        return new() { new PaymentRegisteredEvent(cmd.Id) };
    }
}