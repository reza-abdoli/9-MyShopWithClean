// Api/Common/IJwtTokenGenerator.cs (اصلاح‌شده)
using Application.Features.Users.Responses;

namespace Api.Common;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserProfileDto user);
}