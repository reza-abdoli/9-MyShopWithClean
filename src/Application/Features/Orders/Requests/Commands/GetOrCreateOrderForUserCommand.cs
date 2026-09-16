// Application/Features/Orders/Requests/Commands/GetOrCreateOrderForUserCommand.cs
namespace Application.Features.Orders.Requests.Commands;

public class GetOrCreateOrderForUserCommand
{
    public long UserId { get; set; }
}