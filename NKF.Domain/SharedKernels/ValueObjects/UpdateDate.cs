using FluentResults;
using NKF.Domain.SeedWorks;

namespace NKF.Domain.SharedKernels.ValueObjects;

public class UpdateDate : ValueObject
{
    public DateTime Value { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<UpdateDate> CreateUtcNow()
    {
        Result<UpdateDate> result = new Result<UpdateDate>();
        result.WithValue(new UpdateDate() { Value = DateTime.UtcNow });
        return result;
    }

    public static Result<UpdateDate> Create(DateTime value)
    {
        Result<UpdateDate> result = new Result<UpdateDate>();
        result.WithValue(new UpdateDate() { Value = value });
        return result;
    }
}
