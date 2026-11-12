using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using NKF.Domain.SharedKernels.ValueObjects;

namespace NKF.Domain.SeedWorks;

public abstract class Entity<T, TId> : IEntity<TId>, IDeepDuplicatable<T>, IAuditable, ITenantAware
    where T : Entity<T, TId>
    where TId : EntityId
{
    public TId Id { get; protected set; } = default!;

    Guid IEntity.Id => Id.Value;

    public Guid? CreatedByUserId { get; protected set; }

    public CreateDate CreatedDateTime { get; protected set; } = default!;

    public Guid? UpdatedByUserId { get; protected set; }

    public UpdateDate UpdatedDateTime { get; protected set; } = default!;

    public Guid? OwnedByUserId { get; protected set; }

    public bool IsDeleted { get; protected set; }

    public Guid? DeletedByUserId { get; protected set; }

    [NotMapped]
    public Guid TenantId { get; protected set; }

    protected Entity()
    {
    }

    protected Entity(TId id, Guid createdByUserId) : this()
    {
        Id = id;
        IsDeleted = false;
        Result<CreateDate> result = CreateDate.Create(DateTime.UtcNow);
        CreatedDateTime = result.Value;
        CreatedByUserId = createdByUserId;
    }

    public static bool operator ==(Entity<T, TId>? left, Entity<T, TId>? right)
    {
        return left switch
        {
            null when right is null => true,
            null => false,
            _ => right is not null && left.Equals(right),
        };
    }

    public static bool operator !=(Entity<T, TId>? left, Entity<T, TId>? right)
    {
        return !(left == right);
    }

    private int? _requestedHashCode;

    private bool IsTransient()
    {
        return Id == null || Id.IsEmpty();
    }

    public T Duplicate(DuplicateContext context)
    {
        return context.GetOrAdd(this, () =>
        {
            T copy = CreateDuplicateInstance();
            TId newId = CreateNewId();
            context.MapId(Id, newId);
            copy.Id = newId;
            CopyTo(copy, context);
            return copy;
        });
    }

    private TId CreateNewId()
    {
        System.Reflection.MethodInfo? method = typeof(TId).GetMethod(
            "New",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        if (method != null)
        {
            object? result = method.Invoke(null, null);
            if (result is TId typedId)
            {
                return typedId;
            }
        }

        throw new InvalidOperationException($"No static New() method found for {typeof(TId).Name}.");
    }

    protected abstract T CreateDuplicateInstance();

    protected virtual void CopyTo(T target, DuplicateContext context)
    {
        target.CreatedByUserId = CreatedByUserId;
        target.CreatedDateTime = CreatedDateTime;
        target.UpdatedDateTime = UpdatedDateTime;
        target.UpdatedByUserId = UpdatedByUserId;
        target.OwnedByUserId = OwnedByUserId;
        target.IsDeleted = IsDeleted;
        target.TenantId = TenantId;
    }

    protected void SetTenantId(Guid tenantId)
    {
        if (tenantId != Guid.Empty)
        {
            TenantId = tenantId;
        }
    }

    public void AssignTenantId(Guid tenantId)
    {
        SetTenantId(tenantId);
    }

    public override bool Equals(object? anotherObject)
    {
        if (anotherObject == null)
        {
            return false;
        }

        if (anotherObject is not Entity<T, TId>)
        {
            return false;
        }

        if (ReferenceEquals(this, anotherObject))
        {
            return true;
        }

        Entity<T, TId>? anotherEntity = anotherObject as Entity<T, TId>;
        if (GetRealType() != anotherEntity?.GetRealType())
        {
            return false;
        }

        if (GetType() != anotherEntity?.GetType())
        {
            return false;
        }

        if (IsTransient() || anotherEntity.IsTransient())
        {
            return false;
        }

        return Id.Equals(anotherEntity.Id);
    }

    public override int GetHashCode()
    {
        if (IsTransient())
        {
            return base.GetHashCode();
        }

        _requestedHashCode ??= Id.GetHashCode();
        return _requestedHashCode.Value;
    }

    private Type? GetRealType()
    {
        Type type = GetType();
        return type.ToString().Contains("Castle.Proxies.") ? type.BaseType : type;
    }

    public T SetUpdated(Guid updatedByUserId)
    {
        if (IsDeleted)
        {
            return (T)this;
        }

        UpdatedByUserId = updatedByUserId;
        UpdatedDateTime = UpdateDate.CreateUtcNow().Value;
        return (T)this;
    }

    public T SetDeleted(Guid deletedByUserId)
    {
        if (IsDeleted)
        {
            return (T)this;
        }

        IsDeleted = true;
        DeletedByUserId = deletedByUserId;
        UpdatedDateTime = UpdateDate.CreateUtcNow().Value;
        return (T)this;
    }
}
