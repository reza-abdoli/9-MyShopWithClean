using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Common;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Common
{
    public class CreateBannerDto
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public IFormFile ImageFile { get; set; }
        public int Order { get; set; }
        public BannerLocation BannerLocation { get; set; }
    }
}
