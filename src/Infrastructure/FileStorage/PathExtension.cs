// Infrastructure/FileStorage/PathExtension.cs
namespace Infrastructure.FileStorage;

public static class PathExtension
{
    #region Brand
    public static string BrandServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Brand/origin/");
    public static string BrandThumbServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Brand/thumb/");
    #endregion

    #region ProductImage
    public static string ProductImageServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/ProductImage/origin/");
    public static string ProductImageThumbServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/ProductImage/thumb/");
    #endregion

    #region Category
    public static string CategoryServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Category/origin/");
    public static string CategoryThumbServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Category/thumb/");
    #endregion

    #region ProductGallery
    public static string ProductGalleryServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/ProductGallery/origin/");
    public static string ProductGalleryThumbServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/ProductGallery/thumb/");
    #endregion

    #region Banner
    public static string BannerServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Banner/origin/");
    public static string BannerThumbServer =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Banner/thumb/");
    #endregion
}