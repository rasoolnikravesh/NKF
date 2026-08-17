# NKF.Domain.Events

Domain event marker interface for NKF framework.

## Features

- **IDomainEvent** — Marker interface for domain events

## Install

```bash
dotnet add package NKF.Domain.Events
```

## Usage

```csharp
public record BrandCreatedEvent(Guid BrandId, string Name, bool IsActive) : IDomainEvent;

public record BrandStatusChangedEvent(Guid BrandId, bool IsActive) : IDomainEvent;
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0