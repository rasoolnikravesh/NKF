namespace NKF.Domain.SeedWorks;

public interface IDeepDuplicatable<out T>
{
    T Duplicate(DuplicateContext context);
}
