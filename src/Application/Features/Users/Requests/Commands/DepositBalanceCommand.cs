// Application/Features/User/Requests/Commands/DepositBalanceCommand.cs
using Application.DTOs.Account;

namespace Application.Features.Users.Requests.Commands;

public class DepositBalanceCommand
{
    public DepositBalanceDto DepositBalanceDto { get; set; }
}