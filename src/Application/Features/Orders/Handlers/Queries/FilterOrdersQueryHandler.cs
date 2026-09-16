// Application/Features/Orders/Handlers/Queries/FilterOrdersQueryHandler.cs
using System;
using System.Threading.Tasks;
using Application.DTOs.Order;
using Application.Features.Orders.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers.Queries;

public class FilterOrdersQueryHandler
{
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;

    public FilterOrdersQueryHandler(IRepository<Domain.Entities.Order.Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<FilterOrderDto> Handle(FilterOrdersQuery query)
    {
        var filter = query.FilterOrderDto;
        var q = _orderRepository.GetQuery()
            .Include(o => o.OrderDetails)
            .Include(o => o.PaymentTransaction)
            .OrderByDescending(d => d.CreatedAt)
            .AsQueryable();

        switch (filter.FilterOrderState)
        {
            case FilterOrderState.All:
                break;
            case FilterOrderState.Open:
                q = q.Where(d => d.OrderState == OrderState.Open);
                break;
            case FilterOrderState.Paid:
                q = q.Where(d => d.OrderState == OrderState.Paid);
                break;
            case FilterOrderState.Sent:
                q = q.Where(d => d.OrderState == OrderState.Sent);
                break;
            case FilterOrderState.Canceled:
                q = q.Where(d => d.OrderState == OrderState.Canceled);
                break;
            case FilterOrderState.Returned:
                q = q.Where(d => d.OrderState == OrderState.Returned);
                break;
            case FilterOrderState.Pending:
                q = q.Where(d => d.OrderState == OrderState.Pending);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!string.IsNullOrEmpty(filter.UserName))
            q = q.Where(p => EF.Functions.Like(p.UserName, $"%{filter.UserName}%"));

        if (filter.TransactionId is > 0)
            q = q.Where(o => o.TransactionId == filter.TransactionId);

        if (filter.PaymentRecordId is > 0)
            q = q.Where(o => o.PaymentTransaction  != null && o.PaymentTransaction.Id == filter.PaymentRecordId);

        if (!string.IsNullOrEmpty(filter.DestinationCity))
            q = q.Where(p => EF.Functions.Like(p.DestinationCity, $"%{filter.DestinationCity}%"));

        if (filter.MinimumPrice is > 0)
            q = q.Where(d => d.TotalPrice > filter.MinimumPrice);

        if (filter.UserId is > 0)
            q = q.Where(d => d.UserId == filter.UserId);

        filter.Data = await q.ToListAsync();
        return filter;
    }
}