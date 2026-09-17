// FilterBrandDto.cs
using System.Collections.Generic;
using Application.Features.Brands.Responses; // BrandDto از همینجا میاد

namespace Application.DTOs.Product;

public class FilterBrandDto
{
    public string? Title { get; set; }
    public string? Url { get; set; }
    public List<BrandDto>? Data { get; set; }   // به‌جای List<Brand>
}