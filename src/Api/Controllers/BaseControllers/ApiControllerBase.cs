// Api/Controllers/BaseControllers/ApiControllerBase.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers.BaseControllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private IMessageBus? _bus;
    protected IMessageBus Bus => _bus ??= HttpContext.RequestServices.GetRequiredService<IMessageBus>();

    protected long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}