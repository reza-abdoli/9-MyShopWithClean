using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Product;

public class CreateProductDto
{
    [Required]
    public string Title { get; set; }
    [Required]
    public long BrandId { get; set; }
    [Required]
    public bool IsAvailable { get; set; }
    [Required]
    public string BasePrice { get; set; } 
    [Required]
    public IFormFile MainImage { get; set; }
    [Required]
    public string? ShortDescription { get; set; }
    [Required]
    public string? Description { get; set; }
    [Required]
    public List<long> Categories { get; set; }
    [Required]
    public List<IFormFile>? ProductGalleries { get; set; }
}
