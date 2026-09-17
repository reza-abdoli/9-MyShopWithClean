using Microsoft.AspNetCore.Mvc;
using UI.Services.Base;

namespace UI.Areas.Admin.Controllers;

public class OrderController : AdminBaseController
{
    private readonly IClient _client;

    public OrderController(IClient client)
    {
        _client = client;
    }

    [HttpGet("admin-filter-orders")]
    public async Task<IActionResult> FilterOrder(FilterOrderDto filter)
    {
        var model = await _client.OrdersAsync(
            filter.UserId, filter.MinimumPrice, filter.UserName, filter.DestinationCity,
            filter.TransactionId, filter.PaymentRecordId, filter.FilterOrderState, filter.Data);
        return View(model);
    }

    [HttpGet("admin-order-detail-{orderId}")]
    public async Task<IActionResult> OrderDetail(long orderId)
    {
        var response = await _client.Orders2Async(orderId);
        ViewData["States"] = response.AvailableStates;
        return View(response.Model);
    }

    [HttpPost("process-order")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessOrder(long orderId, OrderState orderState)
    {
        try
        {
            await _client.StateAsync(orderId, orderState);
            TempData[SuccessMessage] = SuccessText;
        }
        catch (ApiException)
        {
            TempData[ErrorMessage] = ErrorText;
        }

        return RedirectToAction("OrderDetail", new { orderId });
    }
}