using Microsoft.AspNetCore.Http;
using NKF.Domain.SharedKernels.Results;
using NKF.Domain.SharedKernels.Results.SimpleResults;

namespace NKF.FastEndpoints.Processors;

internal static class SimpleResultHttpStatusCode
{
    public static int GetSuccessStatusCode(SimpleResult result, int currentStatusCode)
    {
        if (currentStatusCode != StatusCodes.Status200OK)
        {
            return currentStatusCode;
        }

        if (result.Successes.Count == 0)
        {
            return StatusCodes.Status200OK;
        }

        return result.Successes
            .Select(success => success.SuccessType switch
            {
                SuccessType.Created => StatusCodes.Status201Created,
                SuccessType.Deleted => StatusCodes.Status204NoContent,
                SuccessType.Accepted => StatusCodes.Status202Accepted,
                _ => StatusCodes.Status200OK
            })
            .Max();
    }

    public static int GetErrorStatusCode(List<SimpleError> errors)
    {
        if (errors.Count == 0)
        {
            return StatusCodes.Status500InternalServerError;
        }

        return errors
            .Select(error => ErrorTypeStatusCodeMap.GetStatusCode(error.ErrorType))
            .Max();
    }
}