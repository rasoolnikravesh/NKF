namespace NKF.Domain.SharedKernels.Results.SimpleResults;

public class SimpleSuccess
{
    public required string Message { get; set; }
    public SuccessType SuccessType { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}