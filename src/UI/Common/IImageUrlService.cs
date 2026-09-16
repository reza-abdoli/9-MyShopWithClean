// UI/Common/IImageUrlService.cs
namespace UI.Common;

public interface IImageUrlService
{
    string ProductImage(string? fileName);
    string ProductImageThumb(string? fileName);
    string ProductGalleryImage(string? fileName);
    string ProductGalleryThumb(string? fileName);
    string CategoryThumb(string? fileName);
    string BrandThumb(string? fileName);
    string BannerThumb(string? fileName);
}