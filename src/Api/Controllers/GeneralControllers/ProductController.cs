// Api/Controllers/GeneralControllers/ProductController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Product;
using Application.Features.Products.Requests.Queries;
using Application.Features.Products.Responses;
using Domain.Entities.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.GeneralControllers;

[AllowAnonymous]
[Route("api/products")]
public class ProductController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ProductListResponseDto>> FilterProducts([FromQuery] FilterProductDto filter, [FromQuery] string? url)
    {
        if (url != null) filter.categoryUrl = url;

        var result = await Bus.InvokeAsync<FilterProductDto>(new FilterProductQuery { FilterProductDto = filter });
        var colors = await Bus.InvokeAsync<List<ProductColor>>(new Application.Features.Colors.Requests.Queries.GetAllProductColorsQuery());
        var sizes = await Bus.InvokeAsync<List<ProductSize>>(new Application.Features.Sizes.Requests.Queries.GetAllProductSizesQuery());

        return Ok(new ProductListResponseDto { Result = result, Colors = colors, Sizes = sizes });
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductDetailDto>> ProductDetail(long productId)
    {
        var model = await Bus.InvokeAsync<ProductDetailDto>(new GetProductDetailQuery { ProductId = productId });
        return Ok(model);
    }
}