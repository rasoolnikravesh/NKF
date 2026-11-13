using FluentResults;
using NKF.Persistence.Base;
using Microsoft.EntityFrameworkCore;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public sealed class TenantSaveChangesOperation<TContext>(ITenantDbContextSession<TContext> contextSession)
    : ISaveChangesOperation
    where TContext : DbContext
{
    public async Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        await contextSession.SaveChangesAndFlushMessagesAsync(cancellationToken);
        return Result.Ok();
    }
}
