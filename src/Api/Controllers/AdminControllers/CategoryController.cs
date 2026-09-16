// Api/Controllers/AdminControllers/CategoryController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Category;
using Application.Features.Categories.Requests.Commands;
using Application.Features.Categories.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.AdminControllers;

[Route("api/admin/categories")]
public class CategoryController : AdminControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto dto)
    {
        var success = await Bus.InvokeAsync<bool>(new CreateCategoryCommand { CreateCategoryDto = dto });
        return success ? Ok() : BadRequest(new { message = "url تکراری می‌باشد" });
    }

    [HttpGet]
    public async Task<ActionResult<FilterCategoryDto>> FilterCategory([FromQuery] FilterCategoryDto dto)
    {
        var filter = await Bus.InvokeAsync<FilterCategoryDto>(new FilterCategoryQuery { FilterCategoryDto = dto });
        return Ok(filter);
    }
}