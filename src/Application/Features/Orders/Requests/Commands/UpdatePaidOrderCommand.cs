// Application/Features/Orders/Requests/Commands/UpdatePaidOrderCommand.cs
namespace Application.Features.Orders.Requests.Commands;

public class UpdatePaidOrderCommand
{
    public long OrderId { get; set; }
    public long UserId { get; set; }
    public long PaymentTransactionId { get; set; }
}