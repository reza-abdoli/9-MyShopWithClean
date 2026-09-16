using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Product
{
    public class CreateVariantItemDto
    {
        public long ColorId { get; set; }
        // public int Price { get; set; }
        public long SizeId { get; set; }
        public int StockCount { get; set; }

    }
}