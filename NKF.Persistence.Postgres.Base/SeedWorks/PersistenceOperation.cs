using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public abstract class PersistenceOperation<TRequest, TResult>(DbContext context)
    : IPersistenceOperation<TRequest, TResult>
{
    protected DbContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));

    public abstract Task<TResult> Execute(TRequest request, CancellationToken cancellationToken = default);
}
