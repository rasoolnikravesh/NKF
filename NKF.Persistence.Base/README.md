# NKF.Persistence.Base

Persistence abstractions: Repository pattern, Unit of Work, and multi-tenancy.

## Features

- **IRepository<T>** — Read repository
- **ICommandRepository<T>** — Write repository (Add, Update, Remove)
- **IQueryRepository<T>** — Read-only repository
- **ITypedCommandRepository<TEntity,TId>** — Strongly-typed write repository for `TypedAggregateRoot<T,TId>` (1.0.1)
- **ITypedQueryRepository<TEntity,TId>** — Strongly-typed read repository for `TypedAggregateRoot<T,TId>` (1.0.1)
- **IUnitOfWork / ICommandUnitOfWork / IQueryUnitOfWork** — Transaction boundaries with auto `TenantId` propagation
- **IPersistenceOperation<TRequest,TResult>** — One typed `Execute` contract per database operation
- **ICommandPersistenceOperation / IQueryPersistenceOperation** — Role-specific contracts for write and read operations
- **IUnitOfWork** — Save changes only; transaction creation is handled separately
- **ITransactionManager** — Explicit transaction boundary, separate from saving changes
- **RepositoryCollection** — Auto-discovery for repositories
- **TenantContext** — Multi-tenancy support (auto-assigns `ITenantAware.TenantId`)

## Install

```bash
dotnet add package NKF.Persistence.Base
```

## Usage

For operation-based persistence, define a named interface for each database action and implement it in the provider-specific project:

```csharp
public interface IGetBrandByIdOperation : IPersistenceOperation<Guid, Brand?>
{
}
```

Prefer role-specific contracts for new work: `ICommandPersistenceOperation<TRequest, TResult>` or `IQueryPersistenceOperation<TRequest, TResult>`. Give each operation its own named interface and keep command/query interfaces in separate folders.

Application handlers inject the named interface and call `Execute`. They do not inject the generic contract directly.

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
