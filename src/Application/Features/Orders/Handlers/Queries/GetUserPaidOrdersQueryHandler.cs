// Application/Features/Orders/Handlers/Queries/GetUserPaidOrdersQueryHandler.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Order;
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

    public async Task<List<PaidOrderDto>> Handle(GetUserPaidOrdersQuery query)
    {
        return await _orderRepository.GetQuery()
            .Where(o => o.UserId == query.UserId && o.OrderState == OrderState.Paid)
            .Select(o => new PaidOrderDto
            {
                Id = o.Id,
                UserName = o.UserName,
                DestinationCity = o.DestinationCity,
                Address = o.Address,
                PostCode = o.PostCode,
                TotalPrice = o.TotalPrice,
                Description = o.Description,
                PostTraceCode = o.PostTraceCode,
                BankTraceCode = o.TransactionCode ?? string.Empty,   // ← تغییر اینجا
                PaymentDate = o.PaymentDate,
                OrderState = o.OrderState,
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
                    SizeTitle = d.ProductVariant.SizeTitle,
                    StockCount = d.ProductVariant.StockCount
                }).ToList()
            })
            .ToListAsync();
    }
}