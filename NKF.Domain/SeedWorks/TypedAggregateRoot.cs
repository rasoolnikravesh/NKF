using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using FluentResults;
using NKF.Domain.Events;
using NKF.Domain.SharedKernels.ValueObjects;

namespace NKF.Domain.SeedWorks;

public abstract class TypedAggregateRoot<T, TId> : TypedEntity<T, TId>, ITypedAggregateRoot<TId>
    where T : TypedEntity<T, TId>
    where TId : EntityId
{
    private static readonly ConcurrentDictionary<(Type aggregateType, Type eventType), Func<object, object, object>> ApplyFuncS = new();

    protected TypedAggregateRoot(TId id, Guid createdByUserId) : base(id, createdByUserId)
    {
        _domainEvents = [];
    }

#pragma warning disable CS8618
    protected TypedAggregateRoot() : base()
#pragma warning restore CS8618
    {
        _domainEvents = [];
    }

    protected async Task<Result<T>> ApplyEventAsync<TAggregate>(IDomainEvent @event, CancellationToken cancellationToken = default)
        where TAggregate : TypedEntity<T, TId>
    {
        return await Task.Run<Result<T>>(() =>
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            Type aggregateType = GetType();
            Type eventType = @event.GetType();
            (Type aggregateType, Type eventType) key = (aggregateType, eventType);
            Func<object, object, object> func = ApplyFuncS.GetOrAdd(key, k =>
            {
                System.Reflection.MethodInfo? method = aggregateType.GetMethod(
                    "Apply",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new Type[] { eventType },
                    null);

                if (method == null)
                {
                    throw new InvalidOperationException(
                        $"No Apply method found for event type '{eventType.Name}' in aggregate '{aggregateType.Name}'.");
                }

                ParameterExpression handlerParam = Expression.Parameter(typeof(object), "handler");
                ParameterExpression eventParam = Expression.Parameter(typeof(object), "event");

                MethodCallExpression callExpr = Expression.Call(
                    Expression.TypeAs(handlerParam, aggregateType),
                    method,
                    Expression.TypeAs(eventParam, eventType));

                Expression<Func<object, object, object>> lambda =
                    Expression.Lambda<Func<object, object, object>>(callExpr, handlerParam, eventParam);
                return lambda.Compile();
            });

            Result<T>? met = (func(this, @event) as Result<T>)!;
            RaiseDomainEvent(@event);
            return met;
        }, cancellationToken);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    [JsonIgnore]
    private readonly List<IDomainEvent> _domainEvents;

    [JsonIgnore]
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    protected void RaiseDomainEvent(IDomainEvent? domainEvent)
    {
        if (domainEvent is not null)
        {
            _domainEvents.Add(domainEvent);
        }
    }

    protected void RemoveDomainEvent(IDomainEvent? domainEvent)
    {
        if (domainEvent is not null)
        {
            _domainEvents.Remove(domainEvent);
        }
    }

    protected void Update_UpdatedDate()
    {
        UpdateDate update = UpdateDate.CreateUtcNow().Value;
        UpdatedDateTime = update;
    }
}
