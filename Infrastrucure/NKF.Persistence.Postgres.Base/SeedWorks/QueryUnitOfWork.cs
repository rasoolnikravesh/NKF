using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NKF.Domain.SeedWorks;
using NKF.Persistence.Base;
using NKF.Persistence.Base.RepositoryCollection;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public class QueryUnitOfWork(DbContext context, IRepositoryCollection repositoryCollection)
    : FUnitOfWork(repositoryCollection), IQueryUnitOfWork
{
    private DbContext Context { get; } = context;

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
            throw new InvalidOperationException($"No repository found for type {typeof(T)}.");

        if (descriptor.ImplementationInstance is IQueryRepository<T> existingRepo)
            return existingRepo;

        object newInstance = CreateRepositoryInstance(descriptor.ImplementationType);
        if (newInstance is not IQueryRepository<T> repo)
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
        var contextType = Context.GetType(); // نوع واقعی Context
        var constructor = implementationType.GetConstructor(new[] { contextType });
        if (constructor == null)
            throw new InvalidOperationException(
                $"Type {implementationType.Name} does not have a constructor accepting {contextType.Name}.");

        // ساخت Expression برای فراخوانی سازنده
        var param = Expression.Parameter(typeof(object), "context");
        var convertedParam = Expression.Convert(param, contextType); // تبدیل object به contextType
        var newExpr = Expression.New(constructor, convertedParam);
        var convertedResult = Expression.Convert(newExpr, typeof(object)); // تبدیل نتیجه به object
        Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(convertedResult, param);
        Func<object, object> factory = lambda.Compile(); // ساخت delegate

        object instance = factory(Context);

        return instance;
    }
}