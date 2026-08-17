# NKF (Nikravesh Framework)

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)]()
[![NuGet](https://img.shields.io/badge/NuGet-Available-blue?logo=nuget)]()

> A modular, Domain-Driven Design (DDD) framework for building maintainable, scalable .NET applications with clean architecture principles.

---

## 🎯 Overview

**NKF (Nikravesh Framework)** is a lightweight, opinionated .NET framework that enforces **Clean Architecture** and **Domain-Driven Design** patterns out of the box. It provides battle-tested building blocks for:

- **Domain Layer**: Entities, Aggregates, Value Objects, Domain Events
- **Application Layer**: CQRS with MediatR, Commands/Queries/Handlers
- **Persistence Layer**: Repository Pattern, Unit of Work, EF Core (PostgreSQL)
- **Cross-Cutting**: Multi-tenancy, Localization, Validation, Logging pipelines

Built for teams who want **structure without ceremony** — start modeling your domain, not your infrastructure.

---

## 🏗 Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        NKF Framework                            │
├─────────────────┬─────────────────┬─────────────────────────────┤
│     Domain      │   Application   │      Persistence            │
├─────────────────┼─────────────────┼─────────────────────────────┤
│ • AggregateRoot │ • ICommand      │ • IRepository<T>            │
│ • Entity<T>     │ • IQuery<T>     │ • ICommandRepository<T>     │
│ • ValueObject   │ • ICommandHandler│ • IQueryRepository<T>      │
│ • IDomainEvent  │ • IQueryHandler │ • IUnitOfWork               │
│ • Domain Events │ • MediatR       │ • EF Core (PostgreSQL)      │
│ • Enumerations  │ • Pipeline      │ • Multi-tenancy             │
└─────────────────┴─────────────────┴─────────────────────────────┘
```

### Core Principles

| Principle | Implementation |
|-----------|----------------|
| **Domain-Centric** | Business logic lives in Aggregates, not services |
| **CQRS** | Separate read/write models via Commands & Queries |
| **Event-Driven** | Domain events for side effects & integration |
| **Persistence Ignorance** | Domain has zero dependencies on EF Core |
| **Testable by Default** | Pure domain logic, easy to unit test |

---

## 📦 Project Structure

```
src/FrameWork/
├── NKF.Domain                 # Core domain primitives
│   ├── SeedWorks/
│   │   ├── AggregateRoot.cs   # Event-sourced aggregate base
│   │   ├── Entity.cs          # Base entity with auditing
│   │   ├── ValueObject.cs     # Immutable value equality
│   │   ├── Enumeration.cs     # Type-safe enums
│   │   └── IDomainEvent.cs    # Domain event marker
│   └── SharedKernels/         # Cross-domain value objects
│
├── NKF.Domain.Events          # IDomainEvent interface
│
├── NKF.Application.Base       # Application layer contracts
│   ├── ICommandHandler.cs     # Command handler base
│   ├── IQueryHandler.cs       # Query handler base
│   └── MediatorPipelines/     # Logging, Validation, Performance
│
├── NKF.Application.Contracts.Base
│   ├── ICommand.cs            # Command marker (returns Result)
│   └── IQuery.cs              # Query marker (returns Result<T>)
│
├── NKF.Persistence.Base       # Persistence abstractions
│   ├── IRepository.cs         # Read repository
│   ├── ICommandRepository.cs  # Write repository
│   ├── IQueryRepository.cs    # Read-only repository
│   ├── IUnitOfWork.cs         # Transaction boundary
│   └── TenantContext.cs       # Multi-tenancy support
│
├── NKF.Persistence.Postgres.Base  # EF Core PostgreSQL implementation
│   ├── SeedWorks/             # Command/Query repositories
│   └── Services/              # Tenant services
│
├── NKF.Resources              # Shared resources
│   ├── DataDictionary.resx    # Localized entity/property names
│   └── Messages/              # Validation/error messages
│
└── NKF.Domain.Tests           # Domain layer unit tests
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL 14+ (for persistence)
- Redis (optional, for caching)

### Installation

```bash
# Clone the repository
git clone https://github.com/your-org/NKP.git
cd NKP/src/FrameWork

# Restore dependencies
dotnet restore FrameWork.slnx

# Build
dotnet build FrameWork.slnx

# Run tests
dotnet test FrameWork.slnx
```

### Add to Your Solution

Reference the framework projects in your solution:

```xml
<!-- YourService.Domain.csproj -->
<ProjectReference Include="..\..\FrameWork\NKF.Domain\NKF.Domain.csproj" />

<!-- YourService.Application.csproj -->
<ProjectReference Include="..\..\FrameWork\NKF.Application.Base\NKF.Application.Base.csproj" />
<ProjectReference Include="..\..\FrameWork\NKF.Application.Contracts.Base\NKF.Application.Contracts.Base.csproj" />

<!-- YourService.Persistence.csproj -->
<ProjectReference Include="..\..\FrameWork\NKF.Persistence.Base\NKF.Persistence.Base.csproj" />
<ProjectReference Include="..\..\FrameWork\NKF.Persistence.Postgres.Base\NKF.Persistence.Postgres.Base.csproj" />
```

---

## 🧩 Building a Service with NKF

Let's walk through creating a **Brand** aggregate (illustrative example from the companion `BaseData` reference service).

### 1. Domain Layer

**Value Objects** — Immutable, validated data:

```csharp
// NKP.Services.BaseData.Domain/Aggregates/Brands/ValueObjects/BrandName.cs
public class BrandName : ValueObject
{
    public string Name { get; }

    private BrandName(string name) => Name = name;

    public static Result<BrandName> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail("Brand name is required");
        if (name.Length > 100)
            return Result.Fail("Brand name too long");
        return Result.Ok(new BrandName(name.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
    }
}
```

**Aggregate Root** — Encapsulates business logic & emits events:

```csharp
// NKP.Services.BaseData.Domain/Aggregates/Brands/Brand.cs
public class Brand : AggregateRoot<Brand>
{
    public BrandSlug Slug { get; private set; }
    public BrandName DefaultName { get; private set; }
    public ActiveStatus ActiveStatus { get; private set; }
    private readonly List<BrandTranslation> _translations = [];
    public IReadOnlyCollection<BrandTranslation> Translations => _translations.AsReadOnly();

    // Factory method — enforces invariants
    public static async Task<Result<Brand>> CreateAsync(string? name)
    {
        var nameResult = BrandName.Create(name);
        if (nameResult.IsFailed) return Result.Fail(nameResult.Errors);

        var brand = new Brand(nameResult.Value);
        
        // Emit domain event
        var createdEvent = new BrandCreatedEvent(brand.Id, brand.DefaultName.Name, false);
        return await brand.ApplyEventAsync<Brand>(createdEvent);
    }

    // Business operations — return Result, emit events
    public async Task<Result<Brand>> ActivateAsync()
    {
        if (ActiveStatus == ActiveStatus.Active) return Result.Ok(this);
        return await ApplyEventAsync<Brand>(new BrandStatusChangedEvent(Id, true));
    }

    // Event handlers — mutate state
    public Result<Brand> Apply(BrandStatusChangedEvent @event)
    {
        ActiveStatus = @event.IsActive ? ActiveStatus.Active : ActiveStatus.Deactive;
        UpdatedDateTime = UpdateDate.CreateUtcNow().Value;
        return Result.Ok(this);
    }

    protected override Brand CreateDuplicateInstance() => new Brand();
}
```

**Domain Events** — Capture intent for side effects:

```csharp
// NKP.Services.BaseData.Domain/Aggregates/Brands/Events/BrandCreatedEvent.cs
public record BrandCreatedEvent(Guid BrandId, string Name, bool IsActive) : IDomainEvent;
```

### 2. Application Layer

**Commands/Queries** — Request/Response DTOs:

```csharp
// NKP.Services.BaseData.Application.Contracts/Aggregates/Brands/Commands/CreateBrandCommand.cs
public record CreateBrandCommand : ICommand<BrandCreatedResponse>
{
    public string? Name { get; set; }
    [JsonIgnore] public CultureInfo? Culture { get; set; }
}

public record BrandCreatedResponse { public Guid Id { get; set; } public string Name { get; set; } }
```

**Command Handler** — Orchestrates domain logic:

```csharp
// NKP.Services.BaseData.Application/Aggregates/Brands/CommandHandlers/CreateBrandCommandHandler.cs
public class CreateBrandCommandHandler : ICommandHandler<CreateBrandCommand, BrandCreatedResponse>
{
    private readonly ICommandUnitOfWork _unitOfWork;
    private readonly IBrandCommandRepository _brandRepo;
    private readonly IMapper _mapper;

    public async Task<Result<BrandCreatedResponse>> Handle(CreateBrandCommand request, CancellationToken ct)
    {
        // 1. Create aggregate (validates invariants)
        var createResult = await Brand.CreateAsync(request.Name);
        if (createResult.IsFailed) return Result.Fail(createResult.Errors);

        // 2. Check business rules (uniqueness)
        if (await _brandRepo.HasExistBrandByName(createResult.Value.DefaultName, ct))
            return Result.Fail("Brand already exists");

        // 3. Persist
        await _brandRepo.AddAsync(createResult.Value, ct);
        await _unitOfWork.SaveAsync(ct);

        // 4. Return response
        return Result.Ok(_mapper.Map<BrandCreatedResponse>(createResult.Value));
    }
}
```

### 3. Persistence Layer

**Repository** — EF Core implementation:

```csharp
// NKP.Services.BaseData.Persistence.Postgres/Aggregates/Brands/BrandCommandRepository.cs
public class BrandCommandRepository : CommandRepository<Brand>, IBrandCommandRepository
{
    public BrandCommandRepository(FAppDbContext context) : base(context) { }

    public Task<bool> HasExistBrandByName(BrandName name, CancellationToken ct)
        => Context.Set<Brand>().AnyAsync(b => b.DefaultName == name, ct);
}
```

**Configuration** — Fluent API mapping:

```csharp
// NKP.Services.BaseData.Persistence.Postgres/Aggregates/Brands/BrandConfiguration.cs
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        builder.HasKey(b => b.Id);
        builder.OwnsOne(b => b.DefaultName, n => n.Property(p => p.Name).HasColumnName("DefaultName"));
        builder.OwnsOne(b => b.Slug);
        builder.HasMany(b => b.Translations).WithOne().HasForeignKey(t => t.BrandId);
    }
}
```

### 4. API Layer

**Controller** — Thin mediator wrapper:

```csharp
// NKP.Services.BaseData.Api/Controllers/BrandsController.cs
[ApiController]
[Route("api/v1/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    [ProducesResponseType(typeof(Result<BrandCreatedResponse>), 200)]
    [ProducesResponseType(typeof(Result), 400)]
    public async Task<IActionResult> Create(CreateBrandCommand command)
    {
        command.Culture = CultureInfo.CurrentCulture;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
}
```

---

## ⚙️ Key Framework Features

### Pipeline Behaviors (Cross-Cutting Concerns)

```csharp
// NKF.Application.Base/MediatorPipelines/
├── LoggingBehavior.cs        # Request/response logging
├── ValidationBehavior.cs     # FluentValidation integration
├── PerformanceBehaviour.cs   # Slow request detection
├── ExceptionBehaviour.cs     # Global exception handling
└── EventBehavior.cs          # Domain event publishing
```

Register in `Program.cs`:

```csharp
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
});
```

### Multi-Tenancy

```csharp
// NKF.Persistence.Base/TenantContext.cs
public class TenantContext
{
    public Guid TenantId { get; set; }
    public string ConnectionString { get; set; }
}

// Automatically scoped per request via middleware
app.UseMiddleware<TenantMiddleware>();
```

### Localization & Resources

```csharp
// NKF.Resources/Messages/ErrorMessages.resx
// Key: DuplicateValue -> "The {0} '{1}' already exists"

// Usage in domain:
string error = string.Format(ErrorMessages.DuplicateValue, DataDictionary.BrandName, name);
```

### Auditing & Soft Delete (Built into Entity)

```csharp
// Automatic fields on every Entity:
public Guid Id { get; }
public Guid? CreatedByUserId { get; }
public CreateDate CreatedDateTime { get; }
public Guid? UpdatedByUserId { get; }
public UpdateDate UpdatedDateTime { get; }
public bool IsDeleted { get; }
public Guid? DeletedByUserId { get; }

// Usage:
entity.SetUpdated(userId);   // Sets UpdatedBy/UpdatedDate
entity.SetDeleted(userId);   // Soft delete
```

---

### Repository Collection & Auto-Discovery

NKF uses a **RepositoryCollection** pattern for lazy, type-safe repository resolution. Instead of registering each repository individually in DI, you register a collection that maps aggregate interfaces to implementations.

#### How It Works

```csharp
// NKF.Persistence.Base/RepositoryCollection/
├── RepositoryCollection.cs        // Thread-safe collection of RepositoryDescriptor
├── RepositoryDescriptor.cs        // Maps RepositoryType -> ImplementationType/Instance
├── RepositoryCollectionExtensions.cs // Fluent API: Add<T>(), AddFromAssembly()
└── IRepositoryCollection.cs       // IList<RepositoryDescriptor> + IDisposable
```

#### Auto-Discovery with `AddFromAssembly`

Register all repositories in an assembly by convention:

```csharp
// In your Persistence.Settings/DependContainer.cs
private static readonly RepositoryCollection CommandCollection = [];
private static readonly RepositoryCollection QueryCollection = [];

static DependContainer()
{
    // Scans assembly for I*CommandRepository<T> and matches to *CommandRepository<T>
    CommandCollection.AddFromAssembly(
        typeof(ICategoryCommandRepository),  // Any interface in target assembly
        typeof(CategoryCommandRepository)    // Any implementation in impl assembly
    );
    
    QueryCollection.AddFromAssembly(
        typeof(ICategoryQueryRepository),
        typeof(CategoryQueryRepository)
    );
}
```

**Convention**: For aggregate `T`, it finds:
- `I{Name}CommandRepository<T>` → `{Name}CommandRepository<T>`
- `I{Name}QueryRepository<T>` → `{Name}QueryRepository<T>`

#### Manual Registration

```csharp
// Explicit registration
CommandCollection.Add<IBrandCommandRepository, BrandCommandRepository>();
CommandCollection.Add<ICategoryCommandRepository, CategoryCommandRepository>();

// Or with factory
CommandCollection.Add<IBrandCommandRepository>(sp => 
    new BrandCommandRepository(sp.GetRequiredService<AppDbContext>()));
```

#### Usage in Unit of Work

The `CommandUnitOfWork` resolves repositories **on-demand** via `GetCommandRepository<T>()`:

```csharp
// NKF.Persistence.Postgres.Base/SeedWorks/CommandUnitOfWork.cs
public ICommandRepository<T> GetCommandRepository<T>() where T : class, IAggregateRoot
    => DescriptorImplementationInstance<T>(typeof(T));

// In handler:
public async Task<Result<BrandCreatedResponse>> Handle(CreateBrandCommand request, CancellationToken ct)
{
    var brandRepo = _unitOfWork.GetCommandRepository<Brand>(); // Auto-resolved!
    await brandRepo.AddAsync(brand, ct);
    await _unitOfWork.SaveAsync(ct);
}
```

**Key behaviors:**
- **Lazy instantiation**: Repository created on first request
- **Scoped lifecycle**: Instance cached in collection for request duration
- **Type-safe**: Returns `ICommandRepository<T>` not `object`
- **Fails fast**: Throws `InvalidOperationException` if not registered

#### DI Registration

Pass a **deep copy** per request to isolate collections:

```csharp
// In DependContainer.AddUnitOfWork()
services.AddScoped<ICommandUnitOfWork, CommandUnitOfWork>(s =>
{
    var context = s.GetRequiredService<AppDbContext>();
    var mediator = s.GetRequiredService<IMediator>();
    return new CommandUnitOfWork(context, CommandCollection.DeepCopy(), mediator);
});

services.AddScoped<IQueryUnitOfWork, QueryUnitOfWork>(s =>
{
    var context = s.GetRequiredService<AppDbContext>();
    return new QueryUnitOfWork(context, QueryCollection.DeepCopy());
});
```

#### Custom Repository Interfaces

Define aggregate-specific contracts:

```csharp
// Contracts
public interface IBrandCommandRepository : ICommandRepository<Brand>
{
    Task<bool> HasExistBrandByName(BrandName name, CancellationToken ct);
}

// Implementation
public class BrandCommandRepository : CommandRepository<Brand>, IBrandCommandRepository
{
    public BrandCommandRepository(AppDbContext context) : base(context) { }
    
    public Task<bool> HasExistBrandByName(BrandName name, CancellationToken ct)
        => Context.Set<Brand>().AnyAsync(b => b.DefaultName == name, ct);
}
```

The auto-discovery will match `IBrandCommandRepository` → `BrandCommandRepository` automatically.

---

## 🧪 Testing

Domain logic is pure and easily testable:

```csharp
// NKF.Domain.Tests/Aggregates/BrandTests.cs
public class BrandTests
{
    [Fact]
    public async Task CreateBrand_WithValidName_ReturnsBrand()
    {
        // Arrange
        var name = "Test Brand";

        // Act
        var result = await Brand.CreateAsync(name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DefaultName.Name.Should().Be(name);
        result.Value.ActiveStatus.Should().Be(ActiveStatus.Deactive);
        result.Value.DomainEvents.Should().ContainSingle<BrandCreatedEvent>();
    }

    [Fact]
    public async Task ActivateBrand_WhenAlreadyActive_ReturnsSameInstance()
    {
        var brand = (await Brand.CreateAsync("Test")).Value;
        await brand.ActivateAsync(); // First activation
        
        var result = await brand.ActivateAsync(); // Second activation
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(brand); // No new event
    }
}
```

Run tests:

```bash
dotnet test NKF.Domain.Tests/NKF.Domain.Tests.csproj
```

---

## 📚 Reference Implementation

The code examples throughout this README are drawn from a **companion service** (`BaseData`) that demonstrates NKF in a real-world scenario. That service lives in a separate repository and includes:

| Module | Description |
|--------|-------------|
| **Domain** | Brands, Categories, Languages, Parties, Products aggregates |
| **Application** | Commands, Queries, Handlers, AutoMapper profiles |
| **Contracts** | DTOs for Commands/Queries/Responses |
| **Persistence.Postgres** | EF Core mappings, repositories, migrations |
| **Api** | Controllers, Swagger, Middleware, DI configuration |

Key patterns showcased:
- Multi-language support (translations)
- Hierarchical categories (self-referencing)
- Party/Organization unit modeling
- Product with variants & attributes

> **Note**: The `BaseData` service is not included in this framework repository. It serves as a reference implementation only.
---

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Coding Standards

- Follow existing code style (`.editorconfig` provided)
- Write tests for new domain logic
- Update documentation for public APIs
- Keep commits atomic and messages clear

---

## 📄 License

Distributed under the **Apache License 2.0**. See `LICENSE` for more information.

---

## 🙏 Acknowledgments

- **MediatR** — CQRS mediator implementation
- **FluentResults** — Functional result handling
- **FluentValidation** — Validation pipeline
- **EF Core** — ORM for persistence
- **Serilog** — Structured logging

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/rasolnikravesh/NKF/issues)
- **Discussions**: [GitHub Discussions](https://github.com/rasoolnikravesh/NKF/discussions)
- **Email**: nikravesh.rasool@gmail.com

---

<div align="center">

**Built with ❤️ by the Nikravesh Team**

*Empowering .NET developers to build better domain models, faster.*

</div>
