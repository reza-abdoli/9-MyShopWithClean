using Microsoft.AspNetCore.Mvc;

namespace UI.Areas.Admin.Controllers;

public class HomeController : AdminBaseController
{
    public IActionResult Index() => View();
}