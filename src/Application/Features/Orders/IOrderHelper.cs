using Application.Features.Orders;
using Domain.Entities.Order;

namespace Application.Features.Orders;

public interface IOrderHelper
{
    Task<bool> CheckOrderDetailExists(long userId, long productVariantId);
    Task<long> GetOrCreateOrderForUser(long userId);
    Task RemoveStaleOrderDetails(long orderId);
    Task<Domain.Entities.Order.Order> GetOrderWithDetailsOrThrow(long orderId, long userId);

    void ApplyCurrentPriceToOrderDetail(OrderDetail item);
    Task RecordProductSell(OrderDetail item, long orderId, DateTime saleDate);
    Task DeductStock(OrderDetail item);
    Task ApplyShippingInfoFromUser(Domain.Entities.Order.Order order, long userId);

    void FinalizeOrderAsPaid(Domain.Entities.Order.Order order, PaymentTransaction paymentTransaction, DateTime paymentDate, string referenceCode);
    Task ApplyPaymentAsync(Domain.Entities.Order.Order order, PaymentTransaction paymentTx, DateTime now, string referenceCode);

    void EnsurePaymentMatchesOrder(PaymentTransaction paymentTx, Domain.Entities.Order.Order order);
}