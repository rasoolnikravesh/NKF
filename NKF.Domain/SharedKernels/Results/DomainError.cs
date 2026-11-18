using FluentResults;

namespace NKF.Domain.SharedKernels.Results;

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
