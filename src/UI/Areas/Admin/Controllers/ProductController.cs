using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Services.Base;

namespace UI.Areas.Admin.Controllers;

public class ProductController : AdminBaseController
{
    private readonly IClient _client;

    public ProductController(IClient client)
    {
        _client = client;
    }

    #region Color

    [HttpGet("create-color")]
    public IActionResult CreateColor() => View();

    [HttpPost("create-color")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateColor(CreateColorDTo dto)
    {
        if (!ModelState.IsValid)
        {
            TempData[ErrorMessage] = "عنوان یا کد رنگ نباید خالی باشد";
            return View(dto);
        }

        await _client.ColorsPOSTAsync(dto);
        TempData[SuccessMessage] = SuccessText;
        return RedirectToAction("FilterColor");
    }

    [HttpGet("filter-color")]
    public async Task<IActionResult> FilterColor(FilterColorDto dto)
    {
        var filter = await _client.ColorsGETAsync(dto.Title, dto.Data);
        return View(filter);
    }

    #endregion

    #region Brand

    [HttpGet("create-brand")]
    public IActionResult CreateBrand() => View();

    [HttpPost("create-brand")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBrand(CreateBrandViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData[ErrorMessage] = "تایتل یا Url نباید خالی باشد";
            return View(vm);
        }

        try
        {
            var image = new FileParameter(vm.Image.OpenReadStream(), vm.Image.FileName, vm.Image.ContentType);
            await _client.BrandsPOSTAsync(vm.Title, vm.Url, image, vm.Order);

            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("FilterBrand");
        }
        catch (ApiException)
        {
            TempData[ErrorMessage] = "ذخیره عکس ناموفق بود";
            return View(vm);
        }
    }

    [HttpGet("filter-brand")]
    public async Task<IActionResult> FilterBrand(FilterBrandDto dto)
    {
        var filter = await _client.BrandsGETAsync(dto.Title, dto.Url, dto.Data);
        return View(filter);
    }

    #endregion

    #region Category

    [HttpGet("create-category")]
    public async Task<IActionResult> CreateCategory()
    {
        ViewData["ParentCategory"] = (await _client.ActiveAsync()).ToList();
        return View();
    }

    [HttpPost("create-category")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CreateCategoryViewModel vm)
    {
        ViewData["ParentCategory"] = (await _client.ActiveAsync()).ToList();

        if (!ModelState.IsValid)
        {
            TempData[ErrorMessage] = "مشخصات را کامل وارد کنید";
            return View(vm);
        }

        try
        {
            var image = new FileParameter(vm.MainImage.OpenReadStream(), vm.MainImage.FileName, vm.MainImage.ContentType);
            await _client.CategoriesPOSTAsync(vm.ParentId, vm.Title, vm.Url, vm.Order, vm.ShowInHome, image);

            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("FilterCategory");
        }
        catch (ApiException)
        {
            TempData[ErrorMessage] = "url تکراری می‌باشد";
            return View(vm);
        }
    }

    [HttpGet("filter-category")]
    public async Task<IActionResult> FilterCategory(FilterCategoryDto dto)
    {
        var filter = await _client.CategoriesGETAsync(dto.ParentId, dto.Title, dto.Url, dto.CategoryStatus, dto.Data);
        ViewData["ParentCategory"] = (await _client.ActiveAsync()).ToList();
        return View(filter);
    }

    #endregion

    #region Size

    [HttpGet("create-size")]
    public IActionResult CreateSize() => View();

    [HttpPost("create-size")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSize(CreateSizeDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData[ErrorMessage] = "مشخصات را کامل وارد کنید";
            return View(dto);
        }

        await _client.SizesPOSTAsync(dto);
        TempData[SuccessMessage] = SuccessText;
        return RedirectToAction("FilterSize");
    }

    [HttpGet("filter-size")]
    public async Task<IActionResult> FilterSize(FilterSizeDto dto)
    {
        var filter = await _client.SizesGETAsync(dto.Title, dto.SizeType, dto.Data);
        return View(filter);
    }

    #endregion

    #region Product

    [HttpGet("create-product")]
    public async Task<IActionResult> CreateProduct()
    {
        ViewData["Categories"] = (await _client.ActiveAsync()).ToList();
        ViewData["Brands"] = (await _client.AllAsync()).ToList();
        return View(new CreateProductViewModel());
    }

    [HttpPost("create-product")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(CreateProductViewModel vm)
    {
        ViewData["Categories"] = (await _client.ActiveAsync()).ToList();
        ViewData["Brands"] = (await _client.AllAsync()).ToList();

        if (!ModelState.IsValid)
        {
            TempData[ErrorMessage] = "مشخصات را کامل وارد کنید";
            return View(vm);
        }

        try
        {
            var mainImage = new FileParameter(vm.MainImage.OpenReadStream(), vm.MainImage.FileName, vm.MainImage.ContentType);
            var galleries = vm.ProductGalleries?
                .Select(f => new FileParameter(f.OpenReadStream(), f.FileName, f.ContentType))
                .ToList() ?? new List<FileParameter>();

            await _client.ProductsPOSTAsync(
                vm.Title, vm.BrandId, vm.IsAvailable, vm.BasePrice, mainImage,
                vm.ShortDescription, vm.Description, vm.Categories, galleries);

            TempData[SuccessMessage] = "محصول با موفقیت ایجاد شد.";
            return RedirectToAction("FilterProduct");
        }
        catch (ApiException ex)
        {
            TempData[ErrorMessage] = ex.Message;
            return View(vm);
        }
    }

    [HttpGet("filter-product")]
    public async Task<IActionResult> FilterProduct(FilterProductDto filter)
    {
        var result = await _client.FilterAsync(
            filter.Title, filter.BrandId, filter.CategoryId, filter.CategoryUrl, filter.ColorId,
            filter.MostPrice, filter.LeastPrice, filter.StartPrice, filter.EndPrice,
            filter.ProductOrder, filter.ProductStatus, filter.Data,
            filter.PageId, filter.PageCount, filter.AllEntitiesCount, filter.StartPage, filter.EndPage,
            filter.TakeEntity, filter.SkipEntity, filter.HowManyShowPageAfterAndBefore);

        ViewData["Categories"] = (await _client.ActiveAsync()).ToList();
        ViewData["Brands"] = (await _client.AllAsync()).ToList();
        return View(result);
    }

    [HttpGet("edit-product")]
    public async Task<IActionResult> EditProduct(long productId)
    {
        var previous = await _client.EditAsync(productId);
        var vm = new EditProductViewModel
        {
            ProductId = previous.ProductId,
            Title = previous.Title,
            IsAvailable = previous.IsAvailable,
            Price = previous.Price,
            ShortDescription = previous.ShortDescription,
            Description = previous.Description,
            BrandId = previous.BrandId,
            Categories = previous.Categories?.ToList() ?? new List<long>()
        };

        ViewData["Categories"] = (await _client.ActiveAsync()).ToList();
        ViewData["Brands"] = (await _client.AllAsync()).ToList();
        return View(vm);
    }

    [HttpPost("edit-product")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(EditProductViewModel vm)
    {
        ViewData["Categories"] = (await _client.ActiveAsync()).ToList();
        ViewData["Brands"] = (await _client.AllAsync()).ToList();

        if (vm.MainImage != null && !ModelState.IsValid) return View(vm);

        try
        {
            FileParameter? image = vm.MainImage != null && vm.MainImage.Length > 0
                ? new FileParameter(vm.MainImage.OpenReadStream(), vm.MainImage.FileName, vm.MainImage.ContentType)
                : null;

            await _client.ProductsPUTAsync(
                vm.ProductId, vm.Title, vm.IsAvailable, image!, vm.Price,
                vm.ShortDescription, vm.Description, vm.BrandId, vm.Categories);

            TempData[SuccessMessage] = "عملیات با موفقیت انجام شد.";
            return RedirectToAction("ProductDetail", new { productId = vm.ProductId });
        }
        catch (ApiException ex)
        {
            TempData[ErrorMessage] = ex.Message;
            return View(vm);
        }
    }

    [HttpGet("product-detail-{productId}")]
    public async Task<IActionResult> ProductDetail(long productId)
    {
        var detail = await _client.ProductsGET3Async(productId);
        return View(detail);
    }

    [HttpGet("create-variant")]
    public async Task<IActionResult> CreateVariant(long productId)
    {
        ViewData["ProductId"] = productId;
        ViewData["Colors"] = (await _client.ColorsGETAsync(null, null)).Data?.ToList() ?? new List<ProductColorDto>();
        ViewData["Sizes"] = (await _client.SizesGETAsync(null, null, null)).Data?.ToList() ?? new List<ProductSizeDto>();
        return View();
    }

    [HttpPost("create-variant")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVariant(CreateProductVariantDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ProductId"] = dto.ProductId;
            ViewData["Colors"] = (await _client.ColorsGETAsync(null, null)).Data?.ToList() ?? new List<ProductColorDto>();
            ViewData["Sizes"] = (await _client.SizesGETAsync(null, null, null)).Data?.ToList() ?? new List<ProductSizeDto>();
            return View(dto);
        }

        await _client.VariantsAsync(dto);
        return RedirectToAction("ProductDetail", new { productId = dto.ProductId });
    }

    [HttpGet("create-feature")]
    public IActionResult CreateFeature(long productId)
    {
        ViewData["ProductId"] = productId;
        return View();
    }

    [HttpPost("create-feature")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFeature(CreateProductFeatureDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _client.FeaturesAsync(dto);
        TempData[SuccessMessage] = SuccessText;
        return RedirectToAction("ProductDetail", new { productId = dto.ProductId });
    }

    #endregion
}