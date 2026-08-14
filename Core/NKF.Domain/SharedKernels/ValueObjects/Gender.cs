using FluentResults;
using NKF.Domain.SeedWorks;
using NKF.Domain.SharedKernels.Errors;
using NKF.Resources;

namespace NKF.Domain.SharedKernels.ValueObjects
{
    public class Gender : Enumeration
    {
        #region Static Member(s)

        public static readonly Gender Male = new(0, FrameWorkDataDictionary.Male);
        public static readonly Gender Female = new(1, FrameWorkDataDictionary.Female);

        public static Result<Gender> GetByValue(int? value)
        {

            if (value is null)
            {
                var error = DomainErrors.Required(nameof(FrameWorkDataDictionary.Gender),
                    FrameWorkDataDictionary.Gender);
                return Result.Fail<Gender>(error);
            }

            var gender =
                FromValue<Gender>(value: value.Value);

            if (gender is null)
            {
                var error = DomainErrors.InValidCode(nameof(FrameWorkDataDictionary.Gender),
                    FrameWorkDataDictionary.Gender, value.Value.ToString());

                return Result.Fail<Gender>(error);
            }

            return Result.Ok(gender);
        }

        #endregion /Static Member(s)

        private Gender() : base()
        {
        }

        private Gender(int value, string name) : base(value: value, name: name)
        {
        }
    }
}