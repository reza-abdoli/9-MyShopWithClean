// Application/Features/Colors/Handlers/Queries/FilterColorsQueryHandler.cs
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Product;
using Application.Features.Colors.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Colors.Handlers.Queries;

public class FilterColorsQueryHandler
{
    private readonly IRepository<ProductColor> _productColorRepository;

    public FilterColorsQueryHandler(IRepository<ProductColor> productColorRepository)
    {
        _productColorRepository = productColorRepository;
    }

    public async Task<FilterColorDto> Handle(FilterColorsQuery query)
    {
        var filter = query.FilterColorDto;
        var q = _productColorRepository.GetQuery().OrderByDescending(c => c.CreatedAt).AsQueryable();
        if (!string.IsNullOrEmpty(filter.Title))
            q = q.Where(c => EF.Functions.Like(c.Title, $"%{filter.Title}%"));

        return new FilterColorDto
        {
            Title = filter.Title,
            Data = await q.ToListAsync()
        };
    }
}