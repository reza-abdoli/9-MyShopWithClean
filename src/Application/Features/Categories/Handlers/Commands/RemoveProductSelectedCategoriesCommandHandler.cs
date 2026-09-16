// Application/Features/Categories/Handlers/Commands/RemoveProductSelectedCategoriesCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Categories.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Handlers.Commands;

public class RemoveProductSelectedCategoriesCommandHandler
{
    private readonly IRepository<ProductSelectedCategory> _productSelectedCategoryRepository;
    private readonly ILogger<RemoveProductSelectedCategoriesCommandHandler> _logger;

    public RemoveProductSelectedCategoriesCommandHandler(IRepository<ProductSelectedCategory> productSelectedCategoryRepository, ILogger<RemoveProductSelectedCategoriesCommandHandler> logger)
    {
        _productSelectedCategoryRepository = productSelectedCategoryRepository;
        _logger = logger;
    }

    public async Task Handle(RemoveProductSelectedCategoriesCommand command)
    {
        var selectedCategories = await _productSelectedCategoryRepository.GetQuery()
            .Where(c => c.ProductId == command.ProductId).ToListAsync();

        foreach (var item in selectedCategories)
        {
            _productSelectedCategoryRepository.DeleteEntityPermanently(item);
        }

        _logger.LogInformation("Product categories removed. ProductId: {ProductId}, Count: {Count}", command.ProductId, selectedCategories.Count);
    }
}