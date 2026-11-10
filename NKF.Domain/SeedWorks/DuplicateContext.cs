namespace NKF.Domain.SeedWorks;

public sealed class DuplicateContext
{
    private readonly Dictionary<object, object> _map = new();
    private readonly Dictionary<Guid, Guid> _idMap = new();

    public T GetOrAdd<T>(object source, Func<T> factory) where T : notnull
    {
        if (_map.TryGetValue(source, out object? existing))
        {
            return (T)existing;
        }

        T? created = factory();
        _map[source] = created;

        return created;
    }

    public void MapId(Guid sourceId, Guid targetId)
    {
        if (sourceId == Guid.Empty || targetId == Guid.Empty)
        {
            return;
        }

        _idMap[sourceId] = targetId;
    }

    public void MapId(EntityId sourceId, EntityId targetId)
    {
        if (sourceId == null || targetId == null)
        {
            return;
        }

        if (sourceId.IsEmpty() || targetId.IsEmpty())
        {
            return;
        }

        _idMap[sourceId.Value] = targetId.Value;
    }

    public Guid? GetMappedId(Guid? sourceId)
    {
        if (!sourceId.HasValue)
        {
            return null;
        }

        return _idMap.TryGetValue(sourceId.Value, out Guid targetId)
            ? targetId
            : sourceId;
    }

    public TId? GetMappedId<TId>(TId? sourceId) where TId : EntityId
    {
        if (sourceId == null)
        {
            return null;
        }

        if (_idMap.TryGetValue(sourceId.Value, out Guid targetGuid))
        {
            System.Reflection.MethodInfo? method = typeof(TId).GetMethod(
                "Create",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                null,
                new Type[] { typeof(Guid) },
                null);

            if (method != null)
            {
                object? result = method.Invoke(null, new object?[] { targetGuid });
                if (result is FluentResults.Result<TId> typedResult && typedResult.IsSuccess)
                {
                    return typedResult.Value;
                }
            }

            System.Reflection.MethodInfo? createNullable = typeof(TId).GetMethod(
                "Create",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                null,
                new Type[] { typeof(Guid?) },
                null);

            if (createNullable != null)
            {
                object? result = createNullable.Invoke(null, new object?[] { targetGuid });
                if (result is FluentResults.Result<TId> typedResult2 && typedResult2.IsSuccess)
                {
                    return typedResult2.Value;
                }
            }

            return sourceId;
        }

        return sourceId;
    }
}
