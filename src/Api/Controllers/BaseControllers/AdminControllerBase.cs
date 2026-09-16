// Api/Controllers/BaseControllers/AdminControllerBase.cs
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers.BaseControllers;

[Authorize(Roles = "Admin")]
public abstract class AdminControllerBase : ApiControllerBase
{
}