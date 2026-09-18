// Application/DTOs/Product/FilterSizeDto.cs
using System.Collections.Generic;

namespace Application.DTOs.Product
{
    public class FilterSizeDto
    {
        public string? Title { get; set; }
        public string? SizeType { get; set; }
        public List<ProductSizeDto>? Data { get; set; } = new List<ProductSizeDto>();
    }
}