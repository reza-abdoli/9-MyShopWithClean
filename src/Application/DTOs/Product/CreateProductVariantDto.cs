using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Product
{
    public class CreateProductVariantDto
    {
        [Required]

        public long ProductId { get; set; }
        [Required]

        public List<CreateVariantItemDto> Variantitems { get; set; }
    }
}
