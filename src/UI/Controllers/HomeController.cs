using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Services.Base;

namespace UI.Controllers;

public class HomeController : Controller
{
    private readonly IClient _client;

    public HomeController(IClient client)
    {
        _client = client;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var home = await _client.HomeAsync();
            ViewData["Banners"] = home.Banners?.ToList() ?? new List<BannerItemDto>();
            ViewData["Categories"] = home.Categories?.ToList() ?? new List<ProductCategoryDto>();
            ViewData["Products"] = home.Products?.ToList() ?? new List<ProductItemDto>();
        }
        catch (ApiException)
        {
            ViewData["Banners"] = new List<BannerItemDto>();
            ViewData["Categories"] = new List<ProductCategoryDto>();
            ViewData["Products"] = new List<ProductItemDto>();
        }

        return View();
    }

    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
}