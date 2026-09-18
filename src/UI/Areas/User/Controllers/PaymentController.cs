using Microsoft.AspNetCore.Mvc;
using UI.Services.Base;

namespace UI.Areas.User.Controllers;

public class PaymentController : UserBaseController
{
    private readonly IClient _client;

    public PaymentController(IClient client)
    {
        _client = client;
    }

    [HttpGet("pay-order")]
    public async Task<IActionResult> PayOrder(bool isWalletPayment)
    {
        try
        {
            var response = await _client.PayOrderAsync(isWalletPayment);

            if (response.WalletPayment)
            {
                if (response.Success)
                {
                    TempData[SuccessMessage] = "پرداخت شما با موفقیت انجام شد.";
                    return RedirectToAction("PaidOrders", "Order", new { area = "User" });
                }

                TempData[ErrorMessage] = ErrorText;
                return RedirectToAction("Cart", "Order", new { area = "User" });
            }

            ViewBag.Authority = response.Authority;
            return View("GoToGateway", response.Order);
        }
        catch (ApiException ex)
        {
            string userMessage;
            try
            {
                var json = System.Text.Json.JsonDocument.Parse(ex.Response);
                userMessage = json.RootElement.GetProperty("message").GetString();
            }
            catch
            {
                userMessage = "خطایی در پردازش پرداخت رخ داد.";
            }

            TempData[ErrorMessage] = userMessage;
            return RedirectToAction("Cart", "Order", new { area = "User" });
        }
    }

    [HttpPost("pay-order-callback")]
    public async Task<IActionResult> PayOrderCallback(string authority, bool isSuccess)
    {
        try
        {
            var finalized = await _client.PayOrderCallbackAsync(authority, isSuccess);

            if (finalized)
            {
                TempData[SuccessMessage] = "پرداخت شما با موفقیت انجام شد.";
                return RedirectToAction("PaidOrders", "Order", new { area = "User" });
            }

            TempData[ErrorMessage] = ErrorText;
            return RedirectToAction("Cart", "Order", new { area = "User" });
        }
        catch (ApiException ex)
        {
            string userMessage;
            try
            {
                var json = System.Text.Json.JsonDocument.Parse(ex.Response);
                userMessage = json.RootElement.GetProperty("message").GetString();
            }
            catch
            {
                userMessage = "خطایی در پردازش پرداخت رخ داد.";
            }

            TempData[ErrorMessage] = userMessage;
            return RedirectToAction("Cart", "Order", new { area = "User" });
        }
    }

    [HttpPost("add-balance")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBalance(DepositBalanceDto dto)
    {
        if (dto.Amount <= 0)
        {
            TempData[ErrorMessage] = "مبلغ باید بزرگ‌تر از صفر باشد!";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        try
        {
            var response = await _client.AddBalanceAsync(dto);

            if (response.NeedsProfile)
                return RedirectToAction("EditUserDetail", "Account", new { area = "User" });

            ViewBag.Authority = response.Authority;
            return View("PayBalance", dto);
        }
        catch (ApiException ex)
        {
            string userMessage;
            try
            {
                var json = System.Text.Json.JsonDocument.Parse(ex.Response);
                userMessage = json.RootElement.GetProperty("message").GetString();
            }
            catch
            {
                userMessage = "خطایی در پردازش پرداخت رخ داد.";
            }

            TempData[ErrorMessage] = userMessage;
            return RedirectToAction("Cart", "Order", new { area = "User" });
        }
    }

    [HttpPost("pay-balance-callback")]
    public async Task<IActionResult> PayBalanceCallback(string authority, bool isSuccess)
    {
        try
        {
            var deposited = await _client.PayBalanceCallbackAsync(authority, isSuccess);

            TempData[deposited ? SuccessMessage : ErrorMessage] =
                deposited ? "کیف پول شما با موفقیت شارژ شد." : ErrorText;
        }
        catch (ApiException ex)
        {
            string userMessage;
            try
            {
                var json = System.Text.Json.JsonDocument.Parse(ex.Response);
                userMessage = json.RootElement.GetProperty("message").GetString();
            }
            catch
            {
                userMessage = "خطایی در پردازش پرداخت رخ داد.";
            }

            TempData[ErrorMessage] = userMessage;
        }
        return RedirectToAction("Cart", "Order", new { area = "User" });

    }
}