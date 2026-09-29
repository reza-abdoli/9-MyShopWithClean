using KafkaContracts.Payments;
using Microsoft.EntityFrameworkCore;
using Wolverine;                    // ← برای OutgoingMessages
using Wolverine.EntityFrameworkCore;
using PaymentService.Data;          // ← برای PaymentDbContext و PaymentTransaction

namespace PaymentService;

public class VoidPaymentCommandHandler
{
    public async Task<OutgoingMessages> Handle(VoidPaymentCommand cmd, PaymentDbContext db)
    {
        var transaction = await db.PaymentTransactions.FindAsync(cmd.Id);
        if (transaction == null)
            return new() { new PaymentVoidFailedEvent(cmd.Id, "تراکنش یافت نشد") };

        if (transaction.Status == "Voided")
            return new() { new PaymentVoidedEvent(cmd.Id) };

        transaction.Status = "Voided";
        transaction.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(); 
        return new() { new PaymentVoidedEvent(cmd.Id) };
    }
}