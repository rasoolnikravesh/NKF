# NKF.Domain

Core domain primitives for the NKF services.

## Entities and aggregate roots

Use the single typed model:

```csharp
public sealed class BrandId : EntityId
{
    private BrandId(Guid value) : base(value) { }
    public static BrandId New() => new(Guid.NewGuid());
    public static Result<BrandId> Create(Guid value) => Result.Ok(new BrandId(value));
}

public sealed class Brand : AggregateRoot<Brand, BrandId>
{
    private Brand() { }
    private Brand(BrandId id, Guid createdByUserId) : base(id, createdByUserId) { }
}
```

`Entity<TEntity, TId>` supplies a typed ID, audit fields, soft deletion, tenant awareness, and duplicate support. `AggregateRoot<TEntity, TId>` adds domain event tracking and `ApplyEventAsync`.

All persistent entity and aggregate IDs should use a domain-specific `EntityId` type. EF Core mappings convert the typed ID to its stored primitive value.

## Other primitives

- `ValueObject` implements component based equality.
- `Enumeration` provides type safe enumerations.
- `IDomainEvent` marks domain events.
- `ITenantAware` supports tenant assignment from persistence code.
- `DuplicateContext` maps entity IDs while copying object graphs.

## Result errors and successes

Domain and application methods return `FluentResults.Result` or `Result<T>`. Represent a
known business failure with `DomainErrors` and a meaningful successful outcome with
`DomainSuccesses`; do not encode these outcomes as thrown exceptions or untyped strings.

```csharp
// Illustrative fragment; service-specific operations and response mapping are abbreviated.
Result<SomeResponse> result = Result.Ok();
Result<SomeValueObject> valueObjectResult = SomeValueObject.Create(input);
result.WithErrors(valueObjectResult.Errors);

if (result.IsFailed)
{
    return result;
}

// Run the domain action, persistence operation, and unit-of-work commit.
// Map the response and add a success only after those steps succeed.
result.WithValue(response);
result.WithSuccess(DomainSuccesses.Created(displayName));
return result;
```

The example shows result composition; in a command handler, add a success only after the
required persistence operation and unit-of-work commit have succeeded. Use the consuming
service's localized resource value as `displayName` and field labels. The factory methods
format messages from Framework resources.

### Error construction

`DomainErrors` provides `Required`, `TooShort`, `TooLong`, `InValidCode`, `InValidField`,
`InValidNumber`, `Duplicate`, `MustBeDifferent`, `NotFound`, `Unauthorized`, `Forbidden`,
and `Unexpected`. Each factory returns a `DomainError`, a FluentResults `Error` carrying
an `ErrorType` plus structured metadata. Metadata includes a stable `Code` and relevant
field/display-name values; length errors also include the limit, and not-found errors
include the identifier. `DomainError` also stores the enum in its `ErrorType` property and
`ErrorType` metadata key; its `Type` metadata key preserves the factory-specific error code
for compatibility. Use the factory matching the semantic failure. If a needed rule is
missing, add a factory with the same localization and metadata shape rather than returning
an ad-hoc error message.

`ErrorType` values map to HTTP status codes in `ErrorTypeStatusCodeMap`: `None` → 200,
`NotFound` → 404, `Validation` → 400, `Conflict` → 409, `Unauthorized` → 401,
`Forbidden` → 403, and `Unexpected` → 500. `ErrorTypeExtensions.GetHttpStatusCode()` returns
500 for an empty error collection and chooses the numerically largest mapped status when
there are multiple errors. Errors without a recognized type are treated as `Unexpected`.

### Success construction

`DomainSuccesses` provides `Created`, `Updated`, `Deleted`, `Retrieved`, `Activated`,
`Deactivated`, and `Verified`. Each returns a FluentResults `Success` carrying a
`SuccessType`, a stable `Code`, and display-name/field metadata. `DomainSuccess` stores the
type in its `SuccessType` property and metadata key. If no `fieldName` is provided, the
factory derives the code prefix from `displayName` by removing spaces, Persian comma, and
periods.

At the HTTP boundary, `SimpleResult` preserves `Errors`, `Successes`, and `IsSuccess`;
`SimpleResult<T>` also carries `Value`. `ToSimpleResult()` copies the structured metadata
into `SimpleError`/`SimpleSuccess`. It only reads and assigns `Result<T>.Value` when the
result succeeded. Error conversion prefers a `DomainError.ErrorType`, then the `ErrorType`
metadata, and falls back to `Unexpected`; success conversion reads the `SuccessType`
metadata and falls back to `None`. The Tenant FastEndpoints post-processor maps `Created` to 201 and
`Deleted` to 204; other success types return 200. Error types use the mappings above, with
the highest numeric status selected when multiple errors are present.

Implementation references:

- `SharedKernels/Results/DomainErrors.cs` and `DomainSuccesses.cs` — localized factories
- `SharedKernels/Results/DomainError.cs` and `DomainSuccess.cs` — FluentResults metadata
- `SharedKernels/Results/ErrorTypeStatusCodeMap.cs` — error/status mapping
- `SharedKernels/Results/SimpleResults/SimpleResultExtentions.cs` — API result conversion
- [Tenant command handler example](../../Services/Tenant/Core/NKP.Services.Tenant.Application/Aggregates/Businesses/CommandHandlers/CreateBusinessCommandHandler.cs)
