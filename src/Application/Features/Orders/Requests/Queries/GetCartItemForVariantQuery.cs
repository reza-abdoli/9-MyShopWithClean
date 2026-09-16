// Application/Features/Orders/Requests/Queries/GetCartItemForVariantQuery.cs
namespace Application.Features.Orders.Requests.Queries;

public class GetCartItemForVariantQuery
{
    public long UserId { get; set; }
    public long ProductVariantId { get; set; }
}