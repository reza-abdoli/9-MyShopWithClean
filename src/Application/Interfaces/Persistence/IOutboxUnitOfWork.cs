// Application/Interfaces/Persistence/IOutboxUnitOfWork.cs
namespace Application.Interfaces.Persistence;

public interface IOutboxUnitOfWork
{
    ValueTask PublishAsync<T>(T message) where T : notnull;

    /// تنها نقطه‌ی ذخیره: تغییرات دیتابیس + پیام‌های Outbox با هم
    Task CommitAsync(CancellationToken ct = default);
}