// Application/Features/Orders/Handlers/Commands/ProcessOrderCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Orders.Requests.Commands;
using Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Handlers.Commands;

public class ProcessOrderCommandHandler
{
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;
    private readonly ILogger<ProcessOrderCommandHandler> _logger;

    public ProcessOrderCommandHandler(IRepository<Domain.Entities.Order.Order> orderRepository, ILogger<ProcessOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task Handle(ProcessOrderCommand command)
    {
        var data = await _orderRepository.GetEntityById(command.OrderId);
        var oldState = data.OrderState;
        data.OrderState = command.OrderState;

        _orderRepository.UpdateEntity(data);
        await _orderRepository.SaveChanges();

        _logger.LogInformation("Order state changed. OrderId: {OrderId}, OldState: {OldState}, NewState: {NewState}", command.OrderId, oldState, command.OrderState);
    }
}