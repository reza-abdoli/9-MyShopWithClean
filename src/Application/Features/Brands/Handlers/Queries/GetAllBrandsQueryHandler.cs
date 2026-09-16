// Application/Features/Brands/Handlers/Queries/GetAllBrandsQueryHandler.cs (اصلاح‌شده)
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Brands.Requests.Queries;
using Application.Features.Brands.Responses;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Brands.Handlers.Queries;

public class GetAllBrandsQueryHandler
{
    private readonly IRepository<Brand> _brandRepository;

    public GetAllBrandsQueryHandler(IRepository<Brand> brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<List<BrandDto>> Handle(GetAllBrandsQuery query)
    {
        var brands = await _brandRepository.GetQuery().ToListAsync();
        return brands.Select(b => new BrandDto
        {
            Id = b.Id,
            Title = b.Title,
            Url = b.Url,
            Order = b.Order,
            ImageName = b.ImageName
        }).ToList();
    }
}