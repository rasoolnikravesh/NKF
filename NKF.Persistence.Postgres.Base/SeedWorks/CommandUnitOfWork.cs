using System.Linq.Expressions;
using FluentResults;
using FluentResults.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NKF.Domain.SeedWorks;
using NKF.Persistence.Base;
using NKF.Persistence.Base.RepositoryCollection;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public class CommandUnitOfWork(DbContext context, IRepositoryCollection repositoryCollection, IMediator mediator, TenantContext? tenantContext = null)
    : FUnitOfWork(repositoryCollection), ICommandUnitOfWork
{
    private DbContext Context { get; } = context;
    private TenantContext? TenantContext { get; } = tenantContext;

    private IMediator Mediator { get; } = mediator;
    private IDbContextTransaction? ContextTransaction { get; set; }

    public ICommandRepository<T> GetCommandRepository<T>() where T : class, IAggregateRoot
        => DescriptorImplementationInstance<T>(typeof(T));


    public ICommandRepository<T> GetCommandRepository<T>(T obj) where T : class, IAggregateRoot
        => DescriptorImplementationInstance<T>(obj.GetType());


    private ICommandRepository<T> DescriptorImplementationInstance<T>(Type type) where T : class, IAggregateRoot
    {
        RepositoryDescriptor? descriptor = RepositoryCollection
            .FirstOrDefault(z => z.ImplementationType
                .GetInterfaces().Any(x => x.GenericTypeArguments.Contains(type)));
        if (descriptor == null)
            throw new InvalidOperationException($"No repository found for type {typeof(T)}.");

        if (descriptor.ImplementationInstance is ICommandRepository<T> existingRepo)
            return existingRepo;

        object newInstance = CreateRepositoryInstance(descriptor.ImplementationType);
        if (newInstance is not ICommandRepository<T> repo)
            throw new InvalidOperationException(
                $"Created instance of type {descriptor.ImplementationType.Name} does not implement IQueryRepository<{typeof(T).Name}>.");

        RepositoryCollection.Remove(descriptor);
        // افزودن descriptor جدید با نمونه ساخته‌شده
        var newDescriptor = new RepositoryDescriptor(descriptor.RepositoryType, repo);
        RepositoryCollection.Add(newDescriptor);
        return repo;
    }

    private object CreateRepositoryInstance(Type implementationType)
    {
        var contextType = Context.GetType();

        var ctorWithTenant = implementationType.GetConstructor(new[] { contextType, typeof(TenantContext) });
        if (ctorWithTenant != null)
        {
            var ctxParam = Expression.Parameter(typeof(object), "context");
            var tenantParam = Expression.Parameter(typeof(object), "tenantContext");
            var convertedCtx = Expression.Convert(ctxParam, contextType);
            var convertedTenant = Expression.Convert(tenantParam, typeof(TenantContext));
            var newExpr = Expression.New(ctorWithTenant, convertedCtx, convertedTenant);
            var convertedResult = Expression.Convert(newExpr, typeof(object));
            Expression<Func<object, object, object>> lambda = Expression.Lambda<Func<object, object, object>>(convertedResult, ctxParam, tenantParam);
            Func<object, object, object> factory = lambda.Compile();
            return factory(Context, TenantContext!);
        }

        var constructor = implementationType.GetConstructor(new[] { contextType });
        if (constructor == null)
            throw new InvalidOperationException(
                $"Type {implementationType.Name} does not have a constructor accepting {contextType.Name}.");

        var param = Expression.Parameter(typeof(object), "context");
        var convertedParam = Expression.Convert(param, contextType);
        var newExpr2 = Expression.New(constructor, convertedParam);
        var convertedResult2 = Expression.Convert(newExpr2, typeof(object));
        Expression<Func<object, object>> lambda2 = Expression.Lambda<Func<object, object>>(convertedResult2, param);
        Func<object, object> factory2 = lambda2.Compile();
        object instance = factory2(Context);

        if (TenantContext != null)
        {
            var baseType = implementationType.BaseType;
            System.Reflection.FieldInfo? field = null;
            while (baseType != null)
            {
                field = baseType.GetField("TenantContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (field != null) break;
                field = baseType.GetField("_tenantContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (field != null) break;
                baseType = baseType.BaseType;
            }
            if (field == null)
                field = implementationType.GetField("TenantContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (field != null)
            {
                try { field.SetValue(instance, TenantContext); } catch { }
            }
        }
        return instance;
    }

    public async Task<Result> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            int r = await Context.SaveChangesAsync(cancellationToken);
            return Result.OkIf(r != 0, "").OrFailIf(r == 0, "");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ITransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        ContextTransaction = await Context.Database.BeginTransactionAsync(cancellationToken);
        return new Transaction(ContextTransaction);
    }
}