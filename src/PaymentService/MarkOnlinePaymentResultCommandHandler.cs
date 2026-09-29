// PaymentService/MarkOnlinePaymentResultCommandHandler.cs
using KafkaContracts.Payments;
using PaymentService.Data;
using Wolverine;

namespace PaymentService;

public class MarkOnlinePaymentResultCommandHandler
{
    public async Task<OutgoingMessages> Handle(MarkOnlinePaymentResultCommand cmd, PaymentDbContext db)
    {
        var tx = await db.PaymentTransactions.FindAsync(cmd.Id);

        if (tx == null || tx.Status != "Pending")
            return new();

        tx.UpdatedAt = DateTime.UtcNow;

        if (cmd.IsSuccess)
        {
            tx.Status = "Success";
            await db.SaveChangesAsync();
            return new() { new OnlinePaymentConfirmedEvent(cmd.Id) };
        }

        tx.Status = "Failed";
        await db.SaveChangesAsync();
        return new() { new OnlinePaymentDeclinedEvent(cmd.Id, cmd.FailureReason ?? "پرداخت توسط بانک تأیید نشد.") };
    }
}