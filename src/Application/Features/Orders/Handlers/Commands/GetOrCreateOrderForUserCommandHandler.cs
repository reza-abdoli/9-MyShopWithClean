// Application/Features/Orders/Handlers/Commands/GetOrCreateOrderForUserCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Orders.Requests.Commands;

namespace Application.Features.Orders.Handlers.Commands;

public class GetOrCreateOrderForUserCommandHandler
{
    private readonly OrderHelper _orderHelper;

    public GetOrCreateOrderForUserCommandHandler(OrderHelper orderHelper)
    {
        _orderHelper = orderHelper;
    }

    public async Task<long> Handle(GetOrCreateOrderForUserCommand command)
    {
        return await _orderHelper.GetOrCreateOrderForUser(command.UserId);
    }
}