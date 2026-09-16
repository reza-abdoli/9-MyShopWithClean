// Application/Features/Products/Handlers/Commands/CreateProductCommandHandler.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Enums;
using Application.Features.Categories.Requests.Commands;
using Application.Features.Products.Requests.Commands;
using Application.Features.Products.Responses;
using Application.Interfaces.FileStorage;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Application.Features.Products.Handlers.Commands;

public class CreateProductCommandHandler
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Brand> _brandRepository;
    private readonly IRepository<ProductGallery> _productGalleryRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMessageBus _bus;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IRepository<Product> productRepository,
        IRepository<Brand> brandRepository,
        IRepository<ProductGallery> productGalleryRepository,
        IImageStorageService imageStorageService,
        IMessageBus bus,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _productGalleryRepository = productGalleryRepository;
        _imageStorageService = imageStorageService;
        _bus = bus;
        _logger = logger;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand command)
    {
        var dto = command.CreateProductDto;
        int price = int.Parse(dto.BasePrice.Replace(",", ""));
        var newProduct = new Product
        {
            Title = dto.Title,
            Description = dto.Description,
            ShortDescription = dto.ShortDescription,
            IsAvailable = dto.IsAvailable,
            Price = price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var imageResult = await _imageStorageService.SaveImageAsync(dto.MainImage, ImageCategory.ProductImage, 150, 150);
        if (!imageResult.Success)
        {
            _logger.LogWarning("Product creation failed: main image save failed. Title: {Title}", dto.Title);
            return CreateProductResult.SavingMainImageFailed;
        }
        newProduct.MainImageName = imageResult.FileName;

        if (dto.BrandId != 0 && await _brandRepository.GetEntityById(dto.BrandId) == null)
        {
            _logger.LogWarning("Product creation failed: brand not found. Title: {Title}, BrandId: {BrandId}", dto.Title, dto.BrandId);
            return CreateProductResult.BrandNotFound;
        }
        newProduct.BrandId = dto.BrandId == 0 ? null : dto.BrandId;

        await _productRepository.AddEntity(newProduct);
        await _productRepository.SaveChanges();

        var categoryResult = await _bus.InvokeAsync<bool>(new AddProductSelectedCategoriesCommand
        {
            SelectedCategoryIds = dto.Categories,
            ProductId = newProduct.Id
        });
        if (!categoryResult)
        {
            _logger.LogWarning("Product creation failed: category not found. ProductId: {ProductId}", newProduct.Id);
            return CreateProductResult.CategoryNotFound;
        }

        if (dto.ProductGalleries != null && dto.ProductGalleries.Any())
        {
            var galleryOrder = 1;
            foreach (var item in dto.ProductGalleries)
            {
                var galleryImageResult = await _imageStorageService.SaveImageAsync(item, ImageCategory.ProductGallery, 150, 150);

                var galleryItem = new ProductGallery
                {
                    ProductId = newProduct.Id,
                    Order = galleryOrder,
                    ImageName = galleryImageResult.FileName
                };

                await _productGalleryRepository.AddEntity(galleryItem);
                await _productGalleryRepository.SaveChanges();
                galleryOrder++;
            }
        }

        _logger.LogInformation("Product created. ProductId: {ProductId}, Title: {Title}, Price: {Price}", newProduct.Id, newProduct.Title, newProduct.Price);
        return CreateProductResult.Success;
    }
}