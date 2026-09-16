// Application/Features/Categories/Handlers/Commands/AddProductSelectedCategoriesCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Categories.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Handlers.Commands;

public class AddProductSelectedCategoriesCommandHandler
{
    private readonly IRepository<ProductCategory> _categoryRepository;
    private readonly IRepository<ProductSelectedCategory> _productSelectedCategoryRepository;
    private readonly ILogger<AddProductSelectedCategoriesCommandHandler> _logger;

    public AddProductSelectedCategoriesCommandHandler(
        IRepository<ProductCategory> categoryRepository,
        IRepository<ProductSelectedCategory> productSelectedCategoryRepository,
        ILogger<AddProductSelectedCategoriesCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _productSelectedCategoryRepository = productSelectedCategoryRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(AddProductSelectedCategoriesCommand command)
    {
        foreach (var item in command.SelectedCategoryIds)
        {
            if (await _categoryRepository.GetEntityById(item) == null)
            {
                _logger.LogWarning("Adding product categories failed: category not found. ProductId: {ProductId}, CategoryId: {CategoryId}", command.ProductId, item);
                return false;
            }
        }

        foreach (var item in command.SelectedCategoryIds)
        {
            await _productSelectedCategoryRepository.AddEntity(new ProductSelectedCategory
            {
                ProductId = command.ProductId,
                CategoryId = item
            });
        }
        await _productSelectedCategoryRepository.SaveChanges();

        _logger.LogInformation("Product categories assigned. ProductId: {ProductId}, CategoryIds: {CategoryIds}", command.ProductId, string.Join(",", command.SelectedCategoryIds));
        return true;
    }
}