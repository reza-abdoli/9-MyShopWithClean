// Application/Features/Products/Handlers/Commands/CreateProductFeatureCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Products.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.Handlers.Commands;

public class CreateProductFeatureCommandHandler
{
    private readonly IRepository<ProductFeature> _productFeatureRepository;
    private readonly ILogger<CreateProductFeatureCommandHandler> _logger;

    public CreateProductFeatureCommandHandler(IRepository<ProductFeature> productFeatureRepository, ILogger<CreateProductFeatureCommandHandler> logger)
    {
        _productFeatureRepository = productFeatureRepository;
        _logger = logger;
    }

    public async Task Handle(CreateProductFeatureCommand command)
    {
        var dto = command.CreateProductFeatureDto;
        foreach (var item in dto.FeatureItems)
        {
            var productFeature = new ProductFeature
            {
                ProductId = dto.ProductId,
                Title = item.Title,
                Value = item.Value,
                Order = item.Order
            };
            await _productFeatureRepository.AddEntity(productFeature);
        }
        await _productFeatureRepository.SaveChanges();

        _logger.LogInformation("Product features created. ProductId: {ProductId}, Count: {Count}", dto.ProductId, dto.FeatureItems.Count);
    }
}