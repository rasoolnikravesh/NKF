using FluentResults;

namespace NKF.Domain.SharedKernels;


public static class ResultExtensions
{
    public static Task<Result<T>> ToResultTask<T>(this Result<T> result)
    {
        return Task.FromResult(result);
    }

    public static async Task<Result<T>> Then<T>(
        this Result<T> result,
        Func<T, Task<Result<T>>> func)
    {
        if (result.IsFailed)
        {
            return result;
        }

        return await func(result.Value);
    }

    public static async Task<Result<T>> Then<T>(
        this Task<Result<T>> task,
        Func<T, Task<Result<T>>> func)
    {
        Result<T> result = await task;

        if (result.IsFailed)
        {
            return result;
        }

        return await func(result.Value);
    }

    public static async Task<Result<T>> Then<T>(
        this Task<Result<T>> task,
        Func<T, Result<T>> func)
    {
        Result<T> result = await task;

        if (result.IsFailed)
        {
            return result;
        }

        return func(result.Value);
    }
}
