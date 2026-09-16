// Application/Features/Orders/Handlers/Queries/OrderDetailQueryHandler.cs
using System.Threading.Tasks;
using Application.DTOs.Order;
using Application.Features.Orders.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers.Queries;

public class OrderDetailQueryHandler
{
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;
    private readonly IRepository<OrderDetail> _orderDetailRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<PaymentTransaction> _paymentTransactionRepository;

    public OrderDetailQueryHandler(
        IRepository<Domain.Entities.Order.Order> orderRepository,
        IRepository<OrderDetail> orderDetailRepository,
        IRepository<User> userRepository,
        IRepository<PaymentTransaction> paymentTransactionRepository)
    {
        _orderRepository = orderRepository;
        _orderDetailRepository = orderDetailRepository;
        _userRepository = userRepository;
        _paymentTransactionRepository = paymentTransactionRepository;
    }

    public async Task<OrderDetailDto> Handle(OrderDetailQuery query)
    {
        var data = await _orderRepository.GetEntityById(query.OrderId);

        return new OrderDetailDto
        {
            Address = data.Address,
            CreateDate = data.CreatedAt,
            Description = data.Description,
            Id = data.Id,
            OrderState = data.OrderState,
            UserName = data.UserName,
            TotalPrice = data.TotalPrice,
            UserId = data.UserId,
            PostCode = data.PostCode,
            PostTraceCode = data.PostTraceCode,
            BankTraceCode = data.TransactionId?.ToString() ?? string.Empty,
            LastUpdateDate = data.UpdatedAt,
            DestinationCity = data.DestinationCity,
            User = await _userRepository.GetEntityById(data.UserId),
            OrderDetails = await _orderDetailRepository.GetQuery()
                .Include(d => d.ProductVariant)
                    .ThenInclude(v => v.Product)
                .Where(d => d.OrderId == query.OrderId)
                .ToListAsync(),
            PaymentTransaction = await _paymentTransactionRepository.GetQuery()
                .FirstOrDefaultAsync(r => r.OrderId == query.OrderId && r.Status == PaymentStatus.Success)
        };
    }
}