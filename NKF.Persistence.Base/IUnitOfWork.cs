using FluentResults;

namespace NKF.Persistence.Base;

public interface IUnitOfWork
{
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}
