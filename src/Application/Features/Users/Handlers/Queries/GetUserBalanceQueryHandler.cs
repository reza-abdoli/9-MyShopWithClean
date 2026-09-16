// Application/Features/User/Handlers/Queries/GetUserBalanceQueryHandler.cs
using System.Threading.Tasks;
using Application.Features.Users.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;

namespace Application.Features.Users.Handlers.Queries;

public class GetUserBalanceQueryHandler
{
    private readonly IRepository<User> _userRepository;

    public GetUserBalanceQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<int> Handle(GetUserBalanceQuery query)
    {
        var user = await _userRepository.GetEntityById(query.UserId);
        return user.Balance;
    }
}