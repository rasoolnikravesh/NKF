# NKF.Persistence.Base

Persistence abstractions: Repository pattern, Unit of Work, and multi-tenancy.

## Features

- **IRepository<T>** — Read repository
- **ICommandRepository<T>** — Write repository (Add, Update, Remove)
- **IQueryRepository<T>** — Read-only repository
- **IUnitOfWork / ICommandUnitOfWork / IQueryUnitOfWork** — Transaction boundaries
- **RepositoryCollection** — Auto-discovery for repositories
- **TenantContext** — Multi-tenancy support

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

// Auto-discovery registration
CommandCollection.AddFromAssembly(
    typeof(IBrandCommandRepository),
    typeof(BrandCommandRepository)
);

// In handler
var repo = _unitOfWork.GetCommandRepository<Brand>();
await repo.AddAsync(brand, ct);
await _unitOfWork.SaveAsync(ct);
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0