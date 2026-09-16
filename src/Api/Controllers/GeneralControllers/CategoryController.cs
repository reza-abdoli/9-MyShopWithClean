// Api/Controllers/GeneralControllers/CategoryController.cs
using Api.Controllers.BaseControllers;
using Application.Features.Categories.Requests.Queries;
using Domain.Entities.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.GeneralControllers;

[AllowAnonymous]
[Route("api/categories")]
public class CategoryController : ApiControllerBase
{
    [HttpGet("active")]
    public async Task<ActionResult<List<ProductCategory>>> GetActiveCategories()
    {
        var categories = await Bus.InvokeAsync<List<ProductCategory>>(new GetAllActiveCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("children")]
    public async Task<ActionResult<List<ProductCategory>>> GetChildren(long? parentId)
    {
        var children = await Bus.InvokeAsync<List<ProductCategory>>(new GetAllChildsCategoryQuery { ParentId = parentId });
        return Ok(children);
    }
}