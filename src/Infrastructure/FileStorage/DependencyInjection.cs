// Infrastructure/FileStorage/DependencyInjection.cs
using Application.Interfaces.FileStorage;
using Infrastructure.FileStorage;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorage;

public static class DependencyInjection
{
    public static IServiceCollection AddFileStorageServices(this IServiceCollection services)
    {
        services.AddScoped<IImageStorageService, ImageStorageService>();
        return services;
    }
}