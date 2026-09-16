// Application/Features/Products/Handlers/Queries/GetNewestProductsQueryHandler.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Product;
using Application.Features.Products.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Features.Products.Handlers.Queries;

public class GetNewestProductsQueryHandler
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMemoryCache _cache;

    public GetNewestProductsQueryHandler(IRepository<Product> productRepository, IMemoryCache cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<List<ProductItemDto>> Handle(GetNewestProductsQuery query)
    {
        var products = await _cache.GetOrCreateAsync("NewProducts", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            return await _productRepository.GetQuery()
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .Select(p => new ProductItemDto
                {
                    ProductId = p.Id,
                    Title = p.Title,
                    Image = p.MainImageName,
                    Price = p.Price,
                })
                .ToListAsync();
        });

        return products ?? new List<ProductItemDto>();
    }
}