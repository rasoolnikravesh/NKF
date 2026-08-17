# NKF.Persistence.Postgres.Base

EF Core PostgreSQL implementation for NKF Persistence.

## Features

- **CommandRepository<T>** — EF Core write repository base
- **QueryRepository<T>** — EF Core read repository base
- **CommandUnitOfWork** — Transaction management with MediatR for domain events
- **QueryUnitOfWork** — Read-only unit of work
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

// DI Registration
services.AddScoped<ICommandUnitOfWork, CommandUnitOfWork>(s =>
    new CommandUnitOfWork(
        s.GetRequiredService<AppDbContext>(),
        CommandCollection.DeepCopy(),
        s.GetRequiredService<IMediator>()
    ));
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0