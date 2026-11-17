namespace NKF.Domain.SharedKernels.Errors;

public static class ErrorTypeStatusCodeMap
{
    private static readonly Dictionary<ErrorType, int> Map = new()
    {
        [ErrorType.None] = 200,
        [ErrorType.NotFound] = 404,
        [ErrorType.Validation] = 400,
        [ErrorType.Conflict] = 409,
        [ErrorType.Unauthorized] = 401,
        [ErrorType.Forbidden] = 403,
        [ErrorType.Unexpected] = 500,
    };

    public static int GetStatusCode(ErrorType errorType)
    {
        return Map.TryGetValue(errorType, out var code) ? code : 500;
    }
}
