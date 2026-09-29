// Application/Features/Payments/Requests/Commands/StartOnlinePaymentCommand.cs
namespace Application.Features.Payments.Requests.Commands;

public class StartOnlinePaymentCommand
{
    public Guid PaymentId { get; set; }
    public long UserId { get; set; }
    public long OrderId { get; set; }
    public int Amount { get; set; }
}