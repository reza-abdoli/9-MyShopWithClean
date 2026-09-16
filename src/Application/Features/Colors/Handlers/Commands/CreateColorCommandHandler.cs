// Application/Features/Colors/Handlers/Commands/CreateColorCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Features.Colors.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.Extensions.Logging;

namespace Application.Features.Colors.Handlers.Commands;

public class CreateColorCommandHandler
{
    private readonly IRepository<ProductColor> _productColorRepository;
    private readonly ILogger<CreateColorCommandHandler> _logger;

    public CreateColorCommandHandler(IRepository<ProductColor> productColorRepository, ILogger<CreateColorCommandHandler> logger)
    {
        _productColorRepository = productColorRepository;
        _logger = logger;
    }

    public async Task Handle(CreateColorCommand command)
    {
        var dto = command.CreateColorDto;
        var newColor = new ProductColor
        {
            Title = dto.Title,
            ColorCode = dto.ColorCode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _productColorRepository.AddEntity(newColor);
        await _productColorRepository.SaveChanges();

        _logger.LogInformation("Product color created. ColorId: {ColorId}, Title: {Title}", newColor.Id, newColor.Title);
    }
}