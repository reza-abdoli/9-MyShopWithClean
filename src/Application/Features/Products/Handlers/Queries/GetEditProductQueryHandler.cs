// Application/Features/Products/Handlers/Queries/GetEditProductQueryHandler.cs
using System.Threading.Tasks;
using Application.DTOs.Product;
using Application.Features.Products.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Handlers.Queries;

public class GetEditProductQueryHandler
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ProductSelectedCategory> _productSelectedCategoryRepository;

    public GetEditProductQueryHandler(IRepository<Product> productRepository, IRepository<ProductSelectedCategory> productSelectedCategoryRepository)
    {
        _productRepository = productRepository;
        _productSelectedCategoryRepository = productSelectedCategoryRepository;
    }

    public async Task<EditProductDto> Handle(GetEditProductQuery query)
    {
        var data = await _productRepository.GetEntityById(query.ProductId);
        return new EditProductDto
        {
            Description = data.Description,
            IsAvailable = data.IsAvailable,
            Title = data.Title,
            ShortDescription = data.ShortDescription,
            ProductId = query.ProductId,
            BrandId = data.BrandId,
            Price = data.Price.ToString(),
            Categories = await _productSelectedCategoryRepository.GetQuery().Where(d => d.ProductId == query.ProductId)
                .Select(d => d.CategoryId).ToListAsync()
        };
    }
}