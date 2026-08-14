using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NKF.Domain.SeedWorks;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public class QueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : class, IAggregateRoot
{
    public DbContext Context { get; }

    public DbSet<TEntity> Collection { get; }

    public QueryRepository(DbContext context)
    {
        Context = context;
        Collection = context.Set<TEntity>(typeof(TEntity).Name);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<TEntity> result = await Collection.ToListAsync(cancellationToken);
        return result;
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 10,
        IDictionary<string, string> orders = default,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasAnyByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TEntity>> GetSomeAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        List<TEntity> result = await Collection.Where(predicate).ToListAsync(cancellationToken);
        return result;
    }

    public async Task<IEnumerable<TEntity>> GetSomeAsync(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, object>> include,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var lists = await Collection.Where(predicate)
            .Include(include)
            .ToListAsync(cancellationToken);
        return lists;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        TEntity? result = await Collection.Where(x => x.Id == id).SingleOrDefaultAsync(cancellationToken);
        return result;
    }

    public Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken,
        params IList<Expression<Func<TEntity, object>>> predicates)
    {
        throw new NotImplementedException();
    }
}