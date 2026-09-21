// Application/Features/Orders/Handlers/Queries/UserOpenOrderDetailQueryHandler.cs
using System.Threading.Tasks;
using Application.DTOs.Order;
using Application.Features.Orders.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers.Queries;

public class UserOpenOrderDetailQueryHandler
{
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;
    private readonly IOrderHelper _orderHelper;

    public UserOpenOrderDetailQueryHandler(
        IRepository<Domain.Entities.Order.Order> orderRepository,
        IOrderHelper orderHelper)
    {
        _orderRepository = orderRepository;
        _orderHelper = orderHelper;
    }

    public async Task<OpenOrderDto?> Handle(UserOpenOrderDetailQuery query)
    {
        var order = await _orderRepository.GetQuery()
            .FirstOrDefaultAsync(d => d.UserId == query.UserId && d.OrderState == OrderState.Open);

        if (order == null) return null;
        await _orderHelper.RemoveStaleOrderDetails(order.Id);

        var orderDetails = await _orderRepository.GetQuery()
            .Where(o => o.Id == order.Id)
            .SelectMany(o => o.OrderDetails)
            .Where(d => d.ProductVariant.StockCount > 0)
            .Select(d => new OrderItemDto
            {
                Id = d.Id,
                ProductVariantId = d.ProductVariantId,
                ProductPrice = d.ProductVariant.Product.Price,
                TotalPrice = d.ProductVariant.Product.Price * d.Count,
                Count = d.Count,
                StockCount = d.ProductVariant.StockCount,
                ProductId = d.ProductVariant.Product.Id,
                ProductTitle = d.ProductVariant.Product.Title,
                ProductMainImageName = d.ProductVariant.Product.MainImageName,
                ColorTitle = d.ProductVariant.ColorTitle,
                ColorCode = d.ProductVariant.ColorCode,
                SizeTitle = d.ProductVariant.SizeTitle
            })
            .ToListAsync();

        return new OpenOrderDto
        {
            OrderId = order.Id,
            OrderDetails = orderDetails
        };
    }
}