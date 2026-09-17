// Application/Features/Orders/Handlers/Queries/OrderDetailQueryHandler.cs
using System.Threading.Tasks;
using Application.DTOs.Order;
using Application.DTOs.Users;
using Application.Features.Orders.Requests.Queries;
using Application.Features.Payments.Responses;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers.Queries;

public class OrderDetailQueryHandler
{
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;

    public OrderDetailQueryHandler(IRepository<Domain.Entities.Order.Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDetailDto> Handle(OrderDetailQuery query)
    {
        return await _orderRepository.GetQuery()
            .Where(o => o.Id == query.OrderId)
            .Select(o => new OrderDetailDto
            {
                Id = o.Id,
                CreateDate = o.CreatedAt,
                LastUpdateDate = o.UpdatedAt,
                UserId = o.UserId,
                UserName = o.UserName,
                Address = o.Address,
                PostCode = o.PostCode,
                DestinationCity = o.DestinationCity,
                TotalPrice = o.TotalPrice,
                Description = o.Description,
                PostTraceCode = o.PostTraceCode,
                BankTraceCode = o.TransactionId.HasValue ? o.TransactionId.ToString() : string.Empty,
                OrderState = o.OrderState,
                User = new UserSummaryDto
                {
                    Id = o.User.Id,
                    Username = o.User.Username,
                    FullName = o.User.FullName,
                    MobileNumber = o.User.MobileNumber,
                    Email = o.User.Email
                },
                PaymentTransaction = o.PaymentTransaction == null ? null : new PaymentTransactionDto
                {
                    Id = o.PaymentTransaction.Id,
                    UserId = o.PaymentTransaction.UserId,
                    OrderId = o.PaymentTransaction.OrderId,
                    Amount = o.PaymentTransaction.Amount,
                    Authority = o.PaymentTransaction.Authority,
                    Status = o.PaymentTransaction.Status
                },
                OrderDetails = o.OrderDetails.Select(d => new OrderItemDto
                {
                    Id = d.Id,
                    ProductVariantId = d.ProductVariantId,
                    ProductPrice = d.ProductPrice,
                    TotalPrice = d.TotalPrice,
                    Count = d.Count,
                    ProductId = d.ProductVariant.Product.Id,
                    ProductTitle = d.ProductVariant.Product.Title,
                    ProductMainImageName = d.ProductVariant.Product.MainImageName,
                    ColorTitle = d.ProductVariant.ColorTitle,
                    ColorCode = d.ProductVariant.ColorCode,
                    SizeTitle = d.ProductVariant.SizeTitle
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}