using Microsoft.AspNetCore.Mvc;
using UI.Services.Base;

namespace UI.Areas.User.Controllers;

public class AccountController : UserBaseController
{
    private readonly IClient _client;

    public AccountController(IClient client)
    {
        _client = client;
    }

    [HttpGet("edit-user-detail")]
    public async Task<IActionResult> EditUserDetail(bool returnToCheckout)
    {
        var model = await _client.EditDetailGETAsync();
        ViewData["ReturnToCheckout"] = returnToCheckout;
        return View(model);
    }

    [HttpPost("edit-user-detail")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUserDetail(EditUserInfoDTO dto, bool returnToCheckout)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnToCheckout"] = returnToCheckout;
            return View(dto);
        }

        try
        {
            await _client.EditDetailPUTAsync(dto);
            TempData[SuccessMessage] = SuccessText;
        }
        catch (ApiException)
        {
            TempData[ErrorMessage] = ErrorText;
        }

        if (returnToCheckout)
            return RedirectToAction("Checkout", "Order", new { area = "User" });

        return RedirectToAction("EditUserDetail", "Account", new { area = "User" });
    }
}