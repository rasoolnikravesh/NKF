using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NKF.Domain.SeedWorks;
using NKF.Persistence.Base;
using NKF.Persistence.Base.RepositoryCollection;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public class QueryUnitOfWork(DbContext context, IRepositoryCollection repositoryCollection, TenantContext? tenantContext = null)
    : FUnitOfWork(repositoryCollection), IQueryUnitOfWork
{
    private DbContext Context { get; } = context;
    private TenantContext? TenantContext { get; } = tenantContext;

    public IQueryRepository<T> GetQueryRepository<T>() where T : class, IAggregateRoot
    {
        return DescriptorImplementationInstance<T>(typeof(T));
    }


    public IQueryRepository<T> GetQueryRepository<T>(T obj) where T : class, IAggregateRoot
    {
        return DescriptorImplementationInstance<T>(obj.GetType());
    }

    private IQueryRepository<T> DescriptorImplementationInstance<T>(Type type) where T : class, IAggregateRoot
    {
        RepositoryDescriptor? descriptor = RepositoryCollection
            .FirstOrDefault(z => z.ImplementationType
                .GetInterfaces().Any(x => x.GenericTypeArguments.Contains(type)));
        if (descriptor == null)
        {
            throw new InvalidOperationException($"No repository found for type {typeof(T)}.");
        }

        if (descriptor.ImplementationInstance is IQueryRepository<T> existingRepo)
        {
            return existingRepo;
        }

        object newInstance = CreateRepositoryInstance(descriptor.ImplementationType);
        if (newInstance is not IQueryRepository<T> repo)
        {
            throw new InvalidOperationException(
                $"Created instance of type {descriptor.ImplementationType.Name} does not implement IQueryRepository<{typeof(T).Name}>.");
        }

        RepositoryCollection.Remove(descriptor);
        // افزودن descriptor جدید با نمونه ساخته‌شده
        RepositoryDescriptor newDescriptor = new(descriptor.RepositoryType, repo);
        RepositoryCollection.Add(newDescriptor);
        return repo;
    }

    private object CreateRepositoryInstance(Type implementationType)
    {
        Type contextType = Context.GetType();

        // ابتدا سازنده با (DbContext, TenantContext) را امتحان کن
        ConstructorInfo? ctorWithTenant = implementationType.GetConstructor(new[] { contextType, typeof(TenantContext) });
        if (ctorWithTenant != null)
        {
            ParameterExpression ctxParam = Expression.Parameter(typeof(object), "context");
            ParameterExpression tenantParam = Expression.Parameter(typeof(object), "tenantContext");
            UnaryExpression convertedCtx = Expression.Convert(ctxParam, contextType);
            UnaryExpression convertedTenant = Expression.Convert(tenantParam, typeof(TenantContext));
            NewExpression newExpr = Expression.New(ctorWithTenant, convertedCtx, convertedTenant);
            UnaryExpression convertedResult = Expression.Convert(newExpr, typeof(object));
            Expression<Func<object, object, object>> lambda = Expression.Lambda<Func<object, object, object>>(convertedResult, ctxParam, tenantParam);
            Func<object, object, object> factory = lambda.Compile();
            return factory(Context, TenantContext!);
        }

        ConstructorInfo? constructor = implementationType.GetConstructor(new[] { contextType });
        if (constructor == null)
        {
            throw new InvalidOperationException(
                $"Type {implementationType.Name} does not have a constructor accepting {contextType.Name}.");
        }

        ParameterExpression param = Expression.Parameter(typeof(object), "context");
        UnaryExpression convertedParam = Expression.Convert(param, contextType);
        NewExpression newExpr2 = Expression.New(constructor, convertedParam);
        UnaryExpression convertedResult2 = Expression.Convert(newExpr2, typeof(object));
        Expression<Func<object, object>> lambda2 = Expression.Lambda<Func<object, object>>(convertedResult2, param);
        Func<object, object> factory2 = lambda2.Compile();

        object instance = factory2(Context);
        // اگر ریپازیتوری base دارای TenantContext به صورت property/field باشد با reflection ست کن (برای backward compatibility)
        if (TenantContext != null)
        {
            FieldInfo? field = implementationType.GetField("_tenantContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
            // تلاش برای ست کردن فیلد TenantContext در base class
            Type? baseType = implementationType.BaseType;
            while (baseType != null && field == null)
            {
                field = baseType.GetField("TenantContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (field == null)
                {
                    field = baseType.GetField("_tenantContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                }

                baseType = baseType.BaseType;
            }
            if (field != null)
            {
                // نمونه ساخته شده را اگر قبلاً TenantContext نداشت، ست کن
                try { field.SetValue(instance, TenantContext); } catch { }
            }
        }

        return instance;
    }
}
