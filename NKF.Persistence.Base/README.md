# NKF.Persistence.Base

Persistence contracts for named database operations, save coordination, and transactions.

## Operation contracts

Give each database action a named interface. Keep command and query contracts in separate folders, grouped by aggregate and entity:

```csharp
public interface IGetBrandByIdOperation
    : IQueryPersistenceOperation<Guid, Brand?>;
```

Application handlers inject the named interface and call its single `Execute` method. They do not depend on `DbContext` or generic operation contracts directly.

`ICommandPersistenceOperation<TRequest, TResult>` and `IQueryPersistenceOperation<TRequest, TResult>` both extend `IPersistenceOperation<TRequest, TResult>`. The named interface is what the application layer consumes.

## Save and transaction contracts

- `IUnitOfWork.SaveChangesAsync` persists the current unit of work.
- `ITransactionManager.BeginTransactionAsync` starts an explicit transaction when a use case needs one.
- `ITransaction` exposes commit and rollback operations.

The operation interfaces are provider agnostic. Register each provider's operation implementations with scoped lifetime.

## Tenant context

`TenantContext` carries the current tenant identifier for persistence implementations. Tenant filtering and tenant assignment belong in the tenant aware operation implementation so each operation has explicit behavior.
