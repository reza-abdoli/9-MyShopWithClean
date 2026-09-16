// Application/Features/Banners/Requests/Commands/CreateBannerCommand.cs
using Application.DTOs.Common;

namespace Application.Features.Banners.Requests.Commands;

public class CreateBannerCommand
{
    public CreateBannerDto CreateBannerDto { get; set; }
}