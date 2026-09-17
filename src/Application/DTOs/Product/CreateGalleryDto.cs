using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Product;

public class CreateGalleryDto
{
    [Required]

    public long ProductId { get; set; }
    [Required]

    public IFormFile ImageName { get; set; }
    [Required]

    public int Order { get; set; }
}
