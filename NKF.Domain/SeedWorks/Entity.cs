using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using FluentResults;
using NKF.Domain.SharedKernels.ValueObjects;

namespace NKF.Domain.SeedWorks;

public abstract class Entity<T> : IEntity, IAuditable, IDeepDuplicatable<T>, ITenantAware where T : Entity<T>
{
    #region Feilds

    public Guid Id { get; protected set; }

    public Guid? CreatedByUserId { get; protected set; }
    public CreateDate CreatedDateTime { get; protected set; }

    public Guid? UpdatedByUserId { get; protected set; }
    public UpdateDate UpdatedDateTime { get; protected set; }

    public Guid? OwnedByUserId { get; protected set; }

    public bool IsDeleted { get; protected set; }
    public Guid? DeletedByUserId { get; protected set; }

    [NotMapped]
    public Guid TenantId { get; protected set; }

    #endregion

    #region Constroctor(s)

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Entity() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Entity(Guid createdByUserId) : this()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        Id = Guid.NewGuid();
        IsDeleted = false;
        Result<CreateDate> result = CreateDate.Create(DateTime.UtcNow);
        CreatedDateTime = result.Value;
        CreatedByUserId = createdByUserId;
    }


    #endregion /Constroctor(s)


    #region Methods

    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        return left switch
        {
            null when right is null => true,
            null => false,
            _ => right is not null && left.Equals(right),
        };
    }

    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !(left == right);
    }


    private int? _requestedHashCode;

    private bool IsTransient() => Id == Guid.Empty;

    public T Duplicate(DuplicateContext context)
    {
        return context.GetOrAdd(this, () =>
        {
            T copy = CreateDuplicateInstance();

            copy.Id = Guid.NewGuid();
            context.MapId(Id, copy.Id);

            CopyTo(copy, context);

            return copy;
        });
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

    /// <summary>
    /// برای استفاده در لایه Persistence بعد از کوئری - به صورت NotMapped ست می‌شود
    /// </summary>
    public void AssignTenantId(Guid tenantId) => SetTenantId(tenantId);

    public override bool Equals(object? anotherObject)
    {
        if (anotherObject == null)
        {
            return false;
        }

        if (anotherObject is not Entity<T>)
        {
            return false;
        }

        if (ReferenceEquals(this, anotherObject))
        {
            return true;
        }

        Entity<T>? anotherEntity = anotherObject as Entity<T>;
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

        return Id == anotherEntity.Id;
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

    #endregion
}
