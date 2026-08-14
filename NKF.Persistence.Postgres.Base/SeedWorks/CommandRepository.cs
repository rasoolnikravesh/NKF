using System.Linq.Expressions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using NKF.Domain.SeedWorks;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public class CommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : class, IAggregateRoot
{
    protected DbContext Context { get; }
    protected DbSet<TEntity> Collection { get; private set; }

    public CommandRepository(DbContext context)
    {
        Context =
            context ??
            throw new ArgumentNullException(paramName: nameof(context));

        Collection = context.Set<TEntity>();
    }

    public virtual async Task<Result> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        try
        {
            await Collection.AddAsync(entity, cancellationToken);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> AttachAsync(TEntity entity, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            Context.Attach(entity);

            return Result.Ok();
        });
    }

    public async Task<Result> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await Collection.AddRangeAsync(entities, cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return await RemoveByIdAsync(entity.Id, cancellationToken);
    }

    public async Task<Result> RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        int result = await Collection.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken: cancellationToken);
        return result == 1 ? Result.Ok() : Result.Fail("");
    }

    public async Task<Result> RemoveRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await Collection.Where(x => entities.Any(y => y.Id == x.Id)).ExecuteDeleteAsync(cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Run(() => { Collection.Update(entity); }, cancellationToken);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }


    public async Task<Result> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var aggregateRoot in entities)
            {
                await this.UpdateAsync(aggregateRoot, cancellationToken);
            }

            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
}