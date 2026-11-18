namespace NKF.Domain.SharedKernels.Results.SimpleResults;

public class SimpleError
{
    public required string Message { get; set; }
    public ErrorType ErrorType { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
