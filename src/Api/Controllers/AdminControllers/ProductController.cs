// Api/Controllers/AdminControllers/ProductController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Product;
using Application.Features.Products.Requests.Commands;
using Application.Features.Products.Requests.Queries;
using Application.Features.Products.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.AdminControllers;

[Route("api/admin/products")]
public class ProductController : AdminControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
    {
        var result = await Bus.InvokeAsync<CreateProductResult>(new CreateProductCommand { CreateProductDto = dto });

        return result switch
        {
            CreateProductResult.Success => Ok(),
            CreateProductResult.SavingMainImageFailed => BadRequest(new { message = "ذخیره‌ی تصویر محصول با خطا مواجه شد" }),
            CreateProductResult.BrandNotFound => BadRequest(new { message = "برند یافت نشد" }),
            CreateProductResult.CategoryNotFound => BadRequest(new { message = "دسته‌بندی انتخاب‌شده یافت نشد" }),
            _ => BadRequest(new { message = "عملیات با خطا مواجه شد" })
        };
    }

    [HttpGet("filter")]
    public async Task<ActionResult<FilterProductDto>> FilterProduct([FromQuery] FilterProductDto dto)
    {
        var filter = await Bus.InvokeAsync<FilterProductDto>(new FilterProductQuery { FilterProductDto = dto });
        return Ok(filter);
    }

    [HttpGet("{productId}/edit")]
    public async Task<ActionResult<EditProductDto>> GetEditProduct(long productId)
    {
        var model = await Bus.InvokeAsync<EditProductDto>(new GetEditProductQuery { ProductId = productId });
        return Ok(model);
    }

    [HttpPut]
    public async Task<IActionResult> EditProduct([FromForm] EditProductDto dto)
    {
        var result = await Bus.InvokeAsync<EditProductResult>(new EditProductCommand { EditProductDto = dto });

        return result switch
        {
            EditProductResult.Success => Ok(),
            EditProductResult.ImageNotSaved => BadRequest(new { message = "در ذخیره‌سازی تصویر خطایی رخ داده است" }),
            EditProductResult.BrandNotFound => BadRequest(new { message = "برند یافت نشد" }),
            EditProductResult.CategoryNotFound => BadRequest(new { message = "دسته‌بندی انتخاب‌شده یافت نشد" }),
            _ => BadRequest(new { message = "عملیات با خطا مواجه شد" })
        };
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductDetailDto>> ProductDetail(long productId)
    {
        var detail = await Bus.InvokeAsync<ProductDetailDto>(new GetProductDetailQuery { ProductId = productId });
        return Ok(detail);
    }

    [HttpPost("variants")]
    public async Task<IActionResult> CreateVariant(CreateProductVariantDto dto)
    {
        await Bus.InvokeAsync(new CreateProductVariantCommand { CreateProductVariantDto = dto });
        return Ok();
    }

    [HttpPost("features")]
    public async Task<IActionResult> CreateFeature(CreateProductFeatureDto dto)
    {
        await Bus.InvokeAsync(new CreateProductFeatureCommand { CreateProductFeatureDto = dto });
        return Ok();
    }
}