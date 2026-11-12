# NKF.Application.Base

Application layer contracts for CQRS handlers and Mediator pipeline behaviors.

## Handler contracts

- `ICommandHandler<TCommand>` handles commands returning `Result`.
- `ICommandHandler<TCommand, TResponse>` handles commands returning `Result<TResponse>`.
- `IQueryHandler<TQuery, TResponse>` handles queries returning `Result<TResponse>`.

Handlers receive database capabilities through named persistence operation interfaces from the application's persistence contracts. They do not receive a repository or `DbContext`.

```csharp
public sealed class CreateBrandCommandHandler(
    IAddBrandOperation addBrand,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateBrandCommand, BrandCreatedResponse>
{
    public async ValueTask<Result<BrandCreatedResponse>> Handle(
        CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        Result<Brand> brand = await Brand.CreateAsync(request.Name);
        if (brand.IsFailed)
        {
            return Result.Fail<BrandCreatedResponse>(brand.Errors);
        }

        await addBrand.Execute(brand.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new BrandCreatedResponse(brand.Value.Id));
    }
}
```

## Pipeline registration

Mediator registration belongs in the application layer's dependency container:

```csharp
services.AddMediator(options =>
{
    options.Assemblies = [typeof(CreateBrandCommand).Assembly];
    options.PipelineBehaviors =
    [
        typeof(LoggingBehavior<,>),
        typeof(PerformanceBehaviour<,>)
    ];
});
```

## License

Apache-2.0
