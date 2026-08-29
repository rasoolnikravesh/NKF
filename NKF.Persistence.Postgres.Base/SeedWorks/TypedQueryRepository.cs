using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NKF.Domain.SeedWorks;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public class TypedQueryRepository<TEntity, TId> : ITypedQueryRepository<TEntity, TId>
    where TEntity : class, ITypedAggregateRoot<TId>
    where TId : EntityId
{
    public DbContext Context { get; }

    public DbSet<TEntity> Collection { get; }

    protected TenantContext? TenantContext;

    public TypedQueryRepository(DbContext context, TenantContext? tenantContext = null)
    {
        Context = context;
        TenantContext = tenantContext;
        Collection = context.Set<TEntity>(typeof(TEntity).Name);
    }

    private void PopulateTenantId(TEntity entity)
    {
        if (TenantContext == null || TenantContext.TenantId == Guid.Empty)
        {
            return;
        }

        if (entity is ITenantAware e)
        {
            e.AssignTenantId(TenantContext.TenantId);
        }
    }

    private void PopulateTenantId(IEnumerable<TEntity> entities)
    {
        if (TenantContext == null || TenantContext.TenantId == Guid.Empty)
        {
            return;
        }

        foreach (TEntity e in entities)
        {
            if (e is ITenantAware ie)
            {
                ie.AssignTenantId(TenantContext.TenantId);
            }
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<TEntity> result = await Collection.ToListAsync(cancellationToken);
        PopulateTenantId(result);
        return result;
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 10, IDictionary<string, string> orders = default!, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasAnyByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TEntity>> GetSomeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        List<TEntity> result = await Collection.Where(predicate).ToListAsync(cancellationToken);
        PopulateTenantId(result);
        return result;
    }

    public async Task<IEnumerable<TEntity>> GetSomeAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> include, CancellationToken cancellationToken = default)
    {
        List<TEntity> lists = await Collection.Where(predicate).Include(include).ToListAsync(cancellationToken);
        PopulateTenantId(lists);
        return lists;
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        TEntity? result = await Collection.Where(x => x.Id == id).SingleOrDefaultAsync(cancellationToken);
        if (result != null)
        {
            PopulateTenantId(result);
        }

        return result;
    }

    public Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken, params IList<Expression<Func<TEntity, object>>> predicates)
    {
        throw new NotImplementedException();
    }
}
