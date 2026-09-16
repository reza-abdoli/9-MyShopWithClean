// Application/Features/Sizes/Handlers/Queries/FilterSizesQueryHandler.cs
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Product;
using Application.Features.Sizes.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sizes.Handlers.Queries;

public class FilterSizesQueryHandler
{
    private readonly IRepository<ProductSize> _productSizeRepository;

    public FilterSizesQueryHandler(IRepository<ProductSize> productSizeRepository)
    {
        _productSizeRepository = productSizeRepository;
    }

    public async Task<FilterSizeDto> Handle(FilterSizesQuery query)
    {
        var filter = query.FilterSizeDto;
        var q = _productSizeRepository.GetQuery().OrderByDescending(c => c.CreatedAt).AsQueryable();

        if (!string.IsNullOrEmpty(filter.Title))
            q = q.Where(c => EF.Functions.Like(c.Title, $"%{filter.Title}%"));

        if (!string.IsNullOrEmpty(filter.SizeType))
            q = q.Where(c => EF.Functions.Like(c.SizeType, $"%{filter.SizeType}%"));

        return new FilterSizeDto
        {
            Title = filter.Title,
            SizeType = filter.SizeType,
            Data = await q.ToListAsync()
        };
    }
}