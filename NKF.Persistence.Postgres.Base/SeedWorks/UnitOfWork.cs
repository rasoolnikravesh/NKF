using FluentResults;
using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public sealed class UnitOfWork(DbContext context) : IUnitOfWork
{
    private readonly DbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
