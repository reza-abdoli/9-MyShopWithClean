// Application/Features/Banners/Handlers/Commands/CreateBannerCommandHandler.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Enums;
using Application.DTOs.Common;
using Application.Features.Banners.Requests.Commands;
using Application.Interfaces.FileStorage;
using Application.Interfaces.Persistence;
using Domain.Entities.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Features.Banners.Handlers.Commands;

public class CreateBannerCommandHandler
{
    private readonly IRepository<Banner> _bannerRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CreateBannerCommandHandler> _logger;

    public CreateBannerCommandHandler(
        IRepository<Banner> bannerRepository,
        IImageStorageService imageStorageService,
        IMemoryCache cache,
        ILogger<CreateBannerCommandHandler> logger)
    {
        _bannerRepository = bannerRepository;
        _imageStorageService = imageStorageService;
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(CreateBannerCommand command)
    {
        var dto = command.CreateBannerDto;

        var imageResult = await _imageStorageService.SaveImageAsync(dto.ImageFile, ImageCategory.Banner, 300, 150);
        // if (!imageResult.Success) return; // رفتار سرویس قبلی هم نتیجه رو چک نمی‌کرد؛ بی‌تغییر نگه داشتم

        var newBanner = new Banner
        {
            BannerLocation = dto.BannerLocation,
            Order = dto.Order,
            Url = dto.Url,
            Title = dto.Title,
            ImageName = imageResult.FileName
        };

        await _bannerRepository.AddEntity(newBanner);
        await _bannerRepository.SaveChanges();

        _logger.LogInformation("Banner created. BannerId: {BannerId}, Title: {Title}, Location: {BannerLocation}", newBanner.Id, newBanner.Title, newBanner.BannerLocation);

        var cachedBanners = _cache.Get<List<BannerItemDto>>("AllBanners");
        if (cachedBanners == null) return;

        var newBannerDto = new BannerItemDto
        {
            Title = newBanner.Title,
            Url = newBanner.Url,
            ImageName = newBanner.ImageName,
            Order = newBanner.Order,
            BannerLocation = newBanner.BannerLocation
        };

        cachedBanners.Add(newBannerDto);
        cachedBanners = cachedBanners.OrderBy(b => b.Order).ToList();
        _cache.Set("AllBanners", cachedBanners, TimeSpan.FromDays(1));
    }
}