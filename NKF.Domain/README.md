# NKF.Domain

Core domain primitives for Domain-Driven Design in .NET.

## Features

- **AggregateRoot<T>** — Event-sourced aggregate base with `ApplyEventAsync`
- **Entity<T>** — Base entity with auditing (CreatedBy, UpdatedBy, Soft Delete)
- **ValueObject** — Immutable value equality semantics
- **Enumeration** — Type-safe enum pattern
- **Domain Events** — `IDomainEvent`, `RaiseDomainEvent`, `DomainEvents` collection
- **Shared Kernels** — Cross-domain value objects (CreateDate, UpdateDate, etc.)

## Install

```bash
dotnet add package NKF.Domain
```

## Usage

```csharp
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
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0