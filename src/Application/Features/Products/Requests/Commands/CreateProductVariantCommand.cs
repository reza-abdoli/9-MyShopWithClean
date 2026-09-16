// Application/Features/Products/Requests/Commands/CreateProductVariantCommand.cs
using Application.DTOs.Product;

namespace Application.Features.Products.Requests.Commands;

public class CreateProductVariantCommand
{
    public CreateProductVariantDto CreateProductVariantDto { get; set; }
}