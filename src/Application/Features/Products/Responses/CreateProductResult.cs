namespace Application.Features.Products.Responses
{
    public enum CreateProductResult
    {
        Success,
        Error,
        SavingMainImageFailed,
        BrandNotFound,
        CategoryNotFound
    }

}