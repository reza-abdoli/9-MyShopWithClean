// Application/DTOs/Order/OrderDetailDto.cs
using System;
using System.Collections.Generic;
using Application.DTOs.Users;
using Application.Features.Payments.Responses;
using Domain.Entities.Order;

namespace Application.DTOs.Order
{
    public class OrderDetailDto
    {
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string DestinationCity { get; set; }
        public int TotalPrice { get; set; }
        public string? Description { get; set; }
        public string? PostTraceCode { get; set; }
        public string BankTraceCode { get; set; }
        public PaymentTransactionDto? PaymentTransaction { get; set; }
        public OrderState OrderState { get; set; }
        public UserSummaryDto User { get; set; }
        public List<OrderItemDto> OrderDetails { get; set; }
    }
}