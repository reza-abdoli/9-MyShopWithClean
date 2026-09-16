using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Order;
using Domain.Entities.Users;


namespace Application.DTOs.Order
{
    public class OpenOrderDto
    {
        public User User { get; set; }
        public Domain.Entities.Order.Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public int TotalCartPrice()
        {
            return OrderDetails.Select(item => item.ProductVariant.Product.Price * item.Count)
                .Aggregate(0, (current, price) => current + price);
        }
    }
}
