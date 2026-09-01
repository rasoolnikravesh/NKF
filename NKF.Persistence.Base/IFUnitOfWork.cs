namespace NKF.Persistence.Base;

public interface IFUnitOfWork : IDisposable
{
    bool IsDisposed { get; }
}
