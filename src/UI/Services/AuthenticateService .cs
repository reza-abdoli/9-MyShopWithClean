// UI/Services/AuthenticateService.cs 
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using UI.Interfaces;
using UI.Models;
using UI.Services.Base;

namespace UI.Services;

public class AuthenticateService : IAuthenticateService
{
    private readonly IClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticateService(IClient client, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _client = client;
    }

    public async Task<RegistrationResult> Login(string username, string password)
    {
        try
        {
            var result = await _client.LoginAsync(new RegisterOrLoginDto
            {
                Username = username,
                Password = password
            });


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.Username),
                new Claim(ClaimTypes.Role, result.Role),
                new Claim("jwt", result.Token) // <-- توکن همینجا، داخل خودِ Cookie

            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(120)
            };

            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return new RegistrationResult { Success = true, Role = result.Role };
        }
        catch (ApiException ex) when (ex.StatusCode == 401)
        {
            return new RegistrationResult { Success = false, Errors = new List<string> { "نام کاربری یا رمز عبور اشتباه است." } };
        }
        catch (ApiException)
        {
            return new RegistrationResult { Success = false, Errors = new List<string> { "خطایی در سرور رخ داد. لطفاً بعداً تلاش کنید." } };
        }
    }

    public async Task<RegistrationResult> Signup(string username, string password)
    {
        try
        {
            await _client.SignupAsync(new RegisterOrLoginDto { Username = username, Password = password });
            return new RegistrationResult { Success = true };
        }
        catch (ApiException ex) when (ex.StatusCode == 400)
        {
            return new RegistrationResult
            {
                Success = false,
                Errors = new List<string> { "این نام کاربری قبلاً ثبت شده است." }
            };
        }
        catch (ApiException)
        {
            return new RegistrationResult { Success = false, Errors = new List<string> { "خطایی در سرور رخ داد. لطفاً بعداً تلاش کنید." } };
        }
    }

    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}