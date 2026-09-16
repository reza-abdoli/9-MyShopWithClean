// Application/Features/Banners/Responses/HomeDto.cs
using System.Collections.Generic;
using Application.DTOs.Common;
using Application.DTOs.Product;
using Domain.Entities.Product;

namespace Application.Features.Banners.Responses;

public class HomeDto
{
    public List<BannerItemDto> Banners { get; set; }
    public List<ProductCategory> Categories { get; set; }
    public List<ProductItemDto> Products { get; set; }
}