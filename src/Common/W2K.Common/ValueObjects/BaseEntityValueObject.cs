using W2K.Common.Entities;

namespace W2K.Common.ValueObjects;

public abstract class BaseEntityValueObject : BaseEntityLogProps
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (BaseEntityValueObject)obj;
        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => (x?.GetHashCode()) ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public BaseEntityValueObject? GetCopy()
    {
        return this.MemberwiseClone() as BaseEntityValueObject;
    }

    protected static bool EqualOperator(BaseEntityValueObject left, BaseEntityValueObject right)
    {
        return !(left is null ^ right is null) && left?.Equals(right) != false;
    }

    protected static bool NotEqualOperator(BaseEntityValueObject left, BaseEntityValueObject right)
    {
        return !EqualOperator(left, right);
    }
}
