# NKF.Persistence.Postgres.Base

EF Core PostgreSQL support for operation based persistence.

## Base operations

- `CommandPersistenceOperation<TContext, TRequest, TResult>` and `QueryPersistenceOperation<TContext, TRequest, TResult>` provide a strongly typed `DbContext` through the protected `Context` property.
- `PersistenceOperation<TContext, TRequest, TResult>` is the common operation base.
- Implementations override one `Execute` method and should be registered with scoped lifetime.

## Unit of work and transactions

- `UnitOfWork` saves changes and flushes the Wolverine EF Core outbox when configured.
- `TransactionManager` opens an EF Core database transaction.
- `FAppDbContext` is an optional shared EF Core base context.

Operation implementations own tenant-specific query filters and tenant assignment. The framework does not hide database access behind aggregate repositories.
