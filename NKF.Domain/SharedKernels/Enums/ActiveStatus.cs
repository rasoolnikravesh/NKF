using FluentResults;
using NKF.Domain.SeedWorks;
using NKF.Resources;
using NKF.Resources.Messages;

namespace NKF.Domain.SharedKernels.Enums;

public class ActiveStatus : Enumeration
{
    #region Static Fields and Methods

    public const int MaxLength = 10;
    public static readonly ActiveStatus Active = new ActiveStatus(0, FrameWorkDataDictionary.Active);
    public static readonly ActiveStatus Deactive = new ActiveStatus(1, FrameWorkDataDictionary.Deactive);

    public static Result<ActiveStatus> GetByValue(int? value)
    {
        Result<ActiveStatus> result = new Result<ActiveStatus>();

        if (value is null)
        {
            string errorMessage = string.Format(FrameWorkValidations.Required, FrameWorkDataDictionary.ActiveStatus);
            result.WithError(errorMessage);
            return result;
        }

        ActiveStatus? activeStatus = FromValue<ActiveStatus>(value.Value);
        if (activeStatus is null)
        {
            string errorMessage = string.Format(FrameWorkValidations.InValidCode, FrameWorkDataDictionary.ActiveStatus);
            result.WithError(errorMessage);
            return result;
        }

        result.WithValue(activeStatus);
        return result;
    }

    #endregion

    #region Constructors

    private ActiveStatus() : base()
    {
    }

    private ActiveStatus(int value, string name) : base(value, name)
    {
    }

    #endregion
}
