using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Product;

public class CreateProductFeatureDto
{
    [Required]
    public long ProductId { get; set; }
    [Required]
    public List<CreateFeatureItemDTo> FeatureItems { get; set; }
}
