// Application/Features/Products/Requests/Commands/CreateProductFeatureCommand.cs
using Application.DTOs.Product;

namespace Application.Features.Products.Requests.Commands;

public class CreateProductFeatureCommand
{
    public CreateProductFeatureDto CreateProductFeatureDto { get; set; }
}