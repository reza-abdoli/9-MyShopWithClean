// src/SmsService/FileSmsSender.cs
namespace SmsService;
public class FileSmsSender : ISmsSender
{
    private readonly ILogger<FileSmsSender> _logger;
    private readonly string _filePath;

    public FileSmsSender(ILogger<FileSmsSender> logger)
    {
        _logger = logger;
        _filePath = Path.Combine(AppContext.BaseDirectory, "sms-outbox.log");
    }

    public async Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        await Task.Delay(300, cancellationToken); // شبیه‌سازی تاخیر شبکه

        var line = $"[{phoneNumber}] [{message}] [{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC]{Environment.NewLine}";
        await File.AppendAllTextAsync(_filePath, line, cancellationToken);

        _logger.LogInformation("Simulated SMS sent to {PhoneNumber}", phoneNumber);
    }
}