// Application/Features/User/Handlers/Commands/EditUserDetailCommandHandler.cs
using Application.Features.Users.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Handlers.Commands;

public class EditUserDetailCommandHandler
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<EditUserDetailCommandHandler> _logger;

    public EditUserDetailCommandHandler(IRepository<User> userRepository, ILogger<EditUserDetailCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(EditUserDetailCommand command)
    {
        var dto = command.EditUserInfoDto;
        var user = await _userRepository.GetEntityById(dto.UserId);

        var oldValues = $"FullName: {user.FullName}, Email: {user.Email}, Mobile: {user.MobileNumber}, Address: {user.Address}, PostCode: {user.PostCode}, City: {user.UserCity}";

        user.MobileNumber = dto.MobileNumber;
        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.Address = dto.Address;
        user.PostCode = dto.PostCode;
        user.UserCity = dto.UserCity;
        _userRepository.UpdateEntity(user);
        await _userRepository.SaveChanges();

        var newValues = $"FullName: {user.FullName}, Email: {user.Email}, Mobile: {user.MobileNumber}, Address: {user.Address}, PostCode: {user.PostCode}, City: {user.UserCity}";

        _logger.LogInformation(
            "User profile updated. UserId: {UserId}. Old values -> {OldValues}. New values -> {NewValues}",
            dto.UserId, oldValues, newValues);
    }
}