// Application/DTOs/Product/ProductCategoryDto.cs
namespace Application.DTOs.Product;

public class ProductCategoryDto
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string MainImage { get; set; }
    public bool IsActive { get; set; }
    public bool ShowInHome { get; set; }
    public int Order { get; set; }
    public string? ParentTitle { get; set; }
    public List<ProductCategoryDto> SubCategories { get; set; } = new();
}