using FluentResults;
using NKF.Resources;

namespace NKF.Domain.SharedKernels.ValueObjects;

public class FirstName : GenericStringValueObject
{
    #region Constant(s)

    public const int MaxLength = 50;
    public const int MinLength = 2;

    #endregion /Constant(s)

    #region Static Member(s)

    public static Result<FirstName> Create(string? value)
    {
        Result valid = Validate(value, minLength: MinLength, maxLength: MaxLength,
            nameof(FrameWorkDataDictionary.FirstName),
            FrameWorkDataDictionary.FirstName);
        if (valid.IsFailed)
        {
            return Result.Fail<FirstName>(valid.Errors);
        }

        return Result.Ok(new FirstName(value!.FixFormat()));
    }

    #endregion /Static Member(s)

    #region Constructors

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private FirstName() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    private FirstName(string value) : base(value)
    {
    }

    #endregion


    public override int GetMaxLength() => MaxLength;
    public override int GetMinLength() => 0;


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
