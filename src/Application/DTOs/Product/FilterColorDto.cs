// Application/DTOs/Product/FilterColorDto.cs
using System.Collections.Generic;

namespace Application.DTOs.Product;

public class FilterColorDto
{
    public string? Title { get; set; }
    public List<ProductColorDto>? Data { get; set; }
}