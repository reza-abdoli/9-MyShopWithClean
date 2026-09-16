// Application/Features/Orders/Requests/Commands/AddProductToOrderCommand.cs
using Application.DTOs.Order;

namespace Application.Features.Orders.Requests.Commands;

public class AddProductToOrderCommand
{
    public SubmitOrderDetailDto SubmitOrderDetailDto { get; set; }
}