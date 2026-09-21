// src/Contracts/Notifications/SendSmsCommand.cs
namespace KafkaContracts.Notifications;

public class SendSmsCommand
{
    public string PhoneNumber { get; set; } = "";
    public string Message { get; set; } = "";
}