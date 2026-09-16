// Api/Controllers/AdminControllers/ColorController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs;
using Application.DTOs.Product;
using Application.Features.Colors.Requests.Commands;
using Application.Features.Colors.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.AdminControllers;

[Route("api/admin/colors")]
public class ColorController : AdminControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateColor(CreateColorDTo dto)
    {
        await Bus.InvokeAsync(new CreateColorCommand { CreateColorDto = dto });
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<FilterColorDto>> FilterColor([FromQuery] FilterColorDto dto)
    {
        var filter = await Bus.InvokeAsync<FilterColorDto>(new FilterColorsQuery { FilterColorDto = dto });
        return Ok(filter);
    }
}