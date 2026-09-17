using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Services.Base;

namespace UI.Areas.Admin.Controllers;

public class CommonController : AdminBaseController
{
    private readonly IClient _client;

    public CommonController(IClient client)
    {
        _client = client;
    }

    [HttpGet("banners")]
    public async Task<IActionResult> BannersList()
    {
        var banners = await _client.BannersAllAsync();
        return View(banners.ToList());
    }

    [HttpGet("create-banner")]
    public IActionResult CreateBanner() => View();

    [HttpPost("create-banner")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBanner(CreateBannerViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var image = new FileParameter(vm.ImageFile.OpenReadStream(), vm.ImageFile.FileName, vm.ImageFile.ContentType);
            await _client.BannersAsync(vm.Title, vm.Url, image, vm.Order, vm.BannerLocation);

            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("BannersList");
        }
        catch (ApiException)
        {
            TempData[ErrorMessage] = ErrorText;
            return View(vm);
        }
    }
}