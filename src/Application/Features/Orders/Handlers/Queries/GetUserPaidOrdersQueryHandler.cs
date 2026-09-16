// Application/Features/Orders/Handlers/Queries/GetUserPaidOrdersQueryHandler.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Orders.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers.Queries;

public class GetUserPaidOrdersQueryHandler
{
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;

    public GetUserPaidOrdersQueryHandler(IRepository<Domain.Entities.Order.Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<Domain.Entities.Order.Order>> Handle(GetUserPaidOrdersQuery query)
    {
        return await _orderRepository.GetQuery()
            .Where(o => o.UserId == query.UserId && o.OrderState == OrderState.Paid)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.ProductVariant)
                    .ThenInclude(v => v.Product)
            .ToListAsync();
    }
}