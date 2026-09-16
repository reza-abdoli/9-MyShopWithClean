// UI/Models/CreateProductViewModel.cs
namespace UI.Models;

public class CreateProductViewModel
{
    public string Title { get; set; }
    public string BasePrice { get; set; }
    public long BrandId { get; set; }
    public bool IsAvailable { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public IFormFile MainImage { get; set; }
    public List<IFormFile>? ProductGalleries { get; set; }
    public List<long> Categories { get; set; } = new();
}