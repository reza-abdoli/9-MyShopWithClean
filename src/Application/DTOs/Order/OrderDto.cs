using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Order;

namespace Application.DTOs.Order
{
public class OrderDto
{
    public long Id { get; set; }
    public string? DestinationCity { get; set; }
    public string? Address { get; set; }
    public string? PostCode { get; set; }
    public int TotalPrice { get; set; }
    public string? Description { get; set; }
    public string? PostTraceCode { get; set; }
    public DateTime? PaymentDate { get; set; }
    public OrderState OrderState { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}
}