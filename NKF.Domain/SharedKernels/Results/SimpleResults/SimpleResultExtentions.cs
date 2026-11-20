using FluentResults;

namespace NKF.Domain.SharedKernels.Results.SimpleResults;

public static class SimpleResultExtentions
{
    public static SimpleResult ToSimpleResult(this Result result)
    {
        return new SimpleResult(
            result.IsSuccess,
            [.. result.Errors.Select(x => ToSimpleError(x))],
            [.. result.Successes.Select(x => ToSimpleSuccess(x))]
        );
    }


    public static SimpleResult<T> ToSimpleResult<T>(this Result<T> result)
    {
        SimpleResult<T> sResult = new(result.IsSuccess,
            [.. result.Errors.Select(x => ToSimpleError(x))],
            [.. result.Successes.Select(x => ToSimpleSuccess(x))]);

        if (result.IsSuccess)
        {
            sResult.Value = result.Value;
        }

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

    private static SimpleSuccess ToSimpleSuccess(ISuccess success)
    {
        return new SimpleSuccess
        {
            Message = success.Message,
            SuccessType = success.Metadata.TryGetValue("SuccessType", out object? typeObj) && Enum.TryParse<SuccessType>(typeObj?.ToString(), out SuccessType parsed)
                ? parsed
                : SuccessType.None,
            Metadata = success.Metadata.ToDictionary(k => k.Key, v => v.Value)
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
