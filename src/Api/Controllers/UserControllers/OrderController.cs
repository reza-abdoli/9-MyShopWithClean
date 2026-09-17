// Api/Controllers/UserControllers/OrderController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Account;
using Application.DTOs.Order;
using Application.Features.Orders.Requests.Commands;
using Application.Features.Orders.Requests.Queries;
using Application.Features.Orders.Responses;
using Application.Features.Users.Requests.Queries;
using Domain.Entities.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.UserControllers;

[Authorize]
[Route("api/orders")]
public class OrderController : ApiControllerBase
{
    [HttpGet("cart-variant-info")]
    public async Task<ActionResult<CartVariantInfoDto>> CartVariantInfo(long productVariantId)
    {
        var orderDetail = await Bus.InvokeAsync<OrderDetail?>(new GetCartItemForVariantQuery { UserId = UserId, ProductVariantId = productVariantId });

        if (orderDetail == null)
            return Ok(new CartVariantInfoDto { OrderDetailId = null, Count = 0 });

        return Ok(new CartVariantInfoDto { OrderDetailId = orderDetail.Id, Count = orderDetail.Count });
    }

    [HttpPost("cart-items")]
    public async Task<ActionResult<long>> AddProductToCart(SubmitOrderDetailDto dto)
    {
        dto.UserId = UserId;
        var orderDetailId = await Bus.InvokeAsync<long>(new AddProductToOrderCommand { SubmitOrderDetailDto = dto });
        return Ok(orderDetailId);
    }

    [HttpPut("cart-items/{orderDetailId}")]
    public async Task<ActionResult<bool>> ChangeOrderDetailCount(long orderDetailId, int newCount)
    {
        var success = await Bus.InvokeAsync<bool>(new ChangeOrderDetailCountCommand { OrderDetailId = orderDetailId, Count = newCount });
        return Ok(success);
    }

    [HttpGet("cart")]
    public async Task<ActionResult<OpenOrderDto?>> Cart()
    {
        var order = await Bus.InvokeAsync<OpenOrderDto?>(new UserOpenOrderDetailQuery { UserId = UserId });
        return Ok(order);
    }

    [HttpGet("checkout")]
    public async Task<ActionResult<CheckoutResponseDto>> Checkout()
    {
        var isProfileComplete = await Bus.InvokeAsync<bool>(new IsProfileDetailCompletedQuery { UserId = UserId });
        if (!isProfileComplete)
            return Ok(new CheckoutResponseDto { NeedsProfile = true });

        var order = await Bus.InvokeAsync<OpenOrderDto?>(new UserOpenOrderDetailQuery { UserId = UserId });
        var userDetail = await Bus.InvokeAsync<EditUserInfoDTO>(new GetEditUserDetailQuery { UserId = UserId });

        return Ok(new CheckoutResponseDto { NeedsProfile = false, Order = order, User = userDetail });
    }

    [HttpGet("paid")]
    public async Task<ActionResult<List<PaidOrderDto>>> PaidOrders()
    {
        var orders = await Bus.InvokeAsync<List<PaidOrderDto>>(new GetUserPaidOrdersQuery { UserId = UserId });
        return Ok(orders);
    }
}