using FluentResults;

namespace NKF.Domain.SharedKernels.Results;

public static class ErrorTypeExtensions
{
    public static ErrorType GetErrorType(IError error)
    {
        if (error is DomainError de)
        {
            return de.ErrorType;
        }

        if (error.Metadata.TryGetValue("ErrorType", out var typeObj)
            && Enum.TryParse<ErrorType>(typeObj?.ToString(), out var parsed))
        {
            return parsed;
        }

        if (error.Metadata.TryGetValue("Type", out var legacyTypeObj)
            && Enum.TryParse<ErrorType>(legacyTypeObj?.ToString(), out var legacyParsed))
        {
            return legacyParsed;
        }

        return ErrorType.Unexpected;
    }

    public static int GetHttpStatusCode(this IEnumerable<IError> errors)
    {
        var errorList = (errors ?? Enumerable.Empty<IError>()).ToList();
        if (errorList.Count == 0)
        {
            return 500;
        }

        return errorList
            .Select(GetErrorType)
            .Select(t => ErrorTypeStatusCodeMap.GetStatusCode(t))
            .Max();
    }
}
