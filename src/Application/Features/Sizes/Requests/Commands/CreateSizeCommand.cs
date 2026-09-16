// Application/Features/Sizes/Requests/Commands/CreateSizeCommand.cs
using Application.DTOs;
using Application.DTOs.Product;

namespace Application.Features.Sizes.Requests.Commands;

public class CreateSizeCommand
{
    public CreateSizeDto CreateSizeDto { get; set; }
}