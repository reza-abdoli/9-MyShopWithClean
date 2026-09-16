// Application/Features/Payments/Handlers/Commands/MarkTransactionAsFailedCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Payments.Requests.Commands;
using Domain.Entities.Order;
using Wolverine;

namespace Application.Features.Payments.Handlers.Commands;

public class MarkTransactionAsFailedCommandHandler
{
    private readonly IMessageBus _bus;

    public MarkTransactionAsFailedCommandHandler(IMessageBus bus)
    {
        _bus = bus;
    }

    public async Task Handle(MarkTransactionAsFailedCommand command)
    {
        await _bus.InvokeAsync(new UpdateTransactionStatusCommand
        {
            TransactionId = command.TransactionId,
            Status = PaymentStatus.Failed
        });
    }
}