namespace NKF.Persistence.Base;

public interface IQueryPersistenceOperation<in TRequest, TResult> : IPersistenceOperation<TRequest, TResult>
{
}

public interface IQueryPersistenceOperation<TResult> : IPersistenceOperation<TResult>
{
}
