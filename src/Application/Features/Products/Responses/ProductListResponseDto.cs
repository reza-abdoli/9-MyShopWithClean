// Application/Features/Products/Responses/ProductListResponseDto.cs
using System.Collections.Generic;
using Application.DTOs.Product;
using Domain.Entities.Product;

namespace Application.Features.Products.Responses;

public class ProductListResponseDto
{
    public FilterProductDto Result { get; set; }
    public List<ProductColorDto> Colors { get; set; }
    public List<ProductSizeDto> Sizes { get; set; }
}