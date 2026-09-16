// UI/Models/CreateBrandViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UI.Models;

public class CreateBrandViewModel
{
    [Required(ErrorMessage = "عنوان الزامی است")]
    [Display(Name = "عنوان")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "آدرس الزامی است")]
    [Display(Name = "آدرس (Url)")]
    public string Url { get; set; } = string.Empty;

    [Required(ErrorMessage = "تصویر الزامی است")]
    [Display(Name = "تصویر برند")]
    public IFormFile Image { get; set; }
    [Display(Name = "ترتیب نمایش")]
    public int Order { get; set; } = 1;
}