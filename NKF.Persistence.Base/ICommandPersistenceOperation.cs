namespace NKF.Persistence.Base;

public interface ICommandPersistenceOperation<in TRequest, TResult> : IPersistenceOperation<TRequest, TResult>
{
}

public interface ICommandPersistenceOperation<TResult> : IPersistenceOperation<TResult>
{
}
