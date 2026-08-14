using NKF.Domain.SeedWorks;

namespace NKF.Persistence.Base;

public interface IQueryUnitOfWork : IFUnitOfWork
{
	IQueryRepository<T> GetQueryRepository<T>() where T : class, IAggregateRoot;

	IQueryRepository<T> GetQueryRepository<T>(T obj) where T : class, IAggregateRoot;

}