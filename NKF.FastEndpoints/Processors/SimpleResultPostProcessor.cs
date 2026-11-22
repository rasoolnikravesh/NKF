using FastEndpoints;
using NKF.Domain.SharedKernels.Results.SimpleResults;

namespace NKF.FastEndpoints.Processors;

public sealed class SimpleResultPostProcessor<TRequest, TResponse>
    : IPostProcessor<TRequest, SimpleResult<TResponse>>
    where TRequest : notnull
{
    public async Task PostProcessAsync(
        IPostProcessorContext<TRequest, SimpleResult<TResponse>> context,
        CancellationToken cancellationToken)
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        SimpleResult<TResponse>? result = context.Response;
        if (result is null)
        {
            return;
        }

        int statusCode = result.IsSuccess
            ? SimpleResultHttpStatusCode.GetSuccessStatusCode(result, context.HttpContext.Response.StatusCode)
            : SimpleResultHttpStatusCode.GetErrorStatusCode(result.Errors);

        await context.HttpContext.Response.SendAsync(result, statusCode, jsonSerializerContext: null, cancellationToken);
    }
}

public sealed class SimpleResultPostProcessor<TRequest> : IPostProcessor<TRequest, SimpleResult>
    where TRequest : notnull
{
    public async Task PostProcessAsync(
        IPostProcessorContext<TRequest, SimpleResult> context,
        CancellationToken cancellationToken)
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        SimpleResult? result = context.Response;
        if (result is null)
        {
            return;
        }

        int statusCode = result.IsSuccess
            ? SimpleResultHttpStatusCode.GetSuccessStatusCode(result, context.HttpContext.Response.StatusCode)
            : SimpleResultHttpStatusCode.GetErrorStatusCode(result.Errors);

        await context.HttpContext.Response.SendAsync(result, statusCode, jsonSerializerContext: null, cancellationToken);
    }
}