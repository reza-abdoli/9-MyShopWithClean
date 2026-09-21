// src/SmsService/SendSmsCommandHandler.cs
using KafkaContracts.Notifications;

namespace SmsService;

public class SendSmsCommandHandler
{
    private readonly ISmsSender _smsSender;
    public SendSmsCommandHandler(ISmsSender smsSender) => _smsSender = smsSender;

    public Task Handle(SendSmsCommand command)
        => _smsSender.SendAsync(command.PhoneNumber, command.Message);
}