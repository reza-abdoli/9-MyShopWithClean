// Application/Features/Categories/Requests/Commands/CreateCategoryCommand.cs
using Application.DTOs.Category;

namespace Application.Features.Categories.Requests.Commands;

public class CreateCategoryCommand
{
    public CreateCategoryDto CreateCategoryDto { get; set; }
}