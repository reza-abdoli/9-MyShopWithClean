using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Product;

namespace Application.DTOs.Product
{
    public class FilterSizeDto
    {
        public string Title { get; set; }
        public string SizeType { get; set; }
        public List<ProductSize> Data { get; set; }
    }
}