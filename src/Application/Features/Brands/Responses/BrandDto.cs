// Application/Features/Brands/Responses/BrandDto.cs
namespace Application.Features.Brands.Responses;

public class BrandDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public int Order { get; set; }
    public string? ImageName { get; set; }
}