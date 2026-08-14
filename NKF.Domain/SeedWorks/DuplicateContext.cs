namespace NKF.Domain.SeedWorks;

public sealed class DuplicateContext
{
    private readonly Dictionary<object, object> _map = new();
    private readonly Dictionary<Guid, Guid> _idMap = new();

    public T GetOrAdd<T>(object source, Func<T> factory)
    {
        if (_map.TryGetValue(source, out var existing))
            return (T)existing;

        var created = factory();
        _map[source] = created;

        return created;
    }

    public void MapId(Guid sourceId, Guid targetId)
    {
        if (sourceId == Guid.Empty || targetId == Guid.Empty)
            return;

        _idMap[sourceId] = targetId;
    }

    public Guid? GetMappedId(Guid? sourceId)
    {
        if (!sourceId.HasValue)
            return null;

        return _idMap.TryGetValue(sourceId.Value, out var targetId)
            ? targetId
            : sourceId;
    }
}
