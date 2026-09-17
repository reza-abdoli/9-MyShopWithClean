using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Account
{
    public class DepositBalanceDto
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public int Amount { get; set; }
    }
}