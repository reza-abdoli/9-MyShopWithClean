using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Order;
using Domain.Entities.Product;

namespace Domain.Entities.Order
{
    public class OrderDetail : BaseEntity
    {
        public long OrderId { get; set; }
        public long ProductVariantId { get; set; }
        public int ProductPrice { get; set; }
        public int TotalPrice { get; set; }
        public int Count { get; set; }
        public Order Order { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
