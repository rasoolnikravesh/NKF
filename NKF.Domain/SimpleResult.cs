using FluentResults;

// using JetBrains.Annotations;

namespace NKF.Domain;

public class SimpleResult
{

    public List<string> Errors { get; set; }
    public List<string> Successes { get; set; }
    public bool IsSuccess { get; set; }

    public SimpleResult(bool isSuccess, List<string> errors, List<string> successes)
    {
        Errors = errors;
        Successes = successes;
        IsSuccess = isSuccess;
    }

}

public class SimpleResult<T>(bool isSuccess, List<string> errors, List<string> successes)
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
             [.. result.Errors.Select(x => x.Message)],
             [.. result.Successes.Select(x => x.Message)]
        );
    }


    public static SimpleResult<T> ToSimpleResult<T>(this Result<T> result)
    {
        SimpleResult<T> sResult = new(result.IsSuccess,
            [.. result.Errors.Select(x => x.Message)],
            [.. result.Successes.Select(x => x.Message)]);

        sResult.Value = result.Value;

        return sResult;

    }
}
