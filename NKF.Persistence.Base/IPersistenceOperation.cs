namespace NKF.Persistence.Base;

public interface IPersistenceOperation<in TRequest, TResult>
{
    Task<TResult> Execute(TRequest request, CancellationToken cancellationToken = default);
}

public interface IPersistenceOperation<TResult>
{
    Task<TResult> Execute(CancellationToken cancellationToken = default);
}
