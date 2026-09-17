using Microsoft.AspNetCore.Mvc;
using UI.Services.Base;

namespace UI.Controllers;

public class ProductController : Controller
{
    private readonly IClient _client;

    public ProductController(IClient client)
    {
        _client = client;
    }

    [HttpGet("products-list")]
    public async Task<IActionResult> FilterProducts(FilterProductDto filter, string? url)
    {
        if (url != null) filter.CategoryUrl = url;

        var response = await _client.ProductsGETAsync(
            filter.Title, filter.BrandId, filter.CategoryId, filter.CategoryUrl, filter.ColorId,
            filter.MostPrice, filter.LeastPrice, filter.StartPrice, filter.EndPrice,
            filter.ProductOrder, filter.ProductStatus, filter.Data,
            filter.PageId, filter.PageCount, filter.AllEntitiesCount, filter.StartPage, filter.EndPage,
            filter.TakeEntity, filter.SkipEntity, filter.HowManyShowPageAfterAndBefore, url);

        ViewData["Colors"] = response.Colors?.ToList() ?? new List<ProductColorDto>();
        ViewData["Sizes"] = response.Sizes?.ToList() ?? new List<ProductSizeDto>();

        return View(response.Result);
    }

    [HttpGet("product-detail")]
    public async Task<IActionResult> ProductDetail(long productId)
    {
        var model = await _client.ProductsGET2Async(productId);
        return View(model);
    }
}