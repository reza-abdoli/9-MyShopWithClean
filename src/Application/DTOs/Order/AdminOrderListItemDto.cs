// Application/DTOs/Order/AdminOrderListItemDto.cs
using System;
using System.Collections.Generic;
using Domain.Entities.Order;

namespace Application.DTOs.Order
{
    public class AdminOrderListItemDto
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? DestinationCity { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public int TotalPrice { get; set; }
        public string? Description { get; set; }
        public string? PostTraceCode { get; set; }
        public long? TransactionId { get; set; }
        public long? PaymentTransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public OrderState OrderState { get; set; }
        public List<OrderItemDto> OrderDetails { get; set; } = new();
    }
}