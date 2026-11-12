# NKF Framework

NKF provides the shared domain, application, and persistence primitives used by NKP services.

## Architecture

- **Domain:** typed `Entity<TEntity, TId>` and `AggregateRoot<TEntity, TId>`, value objects, domain events, audit fields, soft deletion, and tenant awareness.
- **Application:** command/query handlers and pipeline behaviors, with Mediator used for request dispatch.
- **Persistence:** one named `Execute` operation per database action. Command and query operation contracts remain separate, grouped by aggregate and entity.
- **Save/transaction:** `IUnitOfWork` saves changes; `ITransactionManager` begins explicit transactions.
- **Messaging:** Wolverine handles domain and integration event dispatch and outbox coordination.

Repositories and repository collections are not part of the persistence model.

## Persistence operation example

```csharp
public interface IGetBrandByIdOperation
    : IQueryPersistenceOperation<Guid, Brand?>;

internal sealed class GetBrandByIdOperation(AppDbContext context)
    : QueryPersistenceOperation<AppDbContext, Guid, Brand?>(context), IGetBrandByIdOperation
{
    public override Task<Brand?> Execute(Guid id, CancellationToken cancellationToken = default)
        => Context.Set<Brand>().SingleOrDefaultAsync(x => x.Id.Value == id, cancellationToken);
}
```

Application handlers inject `IGetBrandByIdOperation` and call `Execute`. Provider projects register implementations with scoped lifetime, typically by scanning only the service's persistence operations namespace.

## Unit of work and transaction

```csharp
Brand? brand = await getBrandById.Execute(id, cancellationToken);
// Apply domain changes.
await unitOfWork.SaveChangesAsync(cancellationToken);
```

For workflows that need an explicit transaction, inject `ITransactionManager` and begin the transaction separately. The unit of work does not expose the DbContext.

## Framework projects

- `NKF.Domain`: typed domain model and shared primitives.
- `NKF.Domain.Events`: event marker contract.
- `NKF.Application.Base` and `NKF.Application.Contracts.Base`: handler, command, query, and pipeline contracts.
- `NKF.Persistence.Base`: operation, unit of work, transaction, and tenant contracts.
- `NKF.Persistence.Postgres.Base`: EF Core operation bases, unit of work, and transaction manager.
- `NKF.Resources`: shared localized resource types.
- `NKF.Domain.Tests`: shared domain test helpers.

## Typed identifiers

Each persistent entity and aggregate should have a domain-specific `EntityId` subclass. Configure EF Core conversions explicitly so the database column remains the underlying primitive (currently Guid) while application and domain code use the typed identifier.

See the individual project README files for project-specific details.
