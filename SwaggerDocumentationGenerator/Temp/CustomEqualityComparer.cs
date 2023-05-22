namespace SwaggerDocumentationGenerator.Temp;

public class CustomEqualityComparer<T> : IEqualityComparer<T>
{
    private readonly Func<T, T, bool> equalityDelegate;

    public CustomEqualityComparer(Func<T, T, bool> equalityDelegate)
    {
        this.equalityDelegate = equalityDelegate;
    }

    public bool Equals(T x, T y)
    {
        return equalityDelegate(x, y);
    }

    public virtual int GetHashCode(T obj)
    {
        return 0;
    }
}