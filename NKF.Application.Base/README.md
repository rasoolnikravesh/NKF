# NKF.Application.Base

Application layer base with CQRS handlers and MediatR pipeline behaviors.

## Features

- **ICommandHandler<TCommand>** — Handles commands returning `Result`
- **ICommandHandler<TCommand, TResponse>** — Handles commands returning `Result<TResponse>`
- **IQueryHandler<TQuery, TResponse>** — Handles queries returning `Result<TResponse>`
- **Pipeline Behaviors** — Logging, Validation, Performance, Exception, Event publishing

## Install

```bash
dotnet add package NKF.Application.Base
```

## Usage

```csharp
public class CreateBrandCommandHandler : ICommandHandler<CreateBrandCommand, BrandCreatedResponse>
{
    private readonly ICommandUnitOfWork _unitOfWork;
    
    public async Task<Result<BrandCreatedResponse>> Handle(CreateBrandCommand request, CancellationToken ct)
    {
        var brand = await Brand.CreateAsync(request.Name);
        await _unitOfWork.GetCommandRepository<Brand>().AddAsync(brand.Value, ct);
        await _unitOfWork.SaveAsync(ct);
        return Result.Ok(new BrandCreatedResponse { Id = brand.Value.Id });
    }
}
```

## Register Pipeline Behaviors

```csharp
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
});
```

## Repository

[github.com/rasoolnikravesh/NKF](https://github.com/rasoolnikravesh/NKF)

## License

Apache-2.0