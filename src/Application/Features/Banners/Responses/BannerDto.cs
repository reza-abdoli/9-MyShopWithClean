// Application/Features/Banners/Responses/BannerDto.cs
using Domain.Entities.Common;

namespace Application.Features.Banners.Responses;

public class BannerDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string ImageName { get; set; }
    public int Order { get; set; }
    public BannerLocation BannerLocation { get; set; }
}