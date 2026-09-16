// UI/Areas/User/ViewComponents/GetLastUserDataViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using UI.Services.Base;

namespace UI.Areas.User.ViewComponents;

public class GetLastUserDataViewComponent : ViewComponent
{
    private readonly IClient _client;

    public GetLastUserDataViewComponent(IClient client)
    {
        _client = client;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        int balance;
        try
        {
            balance = await _client.BalanceAsync();
        }
        catch (ApiException)
        {
            balance = 0;
        }

        return View("Default", balance);
    }
}