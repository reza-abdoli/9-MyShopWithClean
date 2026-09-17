// Application/Features/Sizes/Handlers/Queries/GetAllProductSizesQueryHandler.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Product;
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

    // GetAllProductSizesQueryHandler.cs
    public async Task<List<ProductSizeDto>> Handle(GetAllProductSizesQuery query)
    {
        return await _productSizeRepository.GetQuery()
            .Select(s => new ProductSizeDto { Id = s.Id, Title = s.Title, SizeType = s.SizeType, Order = s.Order })
            .ToListAsync();
    }
}