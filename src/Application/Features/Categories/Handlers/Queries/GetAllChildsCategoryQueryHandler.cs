// Application/Features/Categories/Handlers/Queries/GetAllChildsCategoryQueryHandler.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Product;
using Application.Features.Categories.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Handlers.Queries;

public class GetAllChildsCategoryQueryHandler
{
    private readonly IRepository<ProductCategory> _categoryRepository;

    public GetAllChildsCategoryQueryHandler(IRepository<ProductCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<ProductCategoryDto>> Handle(GetAllChildsCategoryQuery query)
    {
        return await _categoryRepository.GetQuery()
            .Where(c => c.ParentId == query.ParentId)
            .OrderBy(o => o.Order)
            .Select(c => new ProductCategoryDto
            {
                Id = c.Id,
                ParentId = c.ParentId,
                Title = c.Title,
                Url = c.Url,
                MainImage = c.MainImage,
                IsActive = c.IsActive,
                ShowInHome = c.ShowInHome,
                Order = c.Order
            })
            .ToListAsync();
    }
}