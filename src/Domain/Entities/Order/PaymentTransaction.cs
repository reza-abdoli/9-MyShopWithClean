using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Users;

namespace Domain.Entities.Order
{
    public class PaymentTransaction : BaseEntity
    {
        public long UserId { get; set; }

        public virtual User User { get; set; }

        public long? OrderId { get; set; } // Nullable if payment is for wallet deposit

        public virtual Order? Order { get; set; }

        public int Amount { get; set; }
        public string Authority { get; set; } = string.Empty; 
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3
    }
}