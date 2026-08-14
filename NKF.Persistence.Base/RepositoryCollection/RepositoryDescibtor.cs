namespace NKF.Persistence.Base.RepositoryCollection;

public class RepositoryDescriptor : IDisposable,ICloneable
{

	public RepositoryDescriptor(Type? repositoryType, Type? implementationType)

	{
		RepositoryType = repositoryType ?? throw new ArgumentNullException(nameof(repositoryType));
		ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
	}

	public RepositoryDescriptor(Type repositoryType, object instance)
		: this(repositoryType, instance.GetType())
	{
		if (repositoryType == null)
			throw new ArgumentNullException(nameof(repositoryType));
		ImplementationInstance = instance != null ? instance : throw new ArgumentNullException(nameof(instance));
	}

	public RepositoryDescriptor(
		Type repositoryType,
		Func<IServiceProvider, object>? factory)
		: this(repositoryType, factory?.Target!.GetType())
	{
        ArgumentNullException.ThrowIfNull(repositoryType);
        ImplementationFactory = factory ?? throw new ArgumentNullException(nameof(factory));
	}
	//private RepositoryDescriptor(Type repositoryType, Type implementationType)
	//{
	//	RepositoryType = repositoryType;
	//	ImplementationType = implementationType;
	//}

	public Type RepositoryType { get; }

	public Type ImplementationType { get; }

	public object? ImplementationInstance { get; }

	public Func<IServiceProvider, object>? ImplementationFactory { get; }

	internal Type GetImplementationType()
	{
		return ImplementationInstance != null ? ImplementationInstance.GetType() : ImplementationType;
	}

	public virtual void Dispose(bool disposing)
	{
		if (IsDisposed)
			return;
		if (disposing)
		{
			(ImplementationInstance as IDisposable)?.Dispose();
		}
		IsDisposed = true;
	}

	public bool IsDisposed { get; protected set; }



	~RepositoryDescriptor()
	{

		Dispose(false);
	}

	public object Clone()
	{
		return new RepositoryDescriptor(RepositoryType,ImplementationType);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

}