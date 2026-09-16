// UI/Models/CreateBannerViewModel.cs
using Microsoft.AspNetCore.Http;
using UI.Services.Base;

namespace UI.Models;

public class CreateBannerViewModel
{
    public string Title { get; set; }
    public string Url { get; set; }
    public BannerLocation BannerLocation { get; set; }
    public int Order { get; set; }
    public IFormFile ImageFile { get; set; }
}