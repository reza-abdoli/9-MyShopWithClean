// Application/DTOs/Order/OpenOrderDto.cs
using System.Collections.Generic;
using System.Linq;

namespace Application.DTOs.Order
{
    public class OpenOrderDto
    {
        public long OrderId { get; set; }
        public List<OrderItemDto> OrderDetails { get; set; } = new();

        public int TotalCartPrice() => OrderDetails.Sum(x => x.TotalPrice);
    }
}