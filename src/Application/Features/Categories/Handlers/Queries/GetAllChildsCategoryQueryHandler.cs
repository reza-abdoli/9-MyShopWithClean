// Application/Features/Categories/Handlers/Queries/GetAllChildsCategoryQueryHandler.cs
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async Task<List<ProductCategory>> Handle(GetAllChildsCategoryQuery query)
    {
        return await _categoryRepository.GetQuery()
            .Where(c => c.ParentId == query.ParentId).OrderBy(o => o.Order).ToListAsync();
    }
}