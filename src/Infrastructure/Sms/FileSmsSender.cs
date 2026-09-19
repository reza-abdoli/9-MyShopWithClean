

using Application.Interfaces.Sms;
using Microsoft.Extensions.Logging;

namespace Infrastructure.SmsSms
{
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
            // شبیه‌سازی تاخیر شبکه‌ی یه سرویس واقعی پیامک
            await Task.Delay(300, cancellationToken);

            // throw new Exception("TEST SMS FAILURE");
            var line = $"[{phoneNumber}] [{message}] [{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC]{Environment.NewLine}";
            await File.AppendAllTextAsync(_filePath, line, cancellationToken);

            _logger.LogInformation("Simulated SMS sent to {PhoneNumber}", phoneNumber);
        }
    }

}