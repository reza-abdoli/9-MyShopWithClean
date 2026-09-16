// Application/Features/Colors/Requests/Commands/CreateColorCommand.cs
using Application.DTOs;
using Application.DTOs.Product;

namespace Application.Features.Colors.Requests.Commands;

public class CreateColorCommand
{
    public CreateColorDTo CreateColorDto { get; set; }
}