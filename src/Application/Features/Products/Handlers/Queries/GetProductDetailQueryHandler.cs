// Application/Features/Products/Handlers/Queries/GetProductDetailQueryHandler.cs
using System.Threading.Tasks;
using Application.DTOs.Product;
using Application.Features.Products.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Handlers.Queries;

public class GetProductDetailQueryHandler
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ProductVariant> _productVariantRepository;
    private readonly IRepository<ProductSelectedCategory> _productSelectedCategoryRepository;
    private readonly IRepository<ProductGallery> _productGalleryRepository;
    private readonly IRepository<ProductFeature> _productFeatureRepository;

    public GetProductDetailQueryHandler(
        IRepository<Product> productRepository,
        IRepository<ProductVariant> productVariantRepository,
        IRepository<ProductSelectedCategory> productSelectedCategoryRepository,
        IRepository<ProductGallery> productGalleryRepository,
        IRepository<ProductFeature> productFeatureRepository)
    {
        _productRepository = productRepository;
        _productVariantRepository = productVariantRepository;
        _productSelectedCategoryRepository = productSelectedCategoryRepository;
        _productGalleryRepository = productGalleryRepository;
        _productFeatureRepository = productFeatureRepository;
    }

    public async Task<ProductDetailDto> Handle(GetProductDetailQuery query)
    {
        var data = await _productRepository.GetEntityById(query.ProductId);
        return new ProductDetailDto
        {
            Id = data.Id,
            Title = data.Title,
            IsDeleted = data.IsAvailable,
            LastUpdateDate = data.UpdatedAt,
            CreateDate = data.CreatedAt,
            Description = data.Description,
            MainImageName = data.MainImageName,
            IsAvailable = data.IsAvailable,
            ShortDescription = data.ShortDescription,
            Price = data.Price,
            ProductSelectedBrand = await _productRepository.GetQuery().AsNoTracking().Include(p => p.Brand).Where(p => p.Id == query.ProductId).Select(p => p.Brand).FirstOrDefaultAsync(),
            ProductVariants = await _productVariantRepository.GetQuery().AsNoTracking().Where(d => d.ProductId == query.ProductId).ToListAsync(),
            SelectedCategories = await _productSelectedCategoryRepository.GetQuery().AsNoTracking().Include(c => c.Category).Where(d => d.ProductId == query.ProductId).ToListAsync(),
            ProductGalleries = await _productGalleryRepository.GetQuery().AsNoTracking().Where(d => d.ProductId == query.ProductId).ToListAsync(),
            ProductFeatures = await _productFeatureRepository.GetQuery().AsNoTracking().Where(d => d.ProductId == query.ProductId).ToListAsync(),
        };
    }
}