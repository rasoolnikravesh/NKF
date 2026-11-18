using NKF.Resources.Messages;

namespace NKF.Domain.SharedKernels.Results;

public static class DomainSuccesses
{
    public static DomainSuccess Created(string displayName, string? fieldName = null)
    {
        string codeField = fieldName ?? SanitizeForCode(displayName);
        string message = string.Format(FrameWorkSuccessMessages.Created, displayName);
        return new DomainSuccess(message, SuccessType.Created, "Created", new Dictionary<string, object>
        {
            ["Code"] = $"{codeField}.Created",
            ["FieldName"] = fieldName ?? string.Empty,
            ["DisplayName"] = displayName
        });
    }

    public static DomainSuccess Updated(string displayName, string? fieldName = null)
    {
        string codeField = fieldName ?? SanitizeForCode(displayName);
        string message = string.Format(FrameWorkSuccessMessages.Updated, displayName);
        return new DomainSuccess(message, SuccessType.Updated, "Updated", new Dictionary<string, object>
        {
            ["Code"] = $"{codeField}.Updated",
            ["FieldName"] = fieldName ?? string.Empty,
            ["DisplayName"] = displayName
        });
    }

    public static DomainSuccess Deleted(string displayName, string? fieldName = null)
    {
        string codeField = fieldName ?? SanitizeForCode(displayName);
        string message = string.Format(FrameWorkSuccessMessages.Deleted, displayName);
        return new DomainSuccess(message, SuccessType.Deleted, "Deleted", new Dictionary<string, object>
        {
            ["Code"] = $"{codeField}.Deleted",
            ["FieldName"] = fieldName ?? string.Empty,
            ["DisplayName"] = displayName
        });
    }

    public static DomainSuccess Retrieved(string displayName, string? fieldName = null)
    {
        string codeField = fieldName ?? SanitizeForCode(displayName);
        string message = string.Format(FrameWorkSuccessMessages.Retrieved, displayName);
        return new DomainSuccess(message, SuccessType.Retrieved, "Retrieved", new Dictionary<string, object>
        {
            ["Code"] = $"{codeField}.Retrieved",
            ["FieldName"] = fieldName ?? string.Empty,
            ["DisplayName"] = displayName
        });
    }

    public static DomainSuccess Activated(string displayName, string? fieldName = null)
    {
        string codeField = fieldName ?? SanitizeForCode(displayName);
        string message = string.Format(FrameWorkSuccessMessages.Activated, displayName);
        return new DomainSuccess(message, SuccessType.Activated, "Activated", new Dictionary<string, object>
        {
            ["Code"] = $"{codeField}.Activated",
            ["FieldName"] = fieldName ?? string.Empty,
            ["DisplayName"] = displayName
        });
    }

    public static DomainSuccess Deactivated(string displayName, string? fieldName = null)
    {
        string codeField = fieldName ?? SanitizeForCode(displayName);
        string message = string.Format(FrameWorkSuccessMessages.Deactivated, displayName);
        return new DomainSuccess(message, SuccessType.Deactivated, "Deactivated", new Dictionary<string, object>
        {
            ["Code"] = $"{codeField}.Deactivated",
            ["FieldName"] = fieldName ?? string.Empty,
            ["DisplayName"] = displayName
        });
    }

    public static DomainSuccess Verified(string displayName, string? fieldName = null)
    {
        string codeField = fieldName ?? SanitizeForCode(displayName);
        string message = string.Format(FrameWorkSuccessMessages.Verified, displayName);
        return new DomainSuccess(message, SuccessType.Verified, "Verified", new Dictionary<string, object>
        {
            ["Code"] = $"{codeField}.Verified",
            ["FieldName"] = fieldName ?? string.Empty,
            ["DisplayName"] = displayName
        });
    }

    private static string SanitizeForCode(string displayName)
    {
        return displayName.Replace(" ", "").Replace("،", "").Replace(".", "");
    }
}