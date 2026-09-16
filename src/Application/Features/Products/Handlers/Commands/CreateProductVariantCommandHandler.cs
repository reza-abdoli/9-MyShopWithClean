// Application/Features/Products/Handlers/Commands/CreateProductVariantCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Products.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Handlers.Commands;

public class CreateProductVariantCommandHandler
{
    private readonly IRepository<ProductVariant> _productVariantRepository;
    private readonly IRepository<ProductColor> _productColorRepository;
    private readonly IRepository<ProductSize> _productSizeRepository;
    private readonly ILogger<CreateProductVariantCommandHandler> _logger;

    public CreateProductVariantCommandHandler(
        IRepository<ProductVariant> productVariantRepository,
        IRepository<ProductColor> productColorRepository,
        IRepository<ProductSize> productSizeRepository,
        ILogger<CreateProductVariantCommandHandler> logger)
    {
        _productVariantRepository = productVariantRepository;
        _productColorRepository = productColorRepository;
        _productSizeRepository = productSizeRepository;
        _logger = logger;
    }

    public async Task Handle(CreateProductVariantCommand command)
    {
        var dto = command.CreateProductVariantDto;
        foreach (var item in dto.Variantitems)
        {
            var color = await _productColorRepository.GetEntityById(item.ColorId);
            var productVariant = new ProductVariant
            {
                ProductId = dto.ProductId,
                ColorId = item.ColorId,
                SizeId = item.SizeId,
                StockCount = item.StockCount,
                ColorTitle = color.Title,
                ColorCode = color.ColorCode,
                SizeTitle = await _productSizeRepository.GetQuery().Where(s => s.Id == item.SizeId).Select(s => s.Title).SingleOrDefaultAsync()
            };
            await _productVariantRepository.AddEntity(productVariant);
        }
        await _productVariantRepository.SaveChanges();

        _logger.LogInformation("Product variants created. ProductId: {ProductId}, Count: {Count}", dto.ProductId, dto.Variantitems.Count);
    }
}