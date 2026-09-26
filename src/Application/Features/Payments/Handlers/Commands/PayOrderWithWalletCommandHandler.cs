// Application/Features/Payments/Handlers/Commands/PayOrderWithWalletCommandHandler.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using KafkaContracts.Notifications;
using Application.Features.Orders;
using Application.Features.Payments.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Handlers.Commands;

public class PayOrderWithWalletCommandHandler
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;
    private readonly IRepository<PaymentTransaction> _paymentRepository;
    private readonly IOrderHelper _orderHelper;
    private readonly IOutboxUnitOfWork _outbox;
    private readonly ILogger<PayOrderWithWalletCommandHandler> _logger;

    public PayOrderWithWalletCommandHandler(
        IRepository<User> userRepository,
        IRepository<Domain.Entities.Order.Order> orderRepository,
        IRepository<PaymentTransaction> paymentRepository,
        IOrderHelper orderHelper,
        IOutboxUnitOfWork outbox,
        ILogger<PayOrderWithWalletCommandHandler> logger)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _orderHelper = orderHelper;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task<bool> Handle(PayOrderWithWalletCommand command)
    {
        try
        {
            var user = await _userRepository.GetEntityById(command.UserId);
            if (user == null)
            {
                _logger.LogWarning("Wallet payment: user {UserId} not found", command.UserId);
                return false;
            }

            var order = await _orderHelper.GetOrderWithDetailsOrThrow(command.OrderId, command.UserId);
            if (!order.OrderDetails.Any())
                return false;

            var now = DateTime.UtcNow;

            var total = order.OrderDetails.Sum(i => i.ProductVariant.Product.Price * i.Count);

            if (user.Balance < total)
            {
                _logger.LogWarning("Wallet payment rejected: insufficient balance. UserId: {UserId}", command.UserId);
                return false; 
            }

            var paymentTx = await _paymentRepository.GetQuery()
                .FirstOrDefaultAsync(t => t.OrderId == order.Id);
            var isNew = paymentTx == null;

            paymentTx ??= new PaymentTransaction
            {
                UserId = command.UserId,
                OrderId = order.Id,
                CreatedAt = now
            };
            paymentTx.Amount = total;
            paymentTx.Authority = $"WALLET_{Guid.NewGuid():N}";
            paymentTx.Status = PaymentStatus.Success;
            paymentTx.UpdatedAt = now;

            if (isNew) await _paymentRepository.AddEntity(paymentTx);
            else _paymentRepository.UpdateEntity(paymentTx);

            user.Balance -= total;
            _userRepository.UpdateEntity(user);

            await _orderHelper.ApplyPaymentAsync(order, paymentTx, now);
            _orderRepository.UpdateEntity(order);

            if (!string.IsNullOrWhiteSpace(user.MobileNumber))
            {
                await _outbox.PublishAsync(new SendSmsCommand
                {
                    PhoneNumber = user.MobileNumber,
                    Message = $"سفارش شما با موفقیت پرداخت شد. مبلغ: {total:N0} تومان"
                });
            }

            await _outbox.CommitAsync();

            _logger.LogInformation("Wallet payment succeeded. UserId: {UserId}, OrderId: {OrderId}",
                command.UserId, command.OrderId);
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict on wallet payment. UserId: {UserId}", command.UserId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wallet payment failed. UserId: {UserId}, OrderId: {OrderId}",
                command.UserId, command.OrderId);
            return false;
        }
    }
}