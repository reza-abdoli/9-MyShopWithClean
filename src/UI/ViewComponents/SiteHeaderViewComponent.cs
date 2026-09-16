// UI/ViewComponents/SiteHeaderViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using UI.Services.Base;

namespace UI.ViewComponents;

public class SiteHeaderViewComponent : ViewComponent
{
    private readonly IClient _client;

    public SiteHeaderViewComponent(IClient client)
    {
        _client = client;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        List<ProductCategory> categories;
        try
        {
            categories = (await _client.ActiveAsync()).ToList();
        }
        catch (ApiException)
        {
            categories = new List<ProductCategory>();
        }

        ViewData["Categories"] = categories;
        return View("Siteheader"); // دقیقاً هم‌نام فایل فعلی؛ پیشنهاد می‌کنم بعداً به SiteHeader.cshtml تغییرش بدی برای یکدستی
    }
}