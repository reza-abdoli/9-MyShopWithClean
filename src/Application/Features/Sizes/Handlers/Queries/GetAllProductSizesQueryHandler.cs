// Application/Features/Sizes/Handlers/Queries/GetAllProductSizesQueryHandler.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Sizes.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sizes.Handlers.Queries;

public class GetAllProductSizesQueryHandler
{
    private readonly IRepository<ProductSize> _productSizeRepository;

    public GetAllProductSizesQueryHandler(IRepository<ProductSize> productSizeRepository)
    {
        _productSizeRepository = productSizeRepository;
    }

    public async Task<List<ProductSize>> Handle(GetAllProductSizesQuery query)
    {
        return await _productSizeRepository.GetQuery().ToListAsync();
    }
}