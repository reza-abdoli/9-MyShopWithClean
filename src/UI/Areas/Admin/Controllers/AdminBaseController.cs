using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class AdminBaseController : Controller
{
    protected const string ErrorMessage = "ErrorMessage";
    protected const string SuccessMessage = "SuccessMessage";
    protected const string InfoMessage = "InfoMessage";
    protected const string WarningMessage = "WarningMessage";

    protected const string SuccessText = "عملیات با موفقیت انجام شد";
    protected const string ErrorText = "در انجام عملیات خطایی رخ داده است";
}