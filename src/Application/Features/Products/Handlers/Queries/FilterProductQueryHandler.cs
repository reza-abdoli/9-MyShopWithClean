// Application/Features/Products/Handlers/Queries/FilterProductQueryHandler.cs
using System;
using System.Threading.Tasks;
using Application.DTOs.Product;
using Application.DTOs.Paging;
using Application.Features.Products.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Handlers.Queries;

public class FilterProductQueryHandler
{
    private readonly IRepository<Product> _productRepository;

    public FilterProductQueryHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<FilterProductDto> Handle(FilterProductQuery query)
    {
        var filter = query.FilterProductDto;
        var q = _productRepository.GetQuery()
            .AsNoTracking()
            .Include(p => p.ProductVariants)
            .Include(p => p.SelectedCategories)
                .ThenInclude(p => p.Category)
            .AsQueryable();

        switch (filter.ProductOrder)
        {
            case FilterProductOrder.Newest:
                q = q.OrderByDescending(d => d.CreatedAt);
                break;
            case FilterProductOrder.Oldest:
                q = q.OrderBy(d => d.CreatedAt);
                break;
            case FilterProductOrder.MostExpensive:
                q = q.OrderByDescending(d => d.Price);
                break;
            case FilterProductOrder.Cheapest:
                q = q.OrderBy(d => d.Price);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (filter.ProductStatus)
        {
            case FilterProductStatus.All:
                break;
            case FilterProductStatus.Available:
                q = q.Where(d => d.IsAvailable);
                break;
            case FilterProductStatus.NotAvailable:
                q = q.Where(d => !d.IsAvailable);
                break;
            case FilterProductStatus.HasStockCount:
                q = q.Where(p => p.ProductVariants.Any(p => p.StockCount > 0));
                break;
            case FilterProductStatus.HasZeroStockCount:
                q = q.Where(p => !p.ProductVariants.Any(p => p.StockCount > 0));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!string.IsNullOrEmpty(filter.Title))
            q = q.Where(p => EF.Functions.Like(p.Title, $"%{filter.Title}%"));

        if (!string.IsNullOrEmpty(filter.categoryUrl))
            q = q.Where(d => d.SelectedCategories.Any(s => s.Category.Url == filter.categoryUrl));

        if (filter.StartPrice != null)
            q = q.Where(d => d.Price > filter.StartPrice);

        if (filter.EndPrice != null)
            q = q.Where(d => d.Price < filter.EndPrice);

        if (filter is { StartPrice: not null, EndPrice: not null })
            q = q.Where(d => d.Price > filter.StartPrice && d.Price < filter.EndPrice);

        if (filter.CategoryId is > 0)
            q = q.Where(d => d.SelectedCategories.Any(s => s.CategoryId == filter.CategoryId));

        if (filter.ColorId is > 0)
            q = q.Where(d => d.ProductVariants.Any(v => v.ColorId == filter.ColorId));

        if (filter.BrandId is > 0)
            q = q.Where(d => d.BrandId == filter.BrandId);

        if (await q.AnyAsync())
        {
            filter.MostPrice = await q.OrderByDescending(d => d.Price).Select(d => d.Price).FirstAsync();
            filter.LeastPrice = await q.OrderBy(d => d.Price).Select(d => d.Price).FirstAsync();
        }

        var page = Page.BuildPage(filter.PageId, await q.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
        var entitiesForThisPage = await q.GetPageData(page).ToListAsync();

        return filter.SetData(entitiesForThisPage).SetPaging(page);
    }
}