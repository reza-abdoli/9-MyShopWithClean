// Application/Features/User/Handlers/Commands/CreateUserCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Features.Users.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;
namespace Application.Features.Users.Handlers.Commands;

public class CreateUserCommandHandler
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(IRepository<User> userRepository, ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(CreateUserCommand command)
    {
        var dto = command.RegisterOrLoginDto;
        var user = new User
        {
            Username = dto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddEntity(user);
        await _userRepository.SaveChanges();
        _logger.LogInformation("New user created. UserId: {UserId}, Username: {Username}", user.Id, user.Username);
    }
}