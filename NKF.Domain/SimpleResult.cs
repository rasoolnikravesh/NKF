using FluentResults;
using NKF.Domain.SharedKernels.Errors;

// using JetBrains.Annotations;

namespace NKF.Domain;

public class SimpleResult
{
    public List<SimpleError> Errors { get; set; }
    public List<string> Successes { get; set; }
    public bool IsSuccess { get; set; }

    public SimpleResult(bool isSuccess, List<SimpleError> errors, List<string> successes)
    {
        Errors = errors;
        Successes = successes;
        IsSuccess = isSuccess;
    }

}

public class SimpleError
{
    public required string Message { get; set; }
    public ErrorType ErrorType { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class SimpleResult<T>(bool isSuccess, List<SimpleError> errors, List<string> successes)
    : SimpleResult(isSuccess, errors, successes)
{
    public T? Value { get; set; }
}

public static class SimpleResultExtentions
{
    public static SimpleResult ToSimpleResult(this Result result)
    {
        return new SimpleResult(
             result.IsSuccess,
             [.. result.Errors.Select(x => ToSimpleError(x))],
             [.. result.Successes.Select(x => x.Message)]
        );
    }


    public static SimpleResult<T> ToSimpleResult<T>(this Result<T> result)
    {
        SimpleResult<T> sResult = new(result.IsSuccess,
            [.. result.Errors.Select(x => ToSimpleError(x))],
            [.. result.Successes.Select(x => x.Message)]);

        sResult.Value = result.Value;

        return sResult;

    }

    private static SimpleError ToSimpleError(IError error)
    {
        return new SimpleError
        {
            Message = error.Message,
            ErrorType = error is DomainError de ? de.ErrorType : ParseErrorType(error),
            Metadata = error.Metadata.ToDictionary(k => k.Key, v => v.Value)
        };
    }

    private static ErrorType ParseErrorType(IError error)
    {
        if (error.Metadata.TryGetValue("ErrorType", out object? typeObj) && Enum.TryParse<ErrorType>(typeObj?.ToString(), out ErrorType parsed))
        {
            return parsed;
        }

        if (error.Metadata.TryGetValue("Type", out object? typeObj2) && Enum.TryParse<ErrorType>(typeObj2?.ToString(), out ErrorType parsed2))
        {
            return parsed2;
        }

        return ErrorType.Unexpected;
    }
}
