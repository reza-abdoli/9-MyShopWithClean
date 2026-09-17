// UI/Areas/Admin/ViewComponents/SelectedCategoriesViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Services.Base;

namespace UI.Areas.Admin.ViewComponents;

public class SelectedCategoriesViewComponent : ViewComponent
{
    private readonly IClient _client;

    public SelectedCategoriesViewComponent(IClient client)
    {
        _client = client;
    }

    public async Task<IViewComponentResult> InvokeAsync(long? thisId, List<long>? selectedCategories, int level = 0)
    {
        List<ProductCategoryDto> children;
        try
        {
            children = (await _client.ChildrenAsync(thisId)).ToList();
        }
        catch (ApiException)
        {
            children = new List<ProductCategoryDto>();
        }

        var model = new TreeViewCategoriesDto
        {
            Level = level,
            productCategories = children,
            SelectedCategoriesIds = selectedCategories
        };
        return View("SelectedCategories", model);
    }
}