// Application/Features/Notifications/Requests/Commands/SendSmsCommand.cs
namespace Application.Features.Notifications.Requests.Commands;

public class SendSmsCommand
{
    public string PhoneNumber { get; set; }
    public string Message { get; set; }
}