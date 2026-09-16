// Application/Features/Brands/Requests/Commands/CreateBrandCommand.cs
using Application.DTOs;
using Application.DTOs.Product;

namespace Application.Features.Brands.Requests.Commands;

public class CreateBrandCommand
{
    public CreateBrandDto CreateBrandDto { get; set; }
}