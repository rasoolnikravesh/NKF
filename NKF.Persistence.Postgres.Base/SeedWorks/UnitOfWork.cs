using FluentResults;
using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;
using Wolverine.EntityFrameworkCore;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IDbContextOutbox _outbox;

    public UnitOfWork(DbContext context, IDbContextOutbox outbox)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _outbox = outbox ?? throw new ArgumentNullException(nameof(outbox));
        _outbox.Enroll(_context);
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
        return Result.Ok();
    }
}
