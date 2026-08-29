# NKF.Persistence.Base

Persistence abstractions: Repository pattern, Unit of Work, and multi-tenancy.

## Features

- **IRepository<T>** — Read repository
- **ICommandRepository<T>** — Write repository (Add, Update, Remove)
- **IQueryRepository<T>** — Read-only repository
- **ITypedCommandRepository<TEntity,TId>** — Strongly-typed write repository for `TypedAggregateRoot<T,TId>` (1.0.1)
- **ITypedQueryRepository<TEntity,TId>** — Strongly-typed read repository for `TypedAggregateRoot<T,TId>` (1.0.1)
- **IUnitOfWork / ICommandUnitOfWork / IQueryUnitOfWork** — Transaction boundaries with auto `TenantId` propagation
- **RepositoryCollection** — Auto-discovery for repositories
- **TenantContext** — Multi-tenancy support (auto-assigns `ITenantAware.TenantId`)

## Install

```bash
dotnet add package NKF.Persistence.Base
```

## Usage

```csharp
public interface IBrandCommandRepository : ICommandRepository<Brand>
{
    Task<bool> HasExistBrandByName(BrandName name, CancellationToken ct);
}

// Strongly-typed (1.0.1)
public interface ITypedBrandCommandRepository : ITypedCommandRepository<TypedBrand, BrandId>
{
    Task<bool> HasExistBrandByName(BrandName name, CancellationToken ct);
}

// Auto-discovery registration
CommandCollection.AddFromAssembly(
    typeof(IBrandCommandRepository),
    typeof(BrandCommandRepository)
);

// In handler
var repo = _unitOfWork.GetCommandRepository<Brand>();
await repo.AddAsync(brand, ct);
await _unitOfWork.SaveAsync(ct);

// Typed with TenantId auto-propagation (assigned by CommandUnitOfWork/QueryUnitOfWork)
var typedRepo = _unitOfWork.GetTypedCommandRepository<TypedBrand, BrandId>();
await typedRepo.AddAsync(typedBrand, ct); // TenantId set automatically
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0