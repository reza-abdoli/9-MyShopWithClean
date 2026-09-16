// Application/Features/Categories/Handlers/Queries/GetAllActiveCategoriesQueryHandler.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Categories.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Handlers.Queries;

public class GetAllActiveCategoriesQueryHandler
{
    private readonly IRepository<ProductCategory> _categoryRepository;

    public GetAllActiveCategoriesQueryHandler(IRepository<ProductCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<ProductCategory>> Handle(GetAllActiveCategoriesQuery query)
    {
        return await _categoryRepository.GetQuery().Where(c => c.IsActive).Include(c => c.Parent).ToListAsync();
    }
}