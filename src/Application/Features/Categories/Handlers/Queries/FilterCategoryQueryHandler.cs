// Application/Features/Categories/Handlers/Queries/FilterCategoryQueryHandler.cs
using System;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.Features.Categories.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using static Application.DTOs.Category.FilterCategoryDto;

namespace Application.Features.Categories.Handlers.Queries;

public class FilterCategoryQueryHandler
{
    private readonly IRepository<ProductCategory> _categoryRepository;

    public FilterCategoryQueryHandler(IRepository<ProductCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<FilterCategoryDto> Handle(FilterCategoryQuery query)
    {
        var filter = query.FilterCategoryDto;
        var q = _categoryRepository.GetQuery().OrderByDescending(d => d.CreatedAt).AsQueryable();

        switch (filter.CategoryStatus)
        {
            case FilterCategoryStatus.All:
                break;
            case FilterCategoryStatus.Active:
                q = q.Where(d => d.IsActive);
                break;
            case FilterCategoryStatus.DeActive:
                q = q.Where(d => !d.IsActive);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!string.IsNullOrEmpty(filter.Title))
            q = q.Where(c => EF.Functions.Like(c.Title, $"%{filter.Title}%"));

        if (filter.ParentId is > 0)
            q = q.Where(c => c.ParentId == filter.ParentId);

        filter.Data = await q.ToListAsync();
        return filter;
    }
}