// Application/Features/Categories/Requests/Commands/AddProductSelectedCategoriesCommand.cs
using System.Collections.Generic;

namespace Application.Features.Categories.Requests.Commands;

public class AddProductSelectedCategoriesCommand
{
    public List<long> SelectedCategoryIds { get; set; }
    public long ProductId { get; set; }
}