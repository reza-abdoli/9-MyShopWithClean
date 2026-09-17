// Api/Controllers/GeneralControllers/CategoryController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Product;
using Application.Features.Categories.Requests.Queries;
// using Domain.Entities.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.GeneralControllers;

[AllowAnonymous]
[Route("api/categories")]
public class CategoryController : ApiControllerBase
{
    [HttpGet("active")]
    public async Task<ActionResult<List<ProductCategoryDto>>> GetActiveCategories()
    {
        var categories = await Bus.InvokeAsync<List<ProductCategoryDto>>(new GetAllActiveCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("children")]
    public async Task<ActionResult<List<ProductCategoryDto>>> GetChildren(long? parentId)
    {
        var children = await Bus.InvokeAsync<List<ProductCategoryDto>>(new GetAllChildsCategoryQuery { ParentId = parentId });
        return Ok(children);
    }
}