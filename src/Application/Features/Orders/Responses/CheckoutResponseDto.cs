// Application/Features/Orders/Responses/CheckoutResponseDto.cs
using Application.DTOs.Account;
using Application.DTOs.Order;

namespace Application.Features.Orders.Responses;

public class CheckoutResponseDto
{
    public bool NeedsProfile { get; set; }
    public OpenOrderDto? Order { get; set; }
    public EditUserInfoDTO? User { get; set; }
}