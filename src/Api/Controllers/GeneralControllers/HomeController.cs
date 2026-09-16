// Api/Controllers/GeneralControllers/HomeController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Common;
using Application.DTOs.Product;
using Application.Features.Banners.Requests.Queries;
using Application.Features.Banners.Responses;
using Application.Features.Categories.Requests.Queries;
using Application.Features.Products.Requests.Queries;
using Domain.Entities.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.GeneralControllers;

[AllowAnonymous]
[Route("api/home")]
public class HomeController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HomeDto>> Index()
    {
        var banners = await Bus.InvokeAsync<List<BannerItemDto>>(new GetHomeBannersQuery());
        var categories = await Bus.InvokeAsync<List<ProductCategory>>(new GetAllActiveCategoriesQuery());
        var products = await Bus.InvokeAsync<List<ProductItemDto>>(new GetNewestProductsQuery());

        return Ok(new HomeDto { Banners = banners, Categories = categories, Products = products });
    }
}