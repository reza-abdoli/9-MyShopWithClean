using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Category
{
    public class CreateCategoryDto
    {
        public long? ParentId { get; set; } 
        [Required]

        public string Title { get; set; }
        [Required]

        public string Url { get; set; }
        [Required]

        public int Order { get; set; }
        [Required]

        public bool ShowInHome { get; set; }
        [Required]

        public IFormFile MainImage { get; set; }
    }
}
