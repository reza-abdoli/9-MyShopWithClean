// Infrastructure/FileStorage/ImageStorageService.cs
using Application.Common.Enums;
using Application.Interfaces.FileStorage;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.FileStorage;

public class ImageStorageService : IImageStorageService
{
    public async Task<ImageSaveResult> SaveImageAsync(IFormFile image, ImageCategory category, int? width = null, int? height = null, bool hasThumbnail = true, string? oldFileNameToDelete = null)
    {
        if (!image.IsImage())
            return new ImageSaveResult { Success = false };

        var (originalPath, thumbPath) = GetPaths(category, hasThumbnail);

        if (!Directory.Exists(originalPath))
            Directory.CreateDirectory(originalPath);

        if (!string.IsNullOrEmpty(oldFileNameToDelete))
        {
            if (File.Exists(originalPath + oldFileNameToDelete))
                File.Delete(originalPath + oldFileNameToDelete);

            if (!string.IsNullOrEmpty(thumbPath) && File.Exists(thumbPath + oldFileNameToDelete))
                File.Delete(thumbPath + oldFileNameToDelete);
        }

        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);
        var fullOriginalPath = originalPath + fileName;

        await using (var stream = new FileStream(fullOriginalPath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        if (!string.IsNullOrEmpty(thumbPath))
        {
            if (!Directory.Exists(thumbPath))
                Directory.CreateDirectory(thumbPath);

            if (width != null && height != null)
            {
                var optimizer = new ImageOptimizer();
                optimizer.ImageResizer(fullOriginalPath, thumbPath + fileName, width, height);
            }
        }

        return new ImageSaveResult { Success = true, FileName = fileName };
    }

    public void DeleteImage(string fileName, ImageCategory category)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        var (originalPath, thumbPath) = GetPaths(category, true);

        if (File.Exists(originalPath + fileName))
            File.Delete(originalPath + fileName);

        if (!string.IsNullOrEmpty(thumbPath) && File.Exists(thumbPath + fileName))
            File.Delete(thumbPath + fileName);
    }

    private static (string original, string? thumb) GetPaths(ImageCategory category, bool hasThumbnail) => category switch
    {
        ImageCategory.Brand => (PathExtension.BrandServer, hasThumbnail ? PathExtension.BrandThumbServer : null),
        ImageCategory.ProductImage => (PathExtension.ProductImageServer, hasThumbnail ? PathExtension.ProductImageThumbServer : null),
        ImageCategory.Category => (PathExtension.CategoryServer, hasThumbnail ? PathExtension.CategoryThumbServer : null),
        ImageCategory.ProductGallery => (PathExtension.ProductGalleryServer, hasThumbnail ? PathExtension.ProductGalleryThumbServer : null),
        ImageCategory.Banner => (PathExtension.BannerServer, hasThumbnail ? PathExtension.BannerThumbServer : null),
        _ => throw new ArgumentOutOfRangeException(nameof(category))
    };
}