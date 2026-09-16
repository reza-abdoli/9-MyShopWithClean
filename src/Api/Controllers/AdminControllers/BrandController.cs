// Api/Controllers/AdminControllers/BrandController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs;
using Application.DTOs.Product;
using Application.Features.Brands.Requests.Commands;
using Application.Features.Brands.Requests.Queries;
using Application.Features.Brands.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.AdminControllers;

[Route("api/admin/brands")]
public class BrandController : AdminControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBrand([FromForm] CreateBrandDto dto)
    {
        var success = await Bus.InvokeAsync<bool>(new CreateBrandCommand { CreateBrandDto = dto });
        return success ? Ok() : BadRequest(new { message = "ذخیره عکس ناموفق بود" });
    }

    [HttpGet]
    public async Task<ActionResult<FilterBrandDto>> FilterBrand([FromQuery] FilterBrandDto dto)
    {
        var filter = await Bus.InvokeAsync<FilterBrandDto>(new FilterBrandsQuery { FilterBrandDto = dto });
        return Ok(filter);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<BrandDto>>> GetAllBrands()
    {
        var brands = await Bus.InvokeAsync<List<BrandDto>>(new GetAllBrandsQuery());
        return Ok(brands);
    }
}