using System.Linq.Expressions;
using NKF.Domain.SeedWorks;

namespace NKF.Persistence.Base;

public interface ITypedQueryRepository<TEntity, TId> : IRepository
    where TEntity : class, ITypedAggregateRoot<TId>
    where TId : EntityId
{
    Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 10, IDictionary<string, string> orders = default!, CancellationToken cancellationToken = default);

    Task<bool> HasAnyByIdAsync(TId id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetSomeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetSomeAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> include, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken, params IList<Expression<Func<TEntity, object>>> predicates);
}
