using Microsoft.EntityFrameworkCore;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public interface ITenantDbContextSession<TContext>
    where TContext : DbContext
{
    ValueTask<TContext> GetCurrentAsync(CancellationToken cancellationToken = default);

    ValueTask<TContext> GetForTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAndFlushMessagesAsync(CancellationToken cancellationToken = default);

    Task PublishAsync(object message);
}
