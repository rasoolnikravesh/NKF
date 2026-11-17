using FluentResults;

namespace NKF.Domain.SharedKernels.Errors;

public enum ErrorType
{
    None = 0,
    NotFound = 1,
    Validation = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5,
    Unexpected = 6
}

public class DomainError : Error
{
    public ErrorType ErrorType { get; }

    public DomainError(string message, ErrorType errorType, string errorCode) : base(message)
    {
        ErrorType = errorType;
        WithMetadata("ErrorType", errorType.ToString());
        WithMetadata("Type", errorCode); // Specific error code for backward compatibility
    }

    public DomainError(string message, ErrorType errorType, string errorCode, Dictionary<string, object> metadata) : base(message)
    {
        ErrorType = errorType;
        foreach (KeyValuePair<string, object> kvp in metadata)
        {
            WithMetadata(kvp.Key, kvp.Value);
        }
        WithMetadata("ErrorType", errorType.ToString());
        WithMetadata("Type", errorCode); // Specific error code for backward compatibility
    }
}