using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Application.DTOs.Account
{
    public class UserDashboardDetailDto
    {
        public User User { get; set; }
        public List<Domain.Entities.Order.Order> PendingOrders { get; set; }
        public int SentOrderCount { get; set; }
        public int PendingOrderCount { get; set; }
        public int ReturnedOrderCount { get; set; }
        public int CanceledOrderCount { get; set; }
    }
}
