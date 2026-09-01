namespace NKF.Domain.SeedWorks;

public abstract class ValueObject : object
{
    #region Static Member(s)

    public static bool operator ==(ValueObject? leftValueObject, ValueObject? rightValueObject)
    {
        switch (leftValueObject)
        {
            case null when rightValueObject is null:
                return true;
            case null:
                return false;
        }

        if (rightValueObject is null)
        {
            return false;
        }
        return leftValueObject.Equals(rightValueObject);
    }

    public static bool operator !=(ValueObject? leftValueObject, ValueObject? rightValueObject)
    {
        return !(leftValueObject == rightValueObject);
    }
    #endregion

    protected ValueObject()
    {

    }

    protected abstract IEnumerable<object?> GetEqualityComponents();
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        if (obj is not ValueObject stronglyTypedOtherObject)
        {
            return false;
        }

        bool result =
            GetEqualityComponents().
                SequenceEqual(stronglyTypedOtherObject
                    .GetEqualityComponents());

        return result;
    }

    public override int GetHashCode()
    {
        int result = GetEqualityComponents().Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
        return result;
    }
}
