// UI/Common/ImageUrlService.cs
namespace UI.Common;

public class ImageUrlService : IImageUrlService
{
    private readonly string _baseUrl;

    public ImageUrlService(IConfiguration configuration)
    {
        _baseUrl = configuration.GetValue<string>("ApiAddress")!.TrimEnd('/');
    }

    private string Build(string folder, string sub, string? fileName) =>
        string.IsNullOrEmpty(fileName) ? string.Empty : $"{_baseUrl}/content/images/{folder}/{sub}/{fileName}";

    public string ProductImage(string? fileName) => Build("ProductImage", "origin", fileName);
    public string ProductImageThumb(string? fileName) => Build("ProductImage", "thumb", fileName);
    public string ProductGalleryImage(string? fileName) => Build("ProductGallery", "origin", fileName);
    public string ProductGalleryThumb(string? fileName) => Build("ProductGallery", "thumb", fileName);
    public string CategoryThumb(string? fileName) => Build("Category", "thumb", fileName);
    public string BrandThumb(string? fileName) => Build("Brand", "thumb", fileName);
    public string BannerThumb(string? fileName) => Build("Banner", "thumb", fileName);
}