using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.FileStorage;

public interface IImageStorageService
{
    Task<ImageSaveResult> SaveImageAsync(IFormFile image, ImageCategory category, int? width = null, int? height = null, bool hasThumbnail = true, string? oldFileNameToDelete = null);
    void DeleteImage(string fileName, ImageCategory category);
}

public class ImageSaveResult
{
    public bool Success { get; set; }
    public string? FileName { get; set; }
}