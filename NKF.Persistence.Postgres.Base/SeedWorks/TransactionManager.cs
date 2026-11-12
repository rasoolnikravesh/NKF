using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public sealed class TransactionManager(DbContext context) : ITransactionManager
{
    private readonly DbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);
        return new Transaction(transaction);
    }
}
