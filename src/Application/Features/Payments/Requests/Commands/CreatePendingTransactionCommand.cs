// Application/Features/Payments/Requests/Commands/CreatePendingTransactionCommand.cs
namespace Application.Features.Payments.Requests.Commands;

public class CreatePendingTransactionCommand
{
    public long UserId { get; set; }
    public long? OrderId { get; set; }
    public int Amount { get; set; }
    public string Authority { get; set; }
}