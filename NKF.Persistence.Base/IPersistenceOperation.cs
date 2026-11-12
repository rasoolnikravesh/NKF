namespace NKF.Persistence.Base;

public interface IPersistenceOperation<in TRequest, TResult>
{
    Task<TResult> Execute(TRequest request, CancellationToken cancellationToken = default);
}
