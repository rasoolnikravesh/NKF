# NKF.Domain

Core domain primitives for Domain-Driven Design in .NET.

## Features

- **AggregateRoot<T>** — Event-sourced aggregate base with `ApplyEventAsync`
- **TypedAggregateRoot<T,TId>** — Strongly-typed aggregate base with `EntityId` and event sourcing
- **Entity<T>** — Base entity with auditing (CreatedBy, UpdatedBy, Soft Delete) and `ITenantAware` (`TenantId [NotMapped]` + `AssignTenantId`)
- **TypedEntity<T,TId>** — Strongly-typed entity base with `TId : EntityId`, same auditing + `ITenantAware`
- **EntityId** — Strongly-typed ID ValueObject wrapping `Guid` (`IsEmpty()`, `==/!=`, `ToString()`) with `EntityIdJsonConverter<TId>` for JSON
- **IEntity / IEntity<TId>** — Non-typed and typed entity markers
- **ITenantAware** — `TenantId` + `AssignTenantId(Guid)` for persistence-layer assignment
- **ValueObject** — Immutable value equality semantics
- **Enumeration** — Type-safe enum pattern
- **Domain Events** — `IDomainEvent`, `RaiseDomainEvent`, `DomainEvents` collection
- **Shared Kernels** — Cross-domain value objects (CreateDate, UpdateDate, DuplicateContext with typed `MapId`/`GetMappedId<TId>`) and `DomainErrors.Duplicate`

## Install

```bash
dotnet add package NKF.Domain
```

## Usage

```csharp
// Non-typed (Guid) aggregate
public class Brand : AggregateRoot<Brand>
{
    public BrandName DefaultName { get; private set; }
    
    public static async Task<Result<Brand>> CreateAsync(string name)
    {
        var nameResult = BrandName.Create(name);
        var brand = new Brand(nameResult.Value);
        
        var evt = new BrandCreatedEvent(brand.Id, name);
        return await brand.ApplyEventAsync<Brand>(evt);
    }
    
    public Result<Brand> Apply(BrandCreatedEvent @event) => Result.Ok(this);
}

// Strongly-typed ID + aggregate (1.0.1)
public sealed class BrandId : EntityId
{
    private BrandId(Guid value) : base(value) { }
    public static Result<BrandId> Create(Guid? value) => !value.HasValue || value.Value == Guid.Empty ? Result.Fail("Id required") : Result.Ok(new BrandId(value.Value));
    public static Result<BrandId> Create(Guid value) => Create((Guid?)value);
    public static BrandId New() => new(Guid.NewGuid());
}
public class TypedBrand : TypedAggregateRoot<TypedBrand, BrandId>
{
    public BrandName DefaultName { get; private set; } = default!;
    private TypedBrand() { }
    private TypedBrand(BrandId id, BrandName name, Guid userId) : base(id, userId) => DefaultName = name;
    public static async Task<Result<TypedBrand>> CreateAsync(string name, Guid userId)
    {
        var n = BrandName.Create(name);
        if (n.IsFailed) return Result.Fail(n.Errors);
        var b = new TypedBrand(BrandId.New(), n.Value, userId);
        return await b.ApplyEventAsync<TypedBrand>(new BrandCreatedEvent(b.Id.Value, name));
    }
    public Result<TypedBrand> Apply(BrandCreatedEvent @event) => Result.Ok(this);
    protected override TypedBrand CreateDuplicateInstance() => new TypedBrand();
}
// JSON: services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new EntityIdJsonConverter<BrandId>()));
// Duplicate with typed ID remapping:
var ctx = new DuplicateContext();
var clone = brand.Duplicate(ctx); // auto-remaps BrandId via MapId/GetMappedId<TId>
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0