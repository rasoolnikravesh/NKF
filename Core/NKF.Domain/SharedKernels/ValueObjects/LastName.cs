using FluentResults;
using NKF.Domain.SeedWorks;
using NKF.Resources;

namespace NKF.Domain.SharedKernels.ValueObjects
{
    public class LastName : GenericStringValueObject
    {
        #region Constant(s)

        public const int MaxLength = 50;
        public const int MinLength = 2;

        #endregion /Constant(s)

        #region Static Member(s)

        public static Result<LastName> Create(string? value)
        {
            var valid = Validate(value, minLength: MinLength, maxLength: MaxLength,
                nameof(FrameWorkDataDictionary.LastName), FrameWorkDataDictionary.LastName);

            return valid.IsFailed
                ? Result.Fail<LastName>(valid.Errors)
                : Result.Ok(new LastName(value: value!.FixFormat()));
        }

        #endregion /Static Member(s)

        #region Constructor(s)

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private LastName() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
        }

        private LastName(string value) : base(value)
        {
        }

        #endregion /Constructor(s)

        public override int GetMaxLength() => MaxLength;

        public override int GetMinLength() => MinLength;


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}