using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Product;

public class CreateSizeDto
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string SizeType { get; set; }
    [Required]
    public int Order { get; set; }
}