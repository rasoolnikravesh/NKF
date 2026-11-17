using FluentResults;

namespace NKF.Domain.SharedKernels.Errors;

public class DomainSuccess : Success
{
    public SuccessType SuccessType { get; }

    public DomainSuccess(string message, SuccessType successType, string successCode) : base(message)
    {
        SuccessType = successType;
        WithMetadata("SuccessType", successType.ToString());
        WithMetadata("Code", successCode);
    }

    public DomainSuccess(string message, SuccessType successType, string successCode, Dictionary<string, object> metadata) : base(message)
    {
        SuccessType = successType;
        foreach (KeyValuePair<string, object> kvp in metadata)
        {
            WithMetadata(kvp.Key, kvp.Value);
        }
        WithMetadata("SuccessType", successType.ToString());
        WithMetadata("Code", successCode);
    }
}