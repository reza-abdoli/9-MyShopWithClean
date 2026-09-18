// Application/DTOs/Product/ProductListItemDto.cs
using System.Collections.Generic;

namespace Application.DTOs.Product;

public class ProductListItemDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public int Price { get; set; }
    public bool IsAvailable { get; set; }
    public string MainImageName { get; set; }
    public string? ShortDescription { get; set; }
    public long? BrandId { get; set; }
    public string? BrandTitle { get; set; }
    public int StockCount { get; set; }
    public List<long> CategoryIds { get; set; } = new();
    public List<string> CategoryTitles { get; set; } = new();
    public List<VariantColorDto> AvailableColors { get; set; } = new();
}

public class VariantColorDto
{
    public long ColorId { get; set; }
    public string ColorTitle { get; set; }
    public string ColorCode { get; set; }
}