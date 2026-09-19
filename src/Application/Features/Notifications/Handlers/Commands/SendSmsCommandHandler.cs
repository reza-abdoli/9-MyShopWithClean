// Application/Features/Notifications/Handlers/Commands/SendSmsCommandHandler.cs
using Application.Features.Notifications.Requests.Commands;
using Application.Interfaces.Sms;

namespace Application.Features.Notifications.Handlers.Commands;

public class SendSmsCommandHandler
{
    private readonly ISmsSender _smsSender;
    public SendSmsCommandHandler(ISmsSender smsSender) => _smsSender = smsSender;

    public Task Handle(SendSmsCommand command)
        => _smsSender.SendAsync(command.PhoneNumber, command.Message);
}