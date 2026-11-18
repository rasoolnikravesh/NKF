namespace NKF.Domain.SharedKernels.Results;

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