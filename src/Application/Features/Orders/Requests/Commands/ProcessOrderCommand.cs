// Application/Features/Orders/Requests/Commands/ProcessOrderCommand.cs
using Domain.Entities.Order;

namespace Application.Features.Orders.Requests.Commands;

public class ProcessOrderCommand
{
    public long OrderId { get; set; }
    public OrderState OrderState { get; set; }
}