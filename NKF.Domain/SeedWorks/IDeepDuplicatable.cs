namespace NKF.Domain.SeedWorks;

public interface IDeepDuplicatable<T>
{
    T Duplicate(DuplicateContext context);
}
