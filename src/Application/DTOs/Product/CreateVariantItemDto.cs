using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Product
{
    public class CreateVariantItemDto
    {
        [Required]
        public long ColorId { get; set; }
        [Required]
        public long SizeId { get; set; }
        [Required]
        public int StockCount { get; set; }

    }
}