# NKF.Domain

Core domain primitives for the NKF services.

## Entities and aggregate roots

Use the single typed model:

```csharp
public sealed class BrandId : EntityId
{
    private BrandId(Guid value) : base(value) { }
    public static BrandId New() => new(Guid.NewGuid());
    public static Result<BrandId> Create(Guid value) => Result.Ok(new BrandId(value));
}

public sealed class Brand : AggregateRoot<Brand, BrandId>
{
    private Brand() { }
    private Brand(BrandId id, Guid createdByUserId) : base(id, createdByUserId) { }
}
```

`Entity<TEntity, TId>` supplies a typed ID, audit fields, soft deletion, tenant awareness, and duplicate support. `AggregateRoot<TEntity, TId>` adds domain event tracking and `ApplyEventAsync`.

All persistent entity and aggregate IDs should use a domain-specific `EntityId` type. EF Core mappings convert the typed ID to its stored primitive value.

## Other primitives

- `ValueObject` implements component based equality.
- `Enumeration` provides type safe enumerations.
- `IDomainEvent` marks domain events.
- `ITenantAware` supports tenant assignment from persistence code.
- `DuplicateContext` maps entity IDs while copying object graphs.
