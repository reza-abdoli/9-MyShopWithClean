// Application/Features/Payments/Requests/Commands/DepositWalletBalanceCommand.cs
using Domain.Entities.Order;

namespace Application.Features.Payments.Requests.Commands;

public class DepositWalletBalanceCommand
{
    public PaymentTransaction PaymentTransaction { get; set; }
}