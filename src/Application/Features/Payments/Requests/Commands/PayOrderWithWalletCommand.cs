// Application/Features/Payments/Requests/Commands/PayOrderWithWalletCommand.cs
namespace Application.Features.Payments.Requests.Commands;

public class PayOrderWithWalletCommand
{
    public long UserId { get; set; }
    public int TotalCartPrice { get; set; }
    public long OrderId { get; set; }
}