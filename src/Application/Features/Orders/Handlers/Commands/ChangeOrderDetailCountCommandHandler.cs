// Application/Features/Orders/Handlers/Commands/ChangeOrderDetailCountCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Orders.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Handlers.Commands;

public class ChangeOrderDetailCountCommandHandler
{
    private readonly IRepository<OrderDetail> _orderDetailRepository;
    private readonly ILogger<ChangeOrderDetailCountCommandHandler> _logger;

    public ChangeOrderDetailCountCommandHandler(IRepository<OrderDetail> orderDetailRepository, ILogger<ChangeOrderDetailCountCommandHandler> logger)
    {
        _orderDetailRepository = orderDetailRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangeOrderDetailCountCommand command)
    {
        var orderDetail = await _orderDetailRepository.GetEntityById(command.OrderDetailId);

        if (command.Count == 0)
        {
            _orderDetailRepository.DeleteEntityPermanently(orderDetail);
            await _orderDetailRepository.SaveChanges();
            _logger.LogInformation("Order detail removed. OrderDetailId: {OrderDetailId}", command.OrderDetailId);
        }
        else
        {
            var variantCount = await _orderDetailRepository.GetQuery()
                .Include(o => o.ProductVariant)
                .Where(o => o.Id == command.OrderDetailId)
                .Select(o => o.ProductVariant.StockCount)
                .SingleOrDefaultAsync();

            if (variantCount < command.Count)
            {
                _logger.LogWarning("Order detail count change failed: not enough stock. OrderDetailId: {OrderDetailId}, RequestedCount: {RequestedCount}, StockCount: {StockCount}", command.OrderDetailId, command.Count, variantCount);
                return false;
            }

            var oldCount = orderDetail.Count;
            orderDetail.Count = command.Count;
            orderDetail.TotalPrice = orderDetail.ProductPrice * command.Count;
            _orderDetailRepository.UpdateEntity(orderDetail);
            await _orderDetailRepository.SaveChanges();

            _logger.LogInformation("Order detail count changed. OrderDetailId: {OrderDetailId}, OldCount: {OldCount}, NewCount: {NewCount}", command.OrderDetailId, oldCount, command.Count);
        }
        return true;
    }
}