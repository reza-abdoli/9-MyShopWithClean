// Application/Features/Orders/Responses/CartVariantInfoDto.cs
namespace Application.Features.Orders.Responses;

public class CartVariantInfoDto
{
    public long? OrderDetailId { get; set; }
    public int Count { get; set; }
}