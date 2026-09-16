// FilterBrandsQueryHandler.cs
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Product;
using Application.Features.Brands.Requests.Queries;
using Application.Features.Brands.Responses;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Brands.Handlers.Queries;

public class FilterBrandsQueryHandler
{
    private readonly IRepository<Brand> _brandRepository;

    public FilterBrandsQueryHandler(IRepository<Brand> brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<FilterBrandDto> Handle(FilterBrandsQuery query)
    {
        var filter = query.FilterBrandDto;
        var q = _brandRepository.GetQuery()
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Title))
            q = q.Where(c => EF.Functions.Like(c.Title, $"%{filter.Title}%"));

        if (!string.IsNullOrEmpty(filter.Url))
            q = q.Where(c => EF.Functions.Like(c.Url, $"%{filter.Url}%"));

        return new FilterBrandDto
        {
            Title = filter.Title,
            Url = filter.Url,
            // Select مستقیم روی دیتابیس اجرا می‌شه، ستون Products اصلاً از دیتابیس خونده نمی‌شه
            Data = await q.Select(b => new BrandDto
            {
                Id = b.Id,
                Title = b.Title,
                Url = b.Url,
                ImageName = b.ImageName,
                Order = b.Order
            }).ToListAsync()
        };
    }
}