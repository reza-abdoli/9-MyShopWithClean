// Application/Features/User/Handlers/Queries/GetLoginQueryHandler.cs
using System.Threading.Tasks;
using Application.Features.Users.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Handlers.Queries;

public class GetLoginQueryHandler
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<GetLoginQueryHandler> _logger;

    public GetLoginQueryHandler(IRepository<User> userRepository, ILogger<GetLoginQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(GetLoginQuery query)
    {
        var dto = query.RegisterOrLoginDto;
        var user = await _userRepository.GetQuery().SingleOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            _logger.LogWarning("Failed login attempt. Username: {Username}", dto.Username);
            return false;
        }

        _logger.LogInformation("User logged in. UserId: {UserId}, Username: {Username}", user.Id, user.Username);
        return true;
    }
}