using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Product
{
    public class CreateBrandDto
    {
        [Required]

        public string Title { get; set; }
        [Required]

        public string Url { get; set; }
        [Required]

        public IFormFile Image { get; set; }
        [Required]

        public int Order { get; set; }
    }
}
