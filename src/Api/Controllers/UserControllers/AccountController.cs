// Api/Controllers/UserControllers/AccountController.cs
using Api.Controllers.BaseControllers;
using Application.DTOs.Account;
using Application.Features.Users.Requests.Commands;
using Application.Features.Users.Requests.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.UserControllers;

[Authorize]
[Route("api/account")]
public class AccountController : ApiControllerBase
{
    [HttpGet("edit-detail")]
    public async Task<ActionResult<EditUserInfoDTO>> GetEditUserDetail()
    {
        var model = await Bus.InvokeAsync<EditUserInfoDTO>(new GetEditUserDetailQuery { UserId = UserId });
        return Ok(model);
    }

    [HttpPut("edit-detail")]
    public async Task<IActionResult> EditUserDetail(EditUserInfoDTO dto)
    {
        dto.UserId = UserId;
        await Bus.InvokeAsync(new EditUserDetailCommand { EditUserInfoDto = dto });
        return Ok();
    }

    [HttpGet("balance")]
    public async Task<ActionResult<int>> GetBalance()
    {
        var balance = await Bus.InvokeAsync<int>(new GetUserBalanceQuery { UserId = UserId });
        return Ok(balance);
    }

    [HttpGet("profile-completed")]
    public async Task<ActionResult<bool>> IsProfileCompleted()
    {
        var completed = await Bus.InvokeAsync<bool>(new IsProfileDetailCompletedQuery { UserId = UserId });
        return Ok(completed);
    }
}