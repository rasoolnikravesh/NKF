namespace NKF.Domain.SeedWorks;

public abstract class EntityId : ValueObject
{
    public Guid Value { get; }

    protected EntityId()
    {
    }

    protected EntityId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public bool IsEmpty()
    {
        return Value == Guid.Empty;
    }

    public static bool operator ==(EntityId? left, EntityId? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(EntityId? left, EntityId? right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
