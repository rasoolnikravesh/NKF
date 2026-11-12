using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;

namespace NKF.Application.Base.MediatorPipelines;


public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
{
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;

    public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public ILogger Get_logger()
    {
        return _logger;
    }


    public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {

        Stopwatch stopwatch = Stopwatch.StartNew();
        TResponse response = await next(request, cancellationToken);
        stopwatch.Stop();
        _logger.LogInformation("Request {Request} executed in {ElapsedMilliseconds}ms", typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
