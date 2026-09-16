// Application/Features/Payments/Requests/Commands/MarkTransactionAsFailedCommand.cs
namespace Application.Features.Payments.Requests.Commands;

public class MarkTransactionAsFailedCommand
{
    public long TransactionId { get; set; }
}