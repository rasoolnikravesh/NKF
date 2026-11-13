using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public sealed class TenantTransactionManager<TContext>(ITenantDbContextSession<TContext> contextSession)
    : ITransactionManager
    where TContext : DbContext
{
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        TContext context = await contextSession.GetCurrentAsync(cancellationToken);
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
            await context.Database.BeginTransactionAsync(cancellationToken);
        return new Transaction(transaction);
    }
}
