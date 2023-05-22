namespace SwaggerDocumentationGenerator.Temp;

public class EqualsEqualityComparer<T>: IEqualityComparer<T>
{
    static EqualsEqualityComparer()
    {
        Instance = new EqualsEqualityComparer<T>();
    }

    public bool Equals(T x, T y)
    {
        return Object.Equals(x, y);
    }

    public int GetHashCode(T obj)
    {
        return Object.Equals(obj, default(T)) ? 0 : obj.GetHashCode();
    }

    public static EqualsEqualityComparer<T> Instance { get; private set; }
}