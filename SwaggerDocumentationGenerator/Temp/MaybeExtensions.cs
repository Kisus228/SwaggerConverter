namespace SwaggerDocumentationGenerator.Temp;

public static class MaybeExtensions
{
    public static bool Try<T>(this Maybe<T> maybe, out T value)
    {
        if (maybe.IsNothing)
        {
            value = default(T);
            return false;
        }

        value = maybe.Value;
        return true;
    }

    public static T? GetOrNull<T>(this Maybe<T> maybe)
        where T: struct
    {
        return maybe.HasValue ? maybe.Value : (T?) null;
    }

    public static T? GetOrDefault<T>(this Maybe<T> maybe)
        where T: class
    {
        return maybe.HasValue ? maybe.Value : default;
    }

    public static Maybe<T> Maybe<T>(this T? value)
    {
        return value == null
            ? Temp.Maybe<T>.Nothing()
            : Temp.Maybe<T>.Just(value);
    }

    public static Maybe<T> Maybe<T>(this T? value)
        where T: struct
    {
        return value == null
            ? Temp.Maybe<T>.Nothing()
            : Temp.Maybe<T>.Just(value.Value);
    }

    public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> source)
    {
        return FirstOrNone(source, _ => true);
    }
    public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (var value in source)
        {
            if (predicate(value))
                return value;
        }
        return Temp.Maybe<T>.Nothing();
    }

    public static Maybe<TValue> FindOrNone<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var value) ? value : Temp.Maybe<TValue>.Nothing();
    }
}