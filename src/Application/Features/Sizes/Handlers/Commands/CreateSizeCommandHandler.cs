// Application/Features/Sizes/Handlers/Commands/CreateSizeCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Sizes.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Product;
using Microsoft.Extensions.Logging;

namespace Application.Features.Sizes.Handlers.Commands;

public class CreateSizeCommandHandler
{
    private readonly IRepository<ProductSize> _productSizeRepository;
    private readonly ILogger<CreateSizeCommandHandler> _logger;

    public CreateSizeCommandHandler(IRepository<ProductSize> productSizeRepository, ILogger<CreateSizeCommandHandler> logger)
    {
        _productSizeRepository = productSizeRepository;
        _logger = logger;
    }

    public async Task Handle(CreateSizeCommand command)
    {
        var dto = command.CreateSizeDto;
        var newSize = new ProductSize
        {
            Title = dto.Title,
            SizeType = dto.SizeType,
            Order = dto.Order
        };
        await _productSizeRepository.AddEntity(newSize);
        await _productSizeRepository.SaveChanges();

        _logger.LogInformation("Product size created. SizeId: {SizeId}, Title: {Title}", newSize.Id, newSize.Title);
    }
}