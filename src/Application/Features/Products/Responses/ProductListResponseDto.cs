// Application/Features/Products/Responses/ProductListResponseDto.cs
using System.Collections.Generic;
using Application.DTOs.Product;
using Domain.Entities.Product;

namespace Application.Features.Products.Responses;

public class ProductListResponseDto
{
    public FilterProductDto Result { get; set; }
    public List<ProductColor> Colors { get; set; }
    public List<ProductSize> Sizes { get; set; }
}