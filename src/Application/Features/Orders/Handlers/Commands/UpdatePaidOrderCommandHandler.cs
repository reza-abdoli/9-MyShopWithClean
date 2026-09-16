using System;
using System.Threading.Tasks;
using Application.Features.Orders.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Order;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Handlers.Commands;

public class UpdatePaidOrderCommandHandler
{
    private readonly IRepository<Domain.Entities.Order.Order> _orderRepository;
    private readonly IRepository<PaymentTransaction> _paymentTransactionRepository;
    private readonly OrderHelper _orderHelper;
    private readonly ILogger<UpdatePaidOrderCommandHandler> _logger;

    public UpdatePaidOrderCommandHandler(
        IRepository<Domain.Entities.Order.Order> orderRepository,
        IRepository<PaymentTransaction> paymentTransactionRepository,
        OrderHelper orderHelper,
        ILogger<UpdatePaidOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _paymentTransactionRepository = paymentTransactionRepository;
        _orderHelper = orderHelper;
        _logger = logger;
    }

    public async Task Handle(UpdatePaidOrderCommand command)
    {
        var order = await _orderHelper.GetOrderWithDetailsOrThrow(
            command.OrderId,
            command.UserId);

        var paymentTransaction =
            await _paymentTransactionRepository.GetEntityById(command.PaymentTransactionId);

        if (paymentTransaction == null)
            throw new InvalidOperationException(
                $"Payment transaction with ID {command.PaymentTransactionId} was not found.");

        if (paymentTransaction.UserId != command.UserId)
            throw new InvalidOperationException(
                "Payment transaction does not belong to the current user.");

        if (paymentTransaction.OrderId != null &&
            paymentTransaction.OrderId != order.Id)
        {
            throw new InvalidOperationException(
                $"Payment transaction {paymentTransaction.Id} is already associated with another order.");
        }

        var now = DateTime.UtcNow;

        foreach (var item in order.OrderDetails)
        {
            _orderHelper.ApplyCurrentPriceToOrderDetail(item);

            await _orderHelper.RecordProductSell(
                item,
                order.Id,
                now);

            await _orderHelper.DeductStockWithRetry(item);
        }

        await _orderHelper.ApplyShippingInfoFromUser(
            order,
            command.UserId);

        _orderHelper.FinalizeOrderAsPaid(
            order,
            paymentTransaction,
            now);

        paymentTransaction.OrderId = order.Id;

        _paymentTransactionRepository.UpdateEntity(paymentTransaction);
        _orderRepository.UpdateEntity(order);

        await _orderRepository.SaveChanges();

        _logger.LogInformation(
            "Order marked as paid. OrderId: {OrderId}, UserId: {UserId}, PaymentTransactionId: {PaymentTransactionId}",
            command.OrderId,
            command.UserId,
            command.PaymentTransactionId);
    }
}
