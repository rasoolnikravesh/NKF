# NKF.Domain.Events

Event contracts for NKF framework.

## Features

- **IDomainEvent** — Marker interface for internal domain events
- **IIntegrationEvent** — Versionable contract interface for events published to other services

## Install

```bash
dotnet add package NKF.Domain.Events
```

## Usage

```csharp
public record BrandCreatedEvent(Guid BrandId, string Name, bool IsActive) : IDomainEvent;

public record BrandStatusChangedEvent(Guid BrandId, bool IsActive) : IDomainEvent;

public sealed record BrandCreatedV1(Guid BrandId, string Name, DateTime OccurredOn) : IIntegrationEvent;
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0
