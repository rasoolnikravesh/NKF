using NKF.Resources.Messages;

namespace NKF.Domain.SharedKernels.Results;

public static class DomainErrors
{
    public static DomainError Required(string fieldName, string displayName)
    {
        string message = string.Format(FrameWorkValidations.Required, displayName);
        return new DomainError(message, ErrorType.Validation, "Required", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.Required",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName
        });
    }

    public static DomainError TooShort(string fieldName, string displayName, int minLength)
    {
        string message = string.Format(FrameWorkValidations.MinLentgh, displayName, minLength);
        return new DomainError(message, ErrorType.Validation, "TooShort", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.TooShort",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName,
            ["MinLength"] = minLength
        });
    }

    public static DomainError TooLong(string fieldName, string displayName, int maxLength)
    {
        string message = string.Format(FrameWorkValidations.MaxLength, displayName, maxLength);
        return new DomainError(message, ErrorType.Validation, "TooLong", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.TooLong",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName,
            ["MaxLength"] = maxLength
        });
    }

    public static DomainError InValidCode(string fieldName, string displayName, string code)
    {
        string message = string.Format(FrameWorkValidations.InValidCode, displayName);
        return new DomainError(message, ErrorType.Validation, "InValidCode", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.InValidCode",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName,
            ["CodeValue"] = code
        });
    }

    public static DomainError InValidField(string fieldName, string displayName)
    {
        string message = string.Format(FrameWorkValidations.InValidFeild, displayName);
        return new DomainError(message, ErrorType.Validation, "InValidField", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.InValidField",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName
        });
    }

    public static DomainError InValidNumber(string fieldName, string displayName)
    {
        string message = string.Format(FrameWorkValidations.InValidNumber, displayName);
        return new DomainError(message, ErrorType.Validation, "InValidNumber", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.InValidNumber",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName
        });
    }

    public static DomainError Duplicate(string fieldName, string displayName)
    {
        string message = string.Format(FramWorkErrorMessages.DuplicateValue, displayName);
        return new DomainError(message, ErrorType.Conflict, "Duplicate", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.Duplicate",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName
        });
    }

    public static DomainError MustBeDifferent(string filedName, string displayName, string otherFiledName,
        string otherDisplayName)
    {
        string message = string.Format(FrameWorkValidations.MustBeDifferent, displayName, otherFiledName);
        return new DomainError(message, ErrorType.Validation, "MustBeDifferent", new Dictionary<string, object>
        {
            ["Code"] = $"{filedName}.MustBeDifferent",
            ["FieldName"] = filedName,
            ["DisplayName"] = displayName,
            ["OtherFieldName"] = otherFiledName,
            ["OtherDisplayName"] = otherDisplayName
        });
    }

    public static DomainError NotFound(string fieldName, string displayName, object? id = null)
    {
        string message = string.Format(FramWorkErrorMessages.NotFound, displayName, id);
        return new DomainError(message, ErrorType.NotFound, "NotFound", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.NotFound",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName,
            ["Id"] = id ?? string.Empty
        });
    }

    public static DomainError Unauthorized(string fieldName, string displayName)
    {
        string message = string.Format(FramWorkErrorMessages.Unauthorized, displayName);
        return new DomainError(message, ErrorType.Unauthorized, "Unauthorized", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.Unauthorized",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName
        });
    }

    public static DomainError Forbidden(string fieldName, string displayName)
    {
        string message = string.Format(FramWorkErrorMessages.Forbidden, displayName);
        return new DomainError(message, ErrorType.Forbidden, "Forbidden", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.Forbidden",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName
        });
    }

    public static DomainError Unexpected(string fieldName, string displayName, string? details = null)
    {
        string message = string.Format(FramWorkErrorMessages.Unexpected, displayName, details ?? string.Empty);
        return new DomainError(message, ErrorType.Unexpected, "Unexpected", new Dictionary<string, object>
        {
            ["Code"] = $"{fieldName}.Unexpected",
            ["FieldName"] = fieldName,
            ["DisplayName"] = displayName,
            ["Details"] = details ?? string.Empty
        });
    }
}
