// Application/Features/Products/Handlers/Commands/EditProductCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Common.Enums;
using Application.Features.Categories.Requests.Commands;
using Application.Features.Products.Requests.Commands;
using Application.Features.Products.Responses;
using Application.Interfaces.FileStorage;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Application.Features.Products.Handlers.Commands;

public class EditProductCommandHandler
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Brand> _brandRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMessageBus _bus;
    private readonly ILogger<EditProductCommandHandler> _logger;

    public EditProductCommandHandler(
        IRepository<Product> productRepository,
        IRepository<Brand> brandRepository,
        IImageStorageService imageStorageService,
        IMessageBus bus,
        ILogger<EditProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _imageStorageService = imageStorageService;
        _bus = bus;
        _logger = logger;
    }

    public async Task<EditProductResult> Handle(EditProductCommand command)
    {
        var dto = command.EditProductDto;
        var product = await _productRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == dto.ProductId);
        if (product == null)
        {
            _logger.LogWarning("Product edit failed: product not found. ProductId: {ProductId}", dto.ProductId);
            return EditProductResult.Error;
        }

        var oldValues = $"Title: {product.Title}, Price: {product.Price}, IsAvailable: {product.IsAvailable}, BrandId: {product.BrandId}";

        var convertedPrice = int.Parse(dto.Price.Replace(",", ""));

        product.Title = dto.Title;
        product.Description = dto.Description;
        product.ShortDescription = dto.ShortDescription;
        product.IsAvailable = dto.IsAvailable;
        product.Price = convertedPrice;

        switch (dto.BrandId)
        {
            case > 0:
                var brand = await _brandRepository.GetQuery().FirstOrDefaultAsync(d => d.Id == dto.BrandId);
                if (brand == null)
                {
                    _logger.LogWarning("Product edit failed: brand not found. ProductId: {ProductId}, BrandId: {BrandId}", dto.ProductId, dto.BrandId);
                    return EditProductResult.BrandNotFound;
                }
                product.BrandId = dto.BrandId;
                break;
            case 0:
                product.BrandId = null;
                break;
        }

        await _bus.InvokeAsync(new RemoveProductSelectedCategoriesCommand { ProductId = dto.ProductId });
        var addCategoryResult = await _bus.InvokeAsync<bool>(new AddProductSelectedCategoriesCommand
        {
            SelectedCategoryIds = dto.Categories,
            ProductId = dto.ProductId
        });
        if (!addCategoryResult)
        {
            _logger.LogWarning("Product edit failed: category not found. ProductId: {ProductId}", dto.ProductId);
            return EditProductResult.CategoryNotFound;
        }

        if (dto.MainImage != null)
        {
            var imageResult = await _imageStorageService.SaveImageAsync(dto.MainImage, ImageCategory.ProductImage, 150, 150, oldFileNameToDelete: product.MainImageName);
            if (!imageResult.Success)
            {
                _logger.LogWarning("Product edit failed: image save failed. ProductId: {ProductId}", dto.ProductId);
                return EditProductResult.ImageNotSaved;
            }
            product.MainImageName = imageResult.FileName;
        }

        _productRepository.UpdateEntity(product);
        await _productRepository.SaveChanges();

        var newValues = $"Title: {product.Title}, Price: {product.Price}, IsAvailable: {product.IsAvailable}, BrandId: {product.BrandId}";

        _logger.LogInformation(
            "Product updated. ProductId: {ProductId}. Old values -> {OldValues}. New values -> {NewValues}",
            dto.ProductId, oldValues, newValues);

        return EditProductResult.Success;
    }
}