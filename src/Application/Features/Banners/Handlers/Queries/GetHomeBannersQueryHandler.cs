// Application/Features/Banners/Handlers/Queries/GetHomeBannersQueryHandler.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Common;
using Application.Features.Banners.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Features.Banners.Handlers.Queries;

public class GetHomeBannersQueryHandler
{
    private readonly IRepository<Banner> _bannerRepository;
    private readonly IMemoryCache _cache;

    public GetHomeBannersQueryHandler(IRepository<Banner> bannerRepository, IMemoryCache cache)
    {
        _bannerRepository = bannerRepository;
        _cache = cache;
    }

    public async Task<List<BannerItemDto>> Handle(GetHomeBannersQuery query)
    {
        var banners = await _cache.GetOrCreateAsync("AllBanners", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            return await _bannerRepository.GetQuery()
                .OrderBy(b => b.Order)
                .Select(b => new BannerItemDto
                {
                    Title = b.Title,
                    Url = b.Url,
                    ImageName = b.ImageName,
                    Order = b.Order,
                    BannerLocation = b.BannerLocation
                })
                .ToListAsync();
        });

        return banners ?? new List<BannerItemDto>();
    }
}