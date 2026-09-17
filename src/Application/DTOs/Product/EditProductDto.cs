using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Product
{
    public class EditProductDto
    {
        [Required]
        public long ProductId { get; set; }
        [Required]
        public string Title { get; set; }
        public bool IsAvailable { get; set; }
        public IFormFile? MainImage { get; set; }
        public string Price { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public long? BrandId { get; set; }
        public List<long> Categories { get; set; }
    }
}
