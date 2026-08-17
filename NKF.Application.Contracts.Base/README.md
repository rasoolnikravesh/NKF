# NKF.Application.Contracts.Base

CQRS contract interfaces for the NKF framework.

## Features

- **ICommand** — Marker for commands returning `Result`
- **ICommand<TResponse>** — Marker for commands returning `Result<TResponse>`
- **IQuery<TResponse>** — Marker for queries returning `Result<TResponse>`

## Install

```bash
dotnet add package NKF.Application.Contracts.Base
```

## Usage

```csharp
public record CreateBrandCommand : ICommand<BrandCreatedResponse>
{
    public string? Name { get; set; }
    [JsonIgnore] public CultureInfo? Culture { get; set; }
}

public record GetBrandQuery : IQuery<BrandResponse>
{
    public Guid Id { get; set; }
}
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0