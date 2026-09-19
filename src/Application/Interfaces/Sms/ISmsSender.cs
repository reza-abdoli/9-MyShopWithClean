// Application/Interfaces/Sms/ISmsSender.cs
namespace Application.Interfaces.Sms;

public interface ISmsSender
{
    Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}