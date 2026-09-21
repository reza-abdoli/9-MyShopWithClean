// src/SmsService/ISmsSender.cs
namespace SmsService;

public interface ISmsSender
{
    Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}