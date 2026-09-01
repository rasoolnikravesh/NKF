using FluentResults;

// using JetBrains.Annotations;

namespace NKF.Domain;

public class SimpleResult
{

    public List<string> Errors { get; set; }
    public List<string> Successes { get; set; }
    public bool IsSucces { get; set; }

    public SimpleResult(bool isSucces, List<string> errors, List<string> successes)
    {
        Errors = errors;
        Successes = successes;
        IsSucces = isSucces;
    }

}

public class SimpleResult<T>(bool isSucces, List<string> errors, List<string> successes)
    : SimpleResult(isSucces, errors, successes)
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
        SimpleResult<T> sResult = new SimpleResult<T>(result.IsSuccess,
            [.. result.Errors.Select(x => x.Message)],
            [.. result.Successes.Select(x => x.Message)]);

        sResult.Value = result.Value;

        return sResult;

    }
}
