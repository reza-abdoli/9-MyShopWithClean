// Persistence/OutboxUnitOfWork.cs  (namespace رو مطابق پروژه‌ات نگه دار)
using Application.Interfaces.Persistence;
using Persistence;
using Wolverine.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class OutboxUnitOfWork : IOutboxUnitOfWork
{
    private readonly IDbContextOutbox<ApplicationDbContext> _outbox;

    public OutboxUnitOfWork(IDbContextOutbox<ApplicationDbContext> outbox) => _outbox = outbox;

    public async ValueTask PublishAsync<T>(T message) where T : notnull
        => await _outbox.PublishAsync(message);

    public Task CommitAsync(CancellationToken ct = default)
        => _outbox.SaveChangesAndFlushMessagesAsync(ct);
}