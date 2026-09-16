// Application/Features/User/Requests/Commands/WithdrawBalanceCommand.cs
namespace Application.Features.Users.Requests.Commands;

public class WithdrawBalanceCommand
{
    public long UserId { get; set; }
    public int Amount { get; set; }
}