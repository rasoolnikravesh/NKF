# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-01-17

### Added
- **NKF.Domain**: Core domain primitives
  - `AggregateRoot<T>` - Event-sourced aggregate base with `ApplyEventAsync`
  - `Entity<T>` - Base entity with auditing (CreatedBy, UpdatedBy, Soft Delete)
  - `ValueObject` - Immutable value equality semantics
  - `Enumeration` - Type-safe enum pattern
  - `IDomainEvent` - Domain event marker interface
  - Shared Kernels: `CreateDate`, `UpdateDate`, `DuplicateContext`

- **NKF.Application.Base**: Application layer with CQRS
  - `ICommandHandler<TCommand>` / `ICommandHandler<TCommand, TResponse>`
  - `IQueryHandler<TQuery, TResponse>`
  - MediatR Pipeline Behaviors:
    - `LoggingBehavior` - Request/response logging
    - `ValidationBehavior` - FluentValidation integration
    - `PerformanceBehaviour` - Slow request detection
    - `ExceptionBehaviour` - Global exception handling
    - `EventBehavior` - Domain event publishing

- **NKF.Application.Contracts.Base**: CQRS contract interfaces
  - `ICommand` / `ICommand<TResponse>` - Command markers
  - `IQuery<TResponse>` - Query marker

- **NKF.Persistence.Base**: Persistence abstractions
  - `IRepository<T>` / `ICommandRepository<T>` / `IQueryRepository<T>`
  - `IUnitOfWork` / `ICommandUnitOfWork` / `IQueryUnitOfWork`
  - `RepositoryCollection` - Auto-discovery for repositories
  - `TenantContext` - Multi-tenancy support

- **NKF.Persistence.Postgres.Base**: EF Core PostgreSQL implementation
  - `CommandRepository<T>` / `QueryRepository<T>` - EF Core bases
  - `CommandUnitOfWork` / `QueryUnitOfWork` - Transaction management
  - Tenant services for multi-tenancy

- **NKF.Resources**: Shared resources
  - `DataDictionary` - Localized entity/property names
  - `ErrorMessages` - Validation error messages
  - `Validations` - Business rule messages

- **NKF.Domain.Tests**: Unit tests for domain primitives

### Infrastructure
- Apache-2.0 License
- SourceLink enabled for debugging
- MinVer for automatic versioning from git tags
- Symbol packages (snupkg) for NuGet
- Embedded XML documentation

---

## [Unreleased]

### Planned
- Roslyn analyzers for DDD patterns
- Additional database providers (SQL Server, MySQL)
- OpenTelemetry integration
- More comprehensive test coverage