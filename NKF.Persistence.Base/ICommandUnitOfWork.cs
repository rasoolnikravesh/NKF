using NKF.Domain.SeedWorks;

namespace NKF.Persistence.Base;

public interface ICommandUnitOfWork : IFUnitOfWork
{
    ICommandRepository<T> GetCommandRepository<T>() where T : class, IAggregateRoot;

    Task<FluentResults.Result> SaveAsync(CancellationToken cancellationToken);
}