using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Users;

namespace Domain.Entities.Order
{
    public class Order : BaseEntity
    {
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? DestinationCity { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public int TotalPrice { get; set; }
        public string? Description { get; set; }
        public string? PostTraceCode { get; set; }
        public long? TransactionId { get; set; }

        // کد یکپارچه‌ی پیگیری: برای پرداخت آنلاین = Authority بانک، برای کیف‌پول = "WALLET-{Guid}"
        public string? TransactionCode { get; set; }

        public virtual PaymentTransaction? PaymentTransaction { get; set; }

        public DateTime? PaymentDate { get; set; }
        public OrderState OrderState { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public enum OrderState
    {
        [Display(Name = "ثبت شده")]
        Open,
        [Display(Name = "پرداخت شده")]
        Paid,
        [Display(Name = "در حال پردازش")]
        Pending,
        [Display(Name = "ارسال شده")]
        Sent,
        [Display(Name = "مرجوع شده")]
        Returned,
        [Display(Name = "لغو شده")]
        Canceled,
    }
}