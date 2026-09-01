using MediatR;
using Microsoft.Extensions.Logging;

namespace NKF.Application.Base.MediatorPipelines;


public class ExeptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExeptionBehaviour<TRequest, TResponse>> _logger;

    public ExeptionBehaviour(ILogger<ExeptionBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request {Request} failed with exception {Message}", typeof(TRequest).Name, ex.Message);
            throw;
        }
    }
}
