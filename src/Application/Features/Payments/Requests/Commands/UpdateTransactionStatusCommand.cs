// Application/Features/Payments/Requests/Commands/UpdateTransactionStatusCommand.cs
using Domain.Entities.Order;

namespace Application.Features.Payments.Requests.Commands;

public class UpdateTransactionStatusCommand
{
    public long TransactionId { get; set; }
    public PaymentStatus Status { get; set; }
}