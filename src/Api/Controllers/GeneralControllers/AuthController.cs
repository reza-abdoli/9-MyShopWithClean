// Api/Controllers/GeneralControllers/AuthController.cs
using Api.Common;
using Api.Controllers.BaseControllers;
using Application.DTOs.Account;
using Application.Features.Users.Requests.Commands;
using Application.Features.Users.Requests.Queries;
using Application.Features.Users.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.GeneralControllers;

[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthController(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> Signup(RegisterOrLoginDto dto)
    {
        var existingUser = await Bus.InvokeAsync<UserProfileDto?>(new GetUserProfileQuery { Username = dto.Username });
        if (existingUser != null)
            return BadRequest(new { message = "این نام کاربری قبلاً ثبت شده است." });

        await Bus.InvokeAsync(new CreateUserCommand { RegisterOrLoginDto = dto });
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(RegisterOrLoginDto dto)
    {
        var isValid = await Bus.InvokeAsync<bool>(new GetLoginQuery { RegisterOrLoginDto = dto });
        if (!isValid)
            return Unauthorized(new { message = "نام کاربری یا رمز عبور وارد شده صحیح نیست." });

        var user = await Bus.InvokeAsync<UserProfileDto?>(new GetUserProfileQuery { Username = dto.Username });
        var token = _jwtTokenGenerator.GenerateToken(user!);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = user!.Username,
            Role = user.Role
        });
    }
}