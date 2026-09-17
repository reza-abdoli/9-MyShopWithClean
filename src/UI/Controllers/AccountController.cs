using Microsoft.AspNetCore.Mvc;
using UI.Interfaces;
using UI.Services.Base;

namespace UI.Controllers;

public class AccountController : Controller
{
    private readonly IAuthenticateService _authenticateService;

    public AccountController(IAuthenticateService authenticateService)
    {
        _authenticateService = authenticateService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(RegisterOrLoginDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _authenticateService.Login(dto.Username, dto.Password);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
            return View(dto);
        }

        if (result.Role == "Admin")
            return RedirectToAction("Index", "Home", new { area = "Admin" });

        return RedirectToAction("Index", "Home", new { area = "" });
    }

    [HttpGet]
    public IActionResult Signup() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Signup(RegisterOrLoginDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _authenticateService.Signup(dto.Username, dto.Password);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
            return View(dto);
        }

        TempData["SuccessMessage"] = "ثبت‌نام با موفقیت انجام شد. اکنون می‌توانید وارد حساب کاربری خود شوید.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _authenticateService.Logout();
        return RedirectToAction("Index", "Home", new { area = "" });
    }
}