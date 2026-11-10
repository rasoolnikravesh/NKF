using FluentResults;
using NKF.Domain.SeedWorks;
using NKF.Resources.Messages;

namespace NKF.Domain.SharedKernels.ValueObjects;

public class CreateDate : ValueObject
{
    public DateTime Value { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private CreateDate()
    {
    }

    private CreateDate(DateTime? dateTime)
    {
        Value = dateTime!.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static Result<CreateDate> Create(DateTime? value)
    {
        Result<CreateDate> result = new();
        if (value is null)
        {
            string errorMessage = string.Format(FrameWorkValidations.Required, "CreateDate");
            result.WithError(errorMessage);
            return result;
        }

        result.WithValue(new CreateDate(value));

        return result;
    }
}
