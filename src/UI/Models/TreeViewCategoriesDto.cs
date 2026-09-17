// UI/Models/TreeViewCategoriesDto.cs
using UI.Services.Base;

namespace UI.Models;

public class TreeViewCategoriesDto
{
    public int Level { get; set; }
    public List<ProductCategoryDto> productCategories { get; set; } = new();
    public List<long>? SelectedCategoriesIds { get; set; }
}