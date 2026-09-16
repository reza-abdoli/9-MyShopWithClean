// Api/Controllers/AdminControllers/OrderController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Order;
using Application.Features.Orders.Requests.Commands;
using Application.Features.Orders.Requests.Queries;
using Application.Features.Orders.Responses;
using Domain.Entities.Order;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.AdminControllers;

[Route("api/admin/orders")]
public class OrderController : AdminControllerBase
{
    [HttpGet]
    public async Task<ActionResult<FilterOrderDto>> FilterOrder([FromQuery] FilterOrderDto filter)
    {
        var model = await Bus.InvokeAsync<FilterOrderDto>(new FilterOrdersQuery { FilterOrderDto = filter });
        return Ok(model);
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderDetailResponseDto>> OrderDetail(long orderId)
    {
        var model = await Bus.InvokeAsync<OrderDetailDto>(new Application.Features.Orders.Requests.Queries.OrderDetailQuery { OrderId = orderId });
        return Ok(new OrderDetailResponseDto { Model = model, AvailableStates = Enum.GetValues<OrderState>() });
    }

    [HttpPut("{orderId}/state")]
    public async Task<IActionResult> ProcessOrder(long orderId, OrderState orderState)
    {
        await Bus.InvokeAsync(new ProcessOrderCommand { OrderId = orderId, OrderState = orderState });
        return Ok();
    }
}