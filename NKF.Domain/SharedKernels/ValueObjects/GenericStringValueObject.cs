using FluentResults;
using NKF.Domain.SeedWorks;
using NKF.Domain.SharedKernels.Errors;

namespace NKF.Domain.SharedKernels.ValueObjects;

public abstract class GenericStringValueObject : ValueObject
{
    public abstract int GetMaxLength();
    public abstract int GetMinLength();

    public string Value { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected GenericStringValueObject()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected GenericStringValueObject(string value)
    {
        Value = value;
    }


    protected static Result Validate(string? value, int minLength, int maxLength, string fieldName, string displayName)
    {
        Result result = Result.Ok();

        if (string.IsNullOrWhiteSpace(value))
        {
            return result.WithError(DomainErrors.Required(fieldName, displayName));
        }

        value = value.FixFormat();

        if (value.Length < minLength)
        {
            return result.WithError(DomainErrors.TooShort(fieldName, displayName, minLength));
        }

        if (value.Length > maxLength)
        {
            return result.WithError(DomainErrors.TooLong(fieldName, displayName, maxLength));
        }

        return result;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
