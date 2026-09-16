// Application/Features/Orders/Requests/Queries/CheckOrderDetailExistsQuery.cs
namespace Application.Features.Orders.Requests.Queries;

public class CheckOrderDetailExistsQuery
{
    public long UserId { get; set; }
    public long ProductVariantId { get; set; }
}