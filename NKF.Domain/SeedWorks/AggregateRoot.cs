using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using FluentResults;
using NKF.Domain.Events;
using NKF.Domain.SharedKernels.ValueObjects;

namespace NKF.Domain.SeedWorks;

public abstract class AggregateRoot<T> : Entity<T>, IAggregateRoot where T : Entity<T>
{
    private static readonly ConcurrentDictionary<(Type aggregateType, Type eventType), Func<object, object, object>>
        ApplyFuncS = new();

    protected AggregateRoot(Guid createdByUserId) : base(createdByUserId)
    {
        _domainEvents = [];
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected AggregateRoot() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }


    protected async Task<Result<T>> ApplyEventAsync<T>(IDomainEvent @event,
        CancellationToken cancellationToken = default) where T : Entity<T>
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
                MethodInfo? method = aggregateType.GetMethod(
                    "Apply",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new[] { eventType },
                    null);

                if (method == null)
                {
                    throw new InvalidOperationException(
                        $"No Apply method found for event type '{eventType.Name}' in aggregate" +
                        $" '{aggregateType.Name}'.");
                }

                // پارامترهای Expression: handler (object) و event (object)
                ParameterExpression handlerParam = Expression.Parameter(typeof(object), "handler");
                ParameterExpression eventParam = Expression.Parameter(typeof(object), "event");

                // فراخوانی متد: ((aggregateType)handler).Apply((eventType)event)
                MethodCallExpression callExpr = Expression.Call(
                    Expression.TypeAs(handlerParam, aggregateType),
                    method,
                    Expression.TypeAs(eventParam, eventType));

                // ساخت Func<object, object, object> (ورودی‌ها object، خروجی object)
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

    [JsonIgnore] private readonly List<IDomainEvent> _domainEvents;
    [JsonIgnore] public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;


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
