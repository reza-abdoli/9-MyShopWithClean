// Application/Features/User/Handlers/Queries/IsProfileDetailCompletedQueryHandler.cs
using System.Threading.Tasks;
using Application.Features.Users.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;

namespace Application.Features.Users.Handlers.Queries;

public class IsProfileDetailCompletedQueryHandler
{
    private readonly IRepository<User> _userRepository;

    public IsProfileDetailCompletedQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(IsProfileDetailCompletedQuery query)
    {
        var user = await _userRepository.GetEntityById(query.UserId);
        if (user.MobileNumber == null || user.FullName == null || user.Address == null ||
            user.PostCode == null || user.UserCity == null)
            return false;
        return true;
    }
}