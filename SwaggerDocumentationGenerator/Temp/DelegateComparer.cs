namespace SwaggerDocumentationGenerator.Temp;

public class DelegateComparer<T>: IComparer<T>
{
    private readonly Func<T, T, int> func;

    public DelegateComparer(Func<T, T, int> func)
    {
        this.func = func;
    }

    public int Compare(T x, T y)
    {
        return func(x, y);
    }
}