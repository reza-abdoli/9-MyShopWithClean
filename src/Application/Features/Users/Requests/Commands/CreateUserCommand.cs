// Application/Features/User/Requests/Commands/CreateUserCommand.cs
using Application.DTOs.Account;

namespace Application.Features.Users.Requests.Commands;

public class CreateUserCommand
{
    public RegisterOrLoginDto RegisterOrLoginDto { get; set; }
}