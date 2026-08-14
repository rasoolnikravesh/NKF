namespace NKF.Persistence.Base.RepositoryCollection.Generics;

public interface IRepositoryCollection<T> : IList<RepositoryDescriptor>,
	ICollection<RepositoryDescriptor>,
	IEnumerable<RepositoryDescriptor>,
	IDisposable where T : IRepository
{

}