// Application/Features/Order/OrderHelper.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Persistence;
using Domain.Entities;
using Domain.Entities.Order;
using Domain.Entities.Product;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders;

public class OrderHelper
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderDetail> _orderDetailRepository;
    private readonly IRepository<ProductVariant> _variantRepository;
    private readonly IRepository<ProductSell> _productSellRepository;
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<OrderHelper> _logger;

    public OrderHelper(
        IRepository<Domain.Entities.Order.Order> orderRepository,
        IRepository<OrderDetail> orderDetailRepository,
        IRepository<ProductVariant> variantRepository,
        IRepository<ProductSell> productSellRepository,
        IRepository<User> userRepository,
        ILogger<OrderHelper> logger)
    {
        _orderRepository = orderRepository;
        _orderDetailRepository = orderDetailRepository;
        _variantRepository = variantRepository;
        _productSellRepository = productSellRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<bool> CheckOrderDetailExists(long userId, long productVariantId)
    {
        return await _orderRepository.GetQuery()
            .Where(o => o.UserId == userId && o.OrderState == OrderState.Open)
            .Include(o => o.OrderDetails)
            .AnyAsync(o => o.OrderDetails.Any(od => od.ProductVariantId == productVariantId));
    }

    public async Task<long> GetOrCreateOrderForUser(long userId)
    {
        var order = await _orderRepository.GetQuery()
            .Where(o => o.UserId == userId && o.OrderState == OrderState.Open)
            .FirstOrDefaultAsync();

        if (order != null) return order.Id;

        var newOrder = new Domain.Entities.Order.Order
        {
            UserId = userId,
            TotalPrice = 0,
            OrderState = OrderState.Open
        };
        await _orderRepository.AddEntity(newOrder);
        await _orderRepository.SaveChanges();

        _logger.LogInformation("New open order created. OrderId: {OrderId}, UserId: {UserId}", newOrder.Id, userId);
        return newOrder.Id;
    }

    public async Task RemoveStaleOrderDetails(long orderId)
    {
        var staleDetails = await _orderDetailRepository.GetQuery()
            .Include(d => d.ProductVariant)
            .Where(d => d.OrderId == orderId && d.ProductVariant.StockCount <= 0)
            .ToListAsync();

        if (staleDetails.Count == 0) return;

        foreach (var detail in staleDetails)
            _orderDetailRepository.DeleteEntityPermanently(detail);

        await _orderDetailRepository.SaveChanges();

        _logger.LogInformation(
            "Removed {Count} stale (out-of-stock) order detail(s) from OrderId: {OrderId}. OrderDetailIds: {OrderDetailIds}",
            staleDetails.Count, orderId, string.Join(",", staleDetails.Select(d => d.Id)));
    }

    public async Task<Domain.Entities.Order.Order> GetOrderWithDetailsOrThrow(long orderId, long userId)
    {
        var order = await _orderRepository.GetQuery()
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.ProductVariant)
                    .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} for User {userId} was not found.");

        if (order.OrderState != OrderState.Open)
            throw new InvalidOperationException($"Order {orderId} is already processed (State: {order.OrderState}).");

        return order;
    }

    public void ApplyCurrentPriceToOrderDetail(OrderDetail item)
    {
        var currentProductPrice = item.ProductVariant.Product.Price;
        item.ProductPrice = currentProductPrice;
        item.TotalPrice = currentProductPrice * item.Count;
        _orderDetailRepository.UpdateEntity(item);
    }

    public async Task RecordProductSell(OrderDetail item, long orderId, DateTime saleDate)
    {
        await _productSellRepository.AddEntity(new ProductSell
        {
            ProductId = item.ProductVariant.ProductId,
            OrderId = orderId,
            SellCount = item.Count,
            SellDate = saleDate,
            ProductPrice = item.ProductPrice
        });
    }

    public async Task DeductStockWithRetry(OrderDetail item, int maxRetries = 5)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            var variant = await _variantRepository.GetEntityById(item.ProductVariantId);
            if (variant == null)
                throw new InvalidOperationException($"Product variant {item.ProductVariantId} was not found.");

            if (variant.StockCount < item.Count)
                throw new InvalidOperationException($"Insufficient stock for product '{item.ProductVariant.Product.Title}'.");

            variant.StockCount -= item.Count;
            _variantRepository.UpdateEntity(variant);

            try
            {
                await _variantRepository.SaveChanges();
                return;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                    entry.State = EntityState.Detached;

                _logger.LogWarning(
                    "Concurrency conflict on stock deduction, retrying. ProductVariantId: {ProductVariantId}, Attempt: {Attempt}",
                    item.ProductVariantId, attempt);
            }
        }

        throw new InvalidOperationException(
            $"Could not deduct stock for variant {item.ProductVariantId} after {maxRetries} attempts due to concurrent updates.");
    }

    public async Task ApplyShippingInfoFromUser(Domain.Entities.Order.Order order, long userId)
    {
        var user = await _userRepository.GetEntityById(userId);
        if (user == null) return;

        order.Address = user.Address;
        order.PostCode = user.PostCode;
        order.DestinationCity = user.UserCity;
        order.UserName = user.FullName;
    }

    public void FinalizeOrderAsPaid(
        Order order,
        PaymentTransaction paymentTransaction,
        DateTime paymentDate)
    {
        order.OrderState = OrderState.Paid;
        order.PaymentDate = paymentDate;
        order.PaymentTransaction = paymentTransaction;
        order.TransactionId = paymentTransaction.Id;
        order.TotalPrice = order.OrderDetails.Sum(
        item => item.ProductPrice * item.Count);
    }

}