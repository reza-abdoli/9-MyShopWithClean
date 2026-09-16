// Application/Features/User/Requests/Queries/GetLoginQuery.cs
using Application.DTOs.Account;

namespace Application.Features.Users.Requests.Queries;

public class GetLoginQuery
{
    public RegisterOrLoginDto RegisterOrLoginDto { get; set; }
}