// Application/Features/Orders/Responses/OrderDetailResponseDto.cs
using Application.DTOs.Order;
using Domain.Entities.Order;

namespace Application.Features.Orders.Responses;

public class OrderDetailResponseDto
{
    public OrderDetailDto Model { get; set; }
    public OrderState[] AvailableStates { get; set; }
}