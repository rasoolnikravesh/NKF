using FluentResults;
using NKF.Resources.Messages;

namespace NKF.Domain.SharedKernels.Errors;

public static class DomainErrors
{
    public static Error Required(string fieldName, string displayName)
    {
        string message = string.Format(FrameWorkValidations.Required, displayName);
        Error error = CreateError(fieldName, displayName, message, "Required");
        return error;
    }

    public static Error TooShort(string fieldName, string displayName, int minLength)
    {
        string message = string.Format(FrameWorkValidations.MinLentgh, displayName, minLength);
        Error error = CreateError(fieldName, displayName, message, "TooShort");
        return error;
    }

    public static Error TooLong(string fieldName, string displayName, int maxLength)
    {
        string message = string.Format(FrameWorkValidations.MaxLength, displayName, maxLength);
        Error error = CreateError(fieldName, displayName, message, "TooLong");
        return error;
    }

    public static Error InValidCode(string fieldName, string displayName, string code)
    {
        string message = string.Format
            (FrameWorkValidations.InValidCode, displayName);
        Error error = CreateError(fieldName, displayName, message, "InValidCode");
        return error;
    }

    private static Error CreateError(string fieldName, string displayName, string message, string type)
    {
        Error error = new Error(message)
            .WithMetadata("Code", $"{fieldName}.{type}")
            .WithMetadata("FieldName", fieldName)
            .WithMetadata("DisplayName", displayName)
            .WithMetadata("Type", type);
        return error;
    }

    public static IError Duplicate(string fieldName, string displayName)
    {
        string message = string.Format(FramWorkErrorMessages.DuplicateValue, displayName);
        Error error = CreateError(fieldName, displayName, message, "Duplicate");
        return error;
    }

    public static IError MustBeDifferent(string filedName, string displayName, string otherFiledName,
        string otherDisplayName)
    {
        string message = string.Format(FrameWorkValidations.MustBeDifferent, displayName, otherFiledName);
        Error error = CreateError(filedName, displayName, message, "MustBeDifferent");
        error
            .WithMetadata("OtherFiledName", otherFiledName)
            .WithMetadata("OtherDisplayName", otherDisplayName);
        return error;
    }
}
