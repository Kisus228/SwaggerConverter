namespace SwaggerDocumentationGenerator.Temp;

public struct Maybe<T>
{
    private T value;

    public bool HasValue { get; private set; }

    public bool IsNothing
    {
        get { return !HasValue; }
    }

    public T Value
    {
        get
        {
            if (IsNothing)
                throw new Exception("this maybe is nothing");
            return value;
        }
    }

    public Maybe<TMap> Map<TMap>(Func<T, TMap> map)
    {
        return IsNothing ? Maybe<TMap>.Nothing() : Maybe<TMap>.Just(map(Value));
    }

    public Maybe<TMap> Bind<TMap>(Func<T, Maybe<TMap>> func)
    {
        if(IsNothing)
            return Maybe<TMap>.Nothing();
        return func(Value);
    }

    public T Or(Func<T> fallback) => HasValue ? Value : fallback();
    public T Or(T fallbackValue) => HasValue ? Value : fallbackValue;

    public static Maybe<T> Just(T someValue)
    {
        return new Maybe<T>
               {
                   value = someValue,
                   HasValue = true
               };
    }

    public static Maybe<T> Nothing()
    {
        return new Maybe<T>();
    }

    public override string ToString()
    {
        return string.Format("[{0}]", IsNothing ? "Nothing" : Value == null ? "null" : Value.ToString());
    }

    public static implicit operator Maybe<T>(T value) => value.Maybe();
}