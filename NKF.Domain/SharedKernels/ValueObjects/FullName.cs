using FluentResults;
using NKF.Domain.SeedWorks;

namespace NKF.Domain.SharedKernels.ValueObjects;

public class FullName : ValueObject
{
    #region Static Member(s)

    public static Result<FullName>
        Create(int? gender, string? firstName, string? lastName)
    {
        List<IError> errors = new();

        // **************************************************
        Result<Gender> genderResult =
            Gender.GetByValue(value: gender);

        errors.AddRange(genderResult.Errors);
        // **************************************************

        // **************************************************
        Result<FirstName> firstNameResult =
            FirstName.Create(value: firstName);

        errors.AddRange(firstNameResult.Errors);
        // **************************************************

        // **************************************************
        Result<LastName> lastNameResult =
            LastName.Create(value: lastName);

        errors.AddRange(lastNameResult.Errors);
        // **************************************************

        if (errors.Any())
        {
            return Result.Fail<FullName>(errors);
        }

        FullName returnValue =
            new(gender: genderResult.Value,
                firstName: firstNameResult.Value, lastName: lastNameResult.Value);

        return Result.Ok(value: returnValue);
    }

    #endregion /Static Member(s)

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private FullName() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    private FullName
        (Gender gender, FirstName firstName, LastName lastName) : this()
    {
        Gender = gender;
        LastName = lastName;
        FirstName = firstName;
    }

    public LastName LastName { get; }

    public FirstName FirstName { get; }

    public virtual Gender Gender { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Gender;
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString()
    {
        string result =
            $"{Gender?.Name} {FirstName?.Value} {LastName?.Value}".Trim();

        return result;
    }
}
