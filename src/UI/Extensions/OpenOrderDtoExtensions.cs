// UI/Extensions/OpenOrderDtoExtensions.cs
using UI.Services.Base;

namespace UI.Extensions;

public static class OpenOrderDtoExtensions
{
    public static int TotalCartPrice(this OpenOrderDto order)
    {
        if (order?.OrderDetails == null) return 0;
        return order.OrderDetails.Sum(d => d.ProductVariant.Product.Price * d.Count);
    }
}