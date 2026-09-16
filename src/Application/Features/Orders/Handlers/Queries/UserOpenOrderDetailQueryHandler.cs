// Application/Features/Orders/Handlers/Queries/UserOpenOrderDetailQueryHandler.cs
using System.Threading.Tasks;
using Application.DTOs.Order;
using Application.Features.Orders.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers.Queries;

public class UserOpenOrderDetailQueryHandler
{
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;
    private readonly IRepository<OrderDetail> _orderDetailRepository;
    private readonly IRepository<User> _userRepository;
    private readonly OrderHelper _orderHelper;

    public UserOpenOrderDetailQueryHandler(
        IRepository<Domain.Entities.Order.Order> orderRepository,
        IRepository<OrderDetail> orderDetailRepository,
        IRepository<User> userRepository,
        OrderHelper orderHelper)
    {
        _orderRepository = orderRepository;
        _orderDetailRepository = orderDetailRepository;
        _userRepository = userRepository;
        _orderHelper = orderHelper;
    }

    public async Task<OpenOrderDto?> Handle(UserOpenOrderDetailQuery query)
    {
        var order = await _orderRepository.GetQuery()
            .FirstOrDefaultAsync(d => d.UserId == query.UserId && d.OrderState == OrderState.Open);

        if (order == null) return null;
        await _orderHelper.RemoveStaleOrderDetails(order.Id);

        return new OpenOrderDto
        {
            User = await _userRepository.GetEntityById(query.UserId),
            Order = order,
            OrderDetails = await _orderDetailRepository.GetQuery()
                .Include(d => d.ProductVariant)
                    .ThenInclude(v => v.Product)
                .Where(d => d.OrderId == order.Id && d.ProductVariant.StockCount > 0)
                .ToListAsync()
        };
    }
}