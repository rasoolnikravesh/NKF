using NKF.Domain.SharedKernels.Errors;

namespace NKF.Domain;

public class SimpleError
{
    public required string Message { get; set; }
    public ErrorType ErrorType { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}