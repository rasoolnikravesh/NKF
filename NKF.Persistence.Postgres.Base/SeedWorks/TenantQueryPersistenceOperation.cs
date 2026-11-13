using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public abstract class TenantQueryPersistenceOperation<TContext, TRequest, TResult>(
    ITenantDbContextSession<TContext> contextSession) : IQueryPersistenceOperation<TRequest, TResult>
    where TContext : DbContext
{
    protected async Task<TContext> GetContextAsync(CancellationToken cancellationToken = default)
    {
        return await contextSession.GetCurrentAsync(cancellationToken);
    }

    public abstract Task<TResult> Execute(TRequest request, CancellationToken cancellationToken = default);
}
