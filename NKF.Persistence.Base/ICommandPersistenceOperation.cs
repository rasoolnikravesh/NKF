namespace NKF.Persistence.Base;

public interface ICommandPersistenceOperation<in TRequest, TResult> : IPersistenceOperation<TRequest, TResult>
{
}
