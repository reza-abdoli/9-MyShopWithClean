// Application/Features/User/Handlers/Queries/UserDashboardDetailQueryHandler.cs
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Account;
using Application.Features.Users.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
namespace Application.Features.Users.Handlers.Queries;

public class UserDashboardDetailQueryHandler
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Order> _orderRepository;

    public UserDashboardDetailQueryHandler(IRepository<User> userRepository, IRepository<Order> orderRepository)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
    }

    public async Task<UserDashboardDetailDto> Handle(UserDashboardDetailQuery query)
    {
        var orders = await _orderRepository.GetQuery().Where(o => o.UserId == query.UserId).ToListAsync();

        return new UserDashboardDetailDto
        {
            User = await _userRepository.GetEntityById(query.UserId),
            CanceledOrderCount = orders.Count(o => o.OrderState == OrderState.Canceled),
            PendingOrderCount = orders.Count(o => o.OrderState == OrderState.Pending),
            ReturnedOrderCount = orders.Count(o => o.OrderState == OrderState.Returned),
            SentOrderCount = orders.Count(o => o.OrderState == OrderState.Sent),
            PendingOrders = orders.Where(o => o.OrderState == OrderState.Pending).ToList()
        };
    }
}