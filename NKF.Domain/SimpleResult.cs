namespace NKF.Domain;

public class SimpleResult
{
    public List<SimpleError> Errors { get; set; }
    public List<SimpleSuccess> Successes { get; set; }
    public bool IsSuccess { get; set; }

    public SimpleResult(bool isSuccess, List<SimpleError> errors, List<SimpleSuccess> successes)
    {
        Errors = errors;
        Successes = successes;
        IsSuccess = isSuccess;
    }
}

public class SimpleResult<T>(bool isSuccess, List<SimpleError> errors, List<SimpleSuccess> successes)
    : SimpleResult(isSuccess, errors, successes)
{
    public T? Value { get; set; }
}