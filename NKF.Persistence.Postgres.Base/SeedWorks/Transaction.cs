using Microsoft.EntityFrameworkCore.Storage;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

internal class Transaction(IDbContextTransaction contextTransaction) : ITransaction
{
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await contextTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        await contextTransaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        contextTransaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await contextTransaction.DisposeAsync();
    }
}
