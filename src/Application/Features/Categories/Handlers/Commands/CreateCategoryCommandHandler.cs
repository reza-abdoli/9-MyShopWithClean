// Application/Features/Categories/Handlers/Commands/CreateCategoryCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Common.Enums;
using Application.Features.Categories.Requests.Commands;
using Application.Interfaces.FileStorage;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Handlers.Commands;

public class CreateCategoryCommandHandler
{
    private readonly IRepository<ProductCategory> _categoryRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(IRepository<ProductCategory> categoryRepository, IImageStorageService imageStorageService, ILogger<CreateCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _imageStorageService = imageStorageService;
        _logger = logger;
    }

    public async Task<bool> Handle(CreateCategoryCommand command)
    {
        var dto = command.CreateCategoryDto;

        var urlInUse = await _categoryRepository.GetQuery().AnyAsync(c => c.Url == dto.Url);
        if (urlInUse)
        {
            _logger.LogWarning("Category creation failed: URL already in use. Url: {Url}", dto.Url);
            return false;
        }

        var imageResult = await _imageStorageService.SaveImageAsync(dto.MainImage, ImageCategory.Category, 150, 150);
        if (!imageResult.Success)
        {
            _logger.LogWarning("Category creation failed: image save failed. Title: {Title}", dto.Title);
            return false;
        }

        var category = new ProductCategory
        {
            Title = dto.Title,
            IsActive = true,
            Order = dto.Order,
            ParentId = dto.ParentId,
            Url = dto.Url,
            ShowInHome = dto.ShowInHome,
            MainImage = imageResult.FileName
        };

        await _categoryRepository.AddEntity(category);
        await _categoryRepository.SaveChanges();

        _logger.LogInformation("Category created. CategoryId: {CategoryId}, Title: {Title}, ParentId: {ParentId}", category.Id, category.Title, category.ParentId);
        return true;
    }
}