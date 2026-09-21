// Application/Features/Orders/Handlers/Queries/CheckOrderDetailExistsQueryHandler.cs
using System.Threading.Tasks;
using Application.Features.Orders.Requests.Queries;

namespace Application.Features.Orders.Handlers.Queries;

public class CheckOrderDetailExistsQueryHandler
{
    private readonly IOrderHelper _orderHelper;

    public CheckOrderDetailExistsQueryHandler(IOrderHelper orderHelper)
    {
        _orderHelper = orderHelper;
    }

    public async Task<bool> Handle(CheckOrderDetailExistsQuery query)
    {
        return await _orderHelper.CheckOrderDetailExists(query.UserId, query.ProductVariantId);
    }
}