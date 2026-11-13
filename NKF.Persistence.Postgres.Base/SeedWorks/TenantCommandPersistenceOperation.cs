using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public abstract class TenantCommandPersistenceOperation<TContext, TRequest, TResult>(
    ITenantDbContextSession<TContext> contextSession) : ICommandPersistenceOperation<TRequest, TResult>
    where TContext : DbContext
{
    protected async Task<TContext> GetContextAsync(CancellationToken cancellationToken = default)
    {
        return await contextSession.GetCurrentAsync(cancellationToken);
    }

    public abstract Task<TResult> Execute(TRequest request, CancellationToken cancellationToken = default);
}
