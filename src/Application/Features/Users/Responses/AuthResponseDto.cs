// Application/Features/Users/Responses/AuthResponseDto.cs
namespace Application.Features.Users.Responses;

public class AuthResponseDto
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
}