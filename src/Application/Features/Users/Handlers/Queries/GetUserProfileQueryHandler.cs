// Application/Features/Users/Handlers/Queries/GetUserProfileQueryHandler.cs
using System.Threading.Tasks;
using Application.Features.Users.Requests.Queries;
using Application.Features.Users.Responses;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers.Queries;

public class GetUserProfileQueryHandler
{
    private readonly IRepository<User> _userRepository;

    public GetUserProfileQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileQuery query)
    {
        var user = await _userRepository.GetQuery().SingleOrDefaultAsync(u => u.Username == query.Username);
        if (user == null) return null;

        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
    }
}