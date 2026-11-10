using System.Reflection;

namespace NKF.Persistence.Base.RepositoryCollection;

public static class RepositoryCollectionExtensions
{


    public static IRepositoryCollection AddFromAssembly(
        this IRepositoryCollection collection,
        Type repositoryType,
        Type impType)
    {

        string baseType = repositoryType.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.GetTypeInfo()
            .ImplementedInterfaces.Any(y => y == typeof(IRepository)))!.Name;

        IEnumerable<Type> types = Assembly.GetAssembly(repositoryType)!.GetTypes().Where(x => x.GetInterfaces().Any(y => y.Name == baseType) && x.IsInterface);

        //var types = Assembly.GetAssembly(ImpType)!.GetTypes().Where(x => x.GetInterfaces()
        //	.Any(y => y.Name == baseType) && x.IsInterface);

        foreach (Type type in types)
        {
            Type? imp = Assembly.GetAssembly(impType)!.GetTypes().FirstOrDefault(x => x.GetInterfaces()
                .Any(y => y.Name == type.Name));
            if (imp != null)
            {
                collection.Add(type, imp);
            }
        }

        return collection;

    }
    public static IRepositoryCollection Add(
        this IRepositoryCollection collection,
        Type serviceType,
        Type implementationType)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (serviceType == null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (implementationType == null)
        {
            throw new ArgumentNullException(nameof(implementationType));
        }

        return AddToCollection(collection, serviceType, implementationType);
    }

    public static IRepositoryCollection Add(
        this IRepositoryCollection collection,
        Type serviceType,
        Func<IServiceProvider, object> implementationFactory)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (serviceType == null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (implementationFactory == null)
        {
            throw new ArgumentNullException(nameof(implementationFactory));
        }

        return AddToCollection(collection, serviceType, implementationFactory);
    }

    public static IRepositoryCollection Add<TRepository, TImplementation>(
        this IRepositoryCollection collection)
        where TRepository : class
        where TImplementation : class, TRepository
    {
        return collection != null ? Add(collection, typeof(TRepository), typeof(TImplementation)) :
            throw new ArgumentNullException(nameof(collection));
    }

    public static IRepositoryCollection Add<TRepository, TImplementation>(
        this IRepositoryCollection collection,
        Func<IServiceProvider, TImplementation> implementationFactory)
        where TRepository : class
        where TImplementation : class, TRepository
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        return implementationFactory != null ? Add(collection, typeof(TRepository), implementationFactory) : throw new ArgumentNullException(nameof(implementationFactory));
    }

    public static IRepositoryCollection Add(
        this IRepositoryCollection collection,
        Type serviceType,
        object implementationInstance)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (serviceType == null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        RepositoryDescriptor repositoryDescriptor = implementationInstance != null ?
            new RepositoryDescriptor(serviceType, implementationInstance) :
            throw new ArgumentNullException(nameof(implementationInstance));
        collection.Add(repositoryDescriptor);
        return collection;
    }

    private static IRepositoryCollection AddToCollection(
        IRepositoryCollection collection,
        Type repositoryType,
        Type implementationType)
    {
        RepositoryDescriptor repositoryDescriptor = new(repositoryType, implementationType);
        collection.Add(repositoryDescriptor);
        return collection;
    }

    private static IRepositoryCollection AddToCollection(
        IRepositoryCollection collection,
        Type repositoryType,
        Func<IServiceProvider, object> implementationFactory)
    {
        RepositoryDescriptor repositoryDescriptor = new(repositoryType, implementationFactory);
        collection.Add(repositoryDescriptor);
        return collection;
    }
}
