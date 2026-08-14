using System.Linq.Expressions;
using NKF.Domain.SeedWorks;

namespace NKF.Persistence.Base;

public interface IQueryRepository<TEntity> : IRepository where TEntity : IAggregateRoot
{
    Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default(CancellationToken));

    Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 10,
        IDictionary<string, string> orders = default,
        CancellationToken cancellationToken = default(CancellationToken));

    Task<bool> HasAnyByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

    Task<IEnumerable<TEntity>> GetSomeAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default(CancellationToken));
    Task<IEnumerable<TEntity>> GetSomeAsync(
        Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity,object>> include,
        CancellationToken cancellationToken = default(CancellationToken));

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

    Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken,
        params IList<Expression<Func<TEntity, object>>> predicates);
}