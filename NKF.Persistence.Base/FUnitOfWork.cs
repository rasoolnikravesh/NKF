using NKF.Persistence.Base.RepositoryCollection;

namespace NKF.Persistence.Base;

public class FUnitOfWork : IFUnitOfWork

{
    protected FUnitOfWork(IRepositoryCollection repositoryCollection)
    {
        RepositoryCollection = repositoryCollection;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            RepositoryCollection.Dispose();
        }

        IsDisposed = true;
    }

    public bool IsDisposed { get; private set; }

    public IRepositoryCollection RepositoryCollection { get; }


    ~FUnitOfWork()
    {

        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
