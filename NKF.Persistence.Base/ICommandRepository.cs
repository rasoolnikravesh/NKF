using System.Linq.Expressions;
using FluentResults;
using NKF.Domain.SeedWorks;

namespace NKF.Persistence.Base;

public interface ICommandRepository<TEntity> : IRepository where TEntity : IAggregateRoot
{
    Task<Result> AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task<Result> AttachAsync(TEntity entity, CancellationToken cancellationToken);

    Task<Result> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    Task<Result> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);


    Task<Result> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

    Task<Result> RemoveRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default(CancellationToken));

    Task<Result> RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
}
