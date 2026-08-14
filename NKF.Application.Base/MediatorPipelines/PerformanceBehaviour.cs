using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace NKF.Application.Base.MediatorPipelines;


public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
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


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        Stopwatch stopwatch = Stopwatch.StartNew();
        TResponse response = await next();
        stopwatch.Stop();
        _logger.LogInformation("Request {Request} executed in {ElapsedMilliseconds}ms", typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

        return response;
    }
}