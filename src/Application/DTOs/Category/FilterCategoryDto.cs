// Application/DTOs/Category/FilterCategoryDto.cs
using System.Collections.Generic;
using Application.DTOs.Product;

namespace Application.DTOs.Category
{
    public class FilterCategoryDto
    {
        public long? ParentId { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public FilterCategoryStatus CategoryStatus { get; set; }
        public List<ProductCategoryDto>? Data { get; set; }

        public enum FilterCategoryStatus
        {
            All,
            Active,
            DeActive
        }
    }
}