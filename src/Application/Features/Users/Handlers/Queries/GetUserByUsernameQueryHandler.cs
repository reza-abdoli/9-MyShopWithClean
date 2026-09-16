// Application/Features/User/Handlers/Queries/GetUserByUsernameQueryHandler.cs
using System.Threading.Tasks;
using Application.Features.Users.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers.Queries;

public class GetUserByUsernameQueryHandler
{
    private readonly IRepository<User> _userRepository;

    public GetUserByUsernameQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Handle(GetUserByUsernameQuery query)
    {
        return await _userRepository.GetQuery().SingleOrDefaultAsync(u => u.Username == query.Username);
    }
}