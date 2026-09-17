// Application/Features/Colors/Handlers/Queries/GetAllProductColorsQueryHandler.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Product;
using Application.Features.Colors.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Colors.Handlers.Queries;

public class GetAllProductColorsQueryHandler
{
    private readonly IRepository<ProductColor> _productColorRepository;

    public GetAllProductColorsQueryHandler(IRepository<ProductColor> productColorRepository)
    {
        _productColorRepository = productColorRepository;
    }

    // GetAllProductColorsQueryHandler.cs
    public async Task<List<ProductColorDto>> Handle(GetAllProductColorsQuery query)
    {
        return await _productColorRepository.GetQuery()
            .Select(c => new ProductColorDto { Id = c.Id, Title = c.Title, ColorCode = c.ColorCode })
            .ToListAsync();
    }
}