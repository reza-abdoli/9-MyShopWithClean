using Microsoft.AspNetCore.Mvc;

namespace UI.Areas.User.Controllers;

public class HomeController : UserBaseController
{
    public IActionResult Index() => View();
}