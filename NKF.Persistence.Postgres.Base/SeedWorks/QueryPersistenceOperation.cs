using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public abstract class QueryPersistenceOperation<TContext, TRequest, TResult>(TContext context)
    : IQueryPersistenceOperation<TRequest, TResult>
    where TContext : DbContext
{
    protected TContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));

    public abstract Task<TResult> Execute(TRequest request, CancellationToken cancellationToken = default);
}
