# NKF.Persistence.Postgres.Base

EF Core PostgreSQL implementation for NKF Persistence.

## Features

- **CommandRepository<T>** — EF Core write repository base (auto-assigns `ITenantAware.TenantId`)
- **QueryRepository<T>** — EF Core read repository base (auto-assigns `ITenantAware.TenantId`)
- **TypedCommandRepository<TEntity,TId>** — EF Core typed write repository base (1.0.1)
- **TypedQueryRepository<TEntity,TId>** — EF Core typed read repository base (1.0.1)
- **CommandUnitOfWork** — Transaction management with Mediator for domain events + `TenantId` propagation
- **QueryUnitOfWork** — Read-only unit of work + `TenantId` propagation
- **PersistenceOperation<TRequest,TResult>** — Base class for one EF Core operation; subclasses implement `Execute`
- **CommandPersistenceOperation / QueryPersistenceOperation** — EF bases parameterized by a concrete DbContext type; expose it to subclasses through `protected Context` only
- **UnitOfWork** — Saves changes only
- **TransactionManager** — Begins EF Core transactions separately from the Unit of Work
- **Tenant Services** — Multi-tenancy with connection resolution
- **FAppDbContext** — Base DbContext with tenant support

## Install

```bash
dotnet add package NKF.Persistence.Postgres.Base
```

## Usage

```csharp
public class BrandCommandRepository : CommandRepository<Brand>, IBrandCommandRepository
{
    public BrandCommandRepository(AppDbContext context) : base(context) { }
    
    public Task<bool> HasExistBrandByName(BrandName name, CancellationToken ct)
        => Context.Set<Brand>().AnyAsync(b => b.DefaultName == name, ct);
}

// Typed (1.0.1)
public class TypedBrandCommandRepository : TypedCommandRepository<TypedBrand, BrandId>, ITypedBrandCommandRepository
{
    public TypedBrandCommandRepository(AppDbContext context) : base(context) { }
    public Task<bool> HasExistBrandByName(BrandName name, CancellationToken ct)
        => Context.Set<TypedBrand>().AnyAsync(b => b.DefaultName == name, ct);
}

// DI Registration (TenantId propagated automatically)
services.AddScoped<ICommandUnitOfWork, CommandUnitOfWork>(s =>
    new CommandUnitOfWork(
        s.GetRequiredService<AppDbContext>(),
        CommandCollection.DeepCopy(),
        s.GetRequiredService<IMediator>()
    ));
// Typed repos share the same CommandCollection / QueryCollection via AddFromAssembly
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0
