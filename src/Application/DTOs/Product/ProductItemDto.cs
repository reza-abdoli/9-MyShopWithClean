using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Product
{
    public class ProductItemDto
    {
        public long ProductId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }
    }
}