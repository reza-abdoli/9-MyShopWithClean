// Application/DTOs/Order/PaidOrderDto.cs
using System;
using System.Collections.Generic;
using Domain.Entities.Order;

namespace Application.DTOs.Order
{
    public class PaidOrderDto
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? DestinationCity { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public int TotalPrice { get; set; }
        public string? Description { get; set; }
        public string? PostTraceCode { get; set; }
        public string? BankTraceCode { get; set; }
        public DateTime? PaymentDate { get; set; }
        public OrderState OrderState { get; set; }
        public List<OrderItemDto> OrderDetails { get; set; } = new();
    }
}