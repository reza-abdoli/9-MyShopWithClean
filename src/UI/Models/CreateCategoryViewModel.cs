// UI/Models/CreateCategoryViewModel.cs
namespace UI.Models;

public class CreateCategoryViewModel
{
    public string Title { get; set; }
    public string Url { get; set; }
    public int Order { get; set; }
    public bool ShowInHome { get; set; }
    public IFormFile MainImage { get; set; }
    public long? ParentId { get; set; }
}