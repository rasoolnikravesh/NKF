using Mediator;
using Microsoft.Extensions.Logging;

namespace NKF.Application.Base.MediatorPipelines;


public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }



    public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        // Log the request being handled
        _logger.LogInformation($"Processing request of type {typeof(TRequest).Name}");

        // Pass control to the next handler or behavior in the pipeline
        TResponse response = await next(request, cancellationToken);

        // Log the response after processing
        _logger.LogInformation($"Completed handling request, response type: {typeof(TResponse).Name}");

        return response;
    }
}
