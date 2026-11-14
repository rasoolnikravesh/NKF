namespace NKF.Persistence.Base;

public interface IPersistenceOperation<in TRequest, TResult>
{
    Task<TResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}

public interface IPersistenceOperation<TResult>
{
    Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
}
