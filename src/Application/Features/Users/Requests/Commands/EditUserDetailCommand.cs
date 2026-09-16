// Application/Features/User/Requests/Commands/EditUserDetailCommand.cs
using Application.DTOs.Account;

namespace Application.Features.Users.Requests.Commands;

public class EditUserDetailCommand
{
    public EditUserInfoDTO EditUserInfoDto { get; set; }
}