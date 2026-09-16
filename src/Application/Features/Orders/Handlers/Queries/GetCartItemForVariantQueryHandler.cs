// Application/Features/Orders/Handlers/Queries/GetCartItemForVariantQueryHandler.cs
using System.Threading.Tasks;
using Application.Features.Orders.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers.Queries;

public class GetCartItemForVariantQueryHandler
{
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;
    private readonly IRepository<OrderDetail> _orderDetailRepository;

    public GetCartItemForVariantQueryHandler(IRepository<Domain.Entities.Order.Order> orderRepository, IRepository<OrderDetail> orderDetailRepository)
    {
        _orderRepository = orderRepository;
        _orderDetailRepository = orderDetailRepository;
    }

    public async Task<OrderDetail?> Handle(GetCartItemForVariantQuery query)
    {
        var order = await _orderRepository.GetQuery()
            .Where(o => o.UserId == query.UserId && o.OrderState == OrderState.Open)
            .FirstOrDefaultAsync();

        if (order == null) return null;

        return await _orderDetailRepository.GetQuery()
            .Where(d => d.OrderId == order.Id && d.ProductVariantId == query.ProductVariantId)
            .FirstOrDefaultAsync();
    }
}