// UI/Models/TreeViewCategoriesDto.cs
namespace UI.Models;

public class TreeViewCategoriesDto
{
    public int Level { get; set; }
    public List<UI.Services.Base.ProductCategory> productCategories { get; set; } = new();
    public List<long>? SelectedCategoriesIds { get; set; }
}