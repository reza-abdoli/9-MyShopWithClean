// Application/DTOs/Order/OrderItemDto.cs
namespace Application.DTOs.Order;

public class OrderItemDto
{
    public long Id { get; set; }
    public long ProductVariantId { get; set; }
    public int ProductPrice { get; set; }
    public int TotalPrice { get; set; }
    public int Count { get; set; }

    public long ProductId { get; set; }
    public string ProductTitle { get; set; }
    public string ProductMainImageName { get; set; }

    public string ColorTitle { get; set; }
    public string ColorCode { get; set; }
    public string SizeTitle { get; set; }
    public int StockCount { get; set; }
}