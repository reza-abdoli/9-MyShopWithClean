using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Product
{
    public class CreateFeatureItemDTo
    {
        [Required]

        public string Title { get; set; }
        [Required]

        public string Value { get; set; }
        [Required]

        public int Order { get; set; }
    }
}
