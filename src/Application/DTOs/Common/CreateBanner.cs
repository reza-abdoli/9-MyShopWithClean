using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Common;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Common
{
    public class CreateBannerDto
    {
        [Required]

        public string Title { get; set; }
        [Required]

        public string Url { get; set; }
        [Required]

        public IFormFile ImageFile { get; set; }
        [Required]

        public int Order { get; set; }
        [Required]

        public BannerLocation BannerLocation { get; set; }
    }
}
