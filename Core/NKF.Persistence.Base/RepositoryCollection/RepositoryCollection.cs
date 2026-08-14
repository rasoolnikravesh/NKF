using System.Collections;

namespace NKF.Persistence.Base.RepositoryCollection;

public sealed class RepositoryCollection : IRepositoryCollection
{
    #region Collection

    private readonly List<RepositoryDescriptor> _descriptors = [];

    public int Count => _descriptors.Count;

    public bool IsReadOnly => false;

    public RepositoryDescriptor this[int index]
    {
        get => _descriptors[index];
        set => _descriptors[index] = value;
    }

    public void Clear() => _descriptors.Clear();

    public bool Contains(RepositoryDescriptor item) => _descriptors.Contains(item);

    public void CopyTo(RepositoryDescriptor[] array, int arrayIndex) => _descriptors.CopyTo(array, arrayIndex);

    public bool Remove(RepositoryDescriptor item) => _descriptors.Remove(item);

    public IEnumerator<RepositoryDescriptor> GetEnumerator() => _descriptors.GetEnumerator();

    void ICollection<RepositoryDescriptor>.Add(RepositoryDescriptor item) => _descriptors.Add(item);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public int IndexOf(RepositoryDescriptor item) => _descriptors.IndexOf(item);

    public void Insert(int index, RepositoryDescriptor item) => _descriptors.Insert(index, item);

    public void RemoveAt(int index) => _descriptors.RemoveAt(index);

    #endregion

    private void Dispose(bool disposing)
    {
        if (IsDisposed)
            return;
        if (disposing)
        {
            foreach (IDisposable obj in _descriptors.OfType<IDisposable>())
            {
                obj.Dispose();
            }

            _descriptors.Clear();
        }

        IsDisposed = true;
    }

    private bool IsDisposed { get; set; }


    ~RepositoryCollection()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    public object ShallowCopy()
    {
        return MemberwiseClone();
    }

    // method for cloning object
    public RepositoryCollection DeepCopy()
    {
        var deepCopyCompany = new RepositoryCollection();
        foreach (var descriptor in _descriptors)
        {
            var cloned = descriptor.Clone() as RepositoryDescriptor;
            deepCopyCompany.Add(cloned!.RepositoryType, cloned.ImplementationType!);
        }

        return deepCopyCompany;
    }
}