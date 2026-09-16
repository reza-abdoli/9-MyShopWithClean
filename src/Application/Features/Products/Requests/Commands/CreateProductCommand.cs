// Application/Features/Products/Requests/Commands/CreateProductCommand.cs
using Application.DTOs.Product;

namespace Application.Features.Products.Requests.Commands;

public class CreateProductCommand
{
    public CreateProductDto CreateProductDto { get; set; }
}