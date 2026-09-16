// Application/Features/Banners/Handlers/Queries/GetAllBannersQueryHandler.cs (اصلاح‌شده)
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Banners.Requests.Queries;
using Application.Features.Banners.Responses;
using Application.Interfaces.Persistence;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Banners.Handlers.Queries;

public class GetAllBannersQueryHandler
{
    private readonly IRepository<Banner> _bannerRepository;

    public GetAllBannersQueryHandler(IRepository<Banner> bannerRepository)
    {
        _bannerRepository = bannerRepository;
    }

    public async Task<List<BannerDto>> Handle(GetAllBannersQuery query)
    {
        var banners = await _bannerRepository.GetQuery().ToListAsync();
        return banners.Select(b => new BannerDto
        {
            Id = b.Id,
            Title = b.Title,
            Url = b.Url,
            ImageName = b.ImageName,
            Order = b.Order,
            BannerLocation = b.BannerLocation
        }).ToList();
    }
}