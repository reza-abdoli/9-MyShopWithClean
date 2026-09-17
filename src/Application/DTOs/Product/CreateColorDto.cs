using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Product
{
    public class CreateColorDTo
    {
        [Required]

        public string Title { get; set; }
        [Required]

        public string ColorCode { get; set; }
    }
}
