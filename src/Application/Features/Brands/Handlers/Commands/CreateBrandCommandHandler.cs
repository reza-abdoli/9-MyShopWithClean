// Application/Features/Brands/Handlers/Commands/CreateBrandCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Common.Enums;
using Application.Features.Brands.Requests.Commands;
using Application.Interfaces.FileStorage;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.Extensions.Logging;

namespace Application.Features.Brands.Handlers.Commands;

public class CreateBrandCommandHandler
{
    private readonly IRepository<Brand> _brandRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly ILogger<CreateBrandCommandHandler> _logger;

    public CreateBrandCommandHandler(IRepository<Brand> brandRepository, IImageStorageService imageStorageService, ILogger<CreateBrandCommandHandler> logger)
    {
        _brandRepository = brandRepository;
        _imageStorageService = imageStorageService;
        _logger = logger;
    }

    public async Task<bool> Handle(CreateBrandCommand command)
    {
        var dto = command.CreateBrandDto;

        var imageResult = await _imageStorageService.SaveImageAsync(dto.Image, ImageCategory.Brand, 150, 150);
        if (!imageResult.Success)
        {
            _logger.LogWarning("Brand creation failed: image upload failed. Title: {Title}", dto.Title);
            return false;
        }

        var newBrand = new Brand
        {
            Title = dto.Title,
            Url = dto.Url,
            Order = dto.Order,
            ImageName = imageResult.FileName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _brandRepository.AddEntity(newBrand);
        await _brandRepository.SaveChanges();

        _logger.LogInformation("Brand created. BrandId: {BrandId}, Title: {Title}", newBrand.Id, newBrand.Title);
        return true;
    }
}