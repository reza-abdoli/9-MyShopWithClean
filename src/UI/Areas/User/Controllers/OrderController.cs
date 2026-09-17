using Microsoft.AspNetCore.Mvc;
using UI.Services.Base;

namespace UI.Areas.User.Controllers;

public class OrderController : UserBaseController
{
    private readonly IClient _client;

    public OrderController(IClient client)
    {
        _client = client;
    }

    [HttpGet("cart-variant-info")]
    public async Task<IActionResult> CartVariantInfo(long productVariantId)
    {
        try
        {
            var info = await _client.CartVariantInfoAsync(productVariantId);
            return Json(info);
        }
        catch (ApiException)
        {
            return Json(new { count = 0 });
        }
    }

    [HttpPost("add-product-to-cart")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProductToCart(SubmitOrderDetailDto dto)
    {
        try
        {
            var orderDetailId = await _client.CartItemsPOSTAsync(dto);
            return Json(new { success = true, orderDetailId });
        }
        catch (ApiException)
        {
            return Json(new { success = false });
        }
    }

    [HttpPost("change-order-detail-count")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeOrderDetailCount(long orderDetailId, int newCount)
    {
        try
        {
            var success = await _client.CartItemsPUTAsync(orderDetailId, newCount);
            return Json(new { success });
        }
        catch (ApiException)
        {
            return Json(new { success = false });
        }
    }

    [HttpGet("cart")]
    public async Task<IActionResult> Cart()
    {
        var order = await _client.CartAsync();
        return View(order);
    }

    [HttpGet("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var response = await _client.CheckoutAsync();

        if (response.NeedsProfile)
            return RedirectToAction("EditUserDetail", "Account", new { area = "User", returnToCheckout = true });

        ViewData["User"] = response.User;
        return View(response.Order);
    }

    [HttpGet("paid-orders")]
    public async Task<IActionResult> PaidOrders()
    {
        var orders = await _client.PaidAsync();
        return View(orders.ToList());
    }
}