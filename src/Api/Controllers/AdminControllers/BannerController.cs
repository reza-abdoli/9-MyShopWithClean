// Api/Controllers/AdminControllers/BannerController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Common;
using Application.Features.Banners.Requests.Commands;
using Application.Features.Banners.Requests.Queries;
using Application.Features.Banners.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.AdminControllers;

[Route("api/admin/banners")]
public class BannerController : AdminControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BannerDto>>> BannersList()
    {
        var banners = await Bus.InvokeAsync<List<BannerDto>>(new GetAllBannersQuery());
        return Ok(banners);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBanner([FromForm] CreateBannerDto dto)
    {
        await Bus.InvokeAsync(new CreateBannerCommand { CreateBannerDto = dto });
        return Ok();
    }
}