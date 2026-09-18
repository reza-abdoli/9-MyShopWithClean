using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Account
{
    public class GetEditUserInfoDto
    {
        public long UserId { get; set; }
        public string? MobileNumber { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public string? UserCity { get; set; }
    }
}