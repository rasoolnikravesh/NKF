# NKF.Persistence.Postgres.Base

EF Core PostgreSQL support for operation based persistence.

## Base operations

- `CommandPersistenceOperation<TContext, TRequest, TResult>` and `QueryPersistenceOperation<TContext, TRequest, TResult>` provide a strongly typed `DbContext` through the protected `Context` property.
- `TenantCommandPersistenceOperation<TContext, TRequest, TResult>` and `TenantQueryPersistenceOperation<TContext, TRequest, TResult>` resolve the current tenant context asynchronously through `ITenantDbContextSession<TContext>`.
- `PersistenceOperation<TRequest, TResult>` is the common operation base.
- Implementations override one `Execute` method and should be registered with scoped lifetime.

## Unit of work and transactions

- `SaveChangesOperation` saves changes and flushes the Wolverine EF Core outbox for a regular scoped context.
- `TenantSaveChangesOperation<TContext>` saves changes and flushes the outbox for the current tenant session.
- `TransactionManager` opens an EF Core database transaction.
- `AddWolverineTenantPersistence<TContext>()` registers a session that creates a Wolverine EF Core outbox context for an explicit tenant id. Use it with Wolverine-managed multi-tenancy.
- `AddScopedWolverineTenantPersistence<TContext>()` registers a session around a `DbContext` and outbox already configured for the current scope's tenant.
- Tenant sessions reject missing tenant ids and requests that do not match the current tenant. Tenant operations use `GetContextAsync(cancellationToken)` before accessing EF Core.
- `FAppDbContext` is an optional shared EF Core base context.

Operation implementations own tenant-specific query filters and tenant assignment. The framework does not hide database access behind aggregate repositories.
