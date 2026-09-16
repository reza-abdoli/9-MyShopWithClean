// Application/Features/Orders/Handlers/Commands/AddProductToOrderCommandHandler.cs
using System.Threading.Tasks;
using Application.Features.Orders.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Handlers.Commands;

public class AddProductToOrderCommandHandler
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ProductVariant> _variantRepository;
    private readonly IRepository<OrderDetail> _orderDetailRepository;
    private readonly OrderHelper _orderHelper;
    private readonly ILogger<AddProductToOrderCommandHandler> _logger;

    public AddProductToOrderCommandHandler(
        IRepository<Product> productRepository,
        IRepository<ProductVariant> variantRepository,
        IRepository<OrderDetail> orderDetailRepository,
        OrderHelper orderHelper,
        ILogger<AddProductToOrderCommandHandler> logger)
    {
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _orderDetailRepository = orderDetailRepository;
        _orderHelper = orderHelper;
        _logger = logger;
    }

    public async Task<long> Handle(AddProductToOrderCommand command)
    {
        var dto = command.SubmitOrderDetailDto;
        var orderId = await _orderHelper.GetOrCreateOrderForUser(dto.UserId);

        var variantCount = await _variantRepository.GetQuery()
            .Where(v => v.Id == dto.ProductVariantId)
            .Select(v => v.StockCount)
            .SingleOrDefaultAsync();

        if (!(variantCount > 0))
        {
            _logger.LogWarning("Add to order failed: no stock. UserId: {UserId}, ProductVariantId: {ProductVariantId}", dto.UserId, dto.ProductVariantId);
            return 0;
        }

        if (await _orderHelper.CheckOrderDetailExists(dto.UserId, dto.ProductVariantId))
        {
            var orderDetail = await _orderDetailRepository.GetQuery()
                .Where(o => o.OrderId == orderId && o.ProductVariantId == dto.ProductVariantId)
                .SingleOrDefaultAsync();

            orderDetail.Count = 1;
            _orderDetailRepository.UpdateEntity(orderDetail);
            await _orderDetailRepository.SaveChanges();

            _logger.LogInformation("Order detail updated. OrderDetailId: {OrderDetailId}, OrderId: {OrderId}, ProductVariantId: {ProductVariantId}", orderDetail.Id, orderId, dto.ProductVariantId);
            return orderDetail.Id;
        }
        else
        {
            var product = await _productRepository.GetEntityById(dto.ProductId);
            var newOrderDetail = new OrderDetail
            {
                OrderId = orderId,
                ProductVariantId = dto.ProductVariantId,
                ProductPrice = product.Price,
                Count = 1,
                TotalPrice = product.Price
            };
            await _orderDetailRepository.AddEntity(newOrderDetail);
            await _orderDetailRepository.SaveChanges();

            _logger.LogInformation("Product added to order. OrderDetailId: {OrderDetailId}, OrderId: {OrderId}, ProductId: {ProductId}, ProductVariantId: {ProductVariantId}", newOrderDetail.Id, orderId, dto.ProductId, dto.ProductVariantId);
            return newOrderDetail.Id;
        }
    }
}