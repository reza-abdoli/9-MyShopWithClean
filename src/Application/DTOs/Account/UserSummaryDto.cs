// Application/DTOs/Users/UserSummaryDto.cs
namespace Application.DTOs.Users;

public class UserSummaryDto
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string? FullName { get; set; }
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
}