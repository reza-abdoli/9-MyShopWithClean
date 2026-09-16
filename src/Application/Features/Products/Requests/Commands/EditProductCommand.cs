// Application/Features/Products/Requests/Commands/EditProductCommand.cs
using Application.DTOs.Product;

namespace Application.Features.Products.Requests.Commands;

public class EditProductCommand
{
    public EditProductDto EditProductDto { get; set; }
}