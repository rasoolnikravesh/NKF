using FluentResults;
using NKF.Domain.SeedWorks;

namespace NKF.Persistence.Base;

public interface ITypedCommandRepository<TEntity, TId> : IRepository
    where TEntity : class, ITypedAggregateRoot<TId>
    where TId : EntityId
{
    Task<Result> AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task<Result> AttachAsync(TEntity entity, CancellationToken cancellationToken);

    Task<Result> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    Task<Result> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    Task<Result> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<Result> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<Result> RemoveByIdAsync(TId id, CancellationToken cancellationToken = default);
}
