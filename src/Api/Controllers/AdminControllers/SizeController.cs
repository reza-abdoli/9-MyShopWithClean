// Api/Controllers/AdminControllers/SizeController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs;
using Application.DTOs.Product;
using Application.Features.Sizes.Requests.Commands;
using Application.Features.Sizes.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.AdminControllers;

[Route("api/admin/sizes")]
public class SizeController : AdminControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateSize(CreateSizeDto dto)
    {
        await Bus.InvokeAsync(new CreateSizeCommand { CreateSizeDto = dto });
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<FilterSizeDto>> FilterSize([FromQuery] FilterSizeDto dto)
    {
        var filter = await Bus.InvokeAsync<FilterSizeDto>(new FilterSizesQuery { FilterSizeDto = dto });
        return Ok(filter);
    }
}