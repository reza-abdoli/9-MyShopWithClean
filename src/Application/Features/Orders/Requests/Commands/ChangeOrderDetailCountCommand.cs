// Application/Features/Orders/Requests/Commands/ChangeOrderDetailCountCommand.cs
namespace Application.Features.Orders.Requests.Commands;

public class ChangeOrderDetailCountCommand
{
    public long OrderDetailId { get; set; }
    public int Count { get; set; }
}