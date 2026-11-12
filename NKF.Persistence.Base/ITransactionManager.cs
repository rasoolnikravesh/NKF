namespace NKF.Persistence.Base;

public interface ITransactionManager
{
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
