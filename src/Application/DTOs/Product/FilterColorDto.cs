using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Product;

namespace Application.DTOs.Product;

public class FilterColorDto
{
    public string Title { get; set; }
    public List<ProductColor> Data { get; set; }
}