using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace SwaggerDocumentationGenerator.Temp;

public static class CoreEnumerable
{
    public static bool TrySingle<T>(this IEnumerable<T> source, Func<T, bool> filter, out T result)
    {
        return source.Where(filter).TrySingle(out result);
    }

    public static IEnumerable<T> From<T>(params T[] items)
    {
        return items;
    }

    public static bool TrySingle<T>(this IEnumerable<T> source, out T result, string context = null)
    {
        var slice = source.Take(2).ToArray();
        if (slice.Any())
        {
            result = slice.Single(context);
            return true;
        }

        result = default(T);
        return false;
    }

    public static IEnumerable<int> Numbers(int start)
    {
        var i = start;
        while (true)
            yield return i++;
    }

    public static bool TrySingle<T>(this IQueryable<T> source, Expression<Func<T, bool>> filter, out T result)
    {
        return source.Where(filter).TrySingle(out result);
    }

    public static bool TrySingle<T>(this IQueryable<T> source, out T result)
    {
        var slice = source.Take(2).ToArray();
        if (slice.Any())
        {
            result = slice.Single();
            return true;
        }

        result = default(T);
        return false;
    }

    public static T First<T>(this IEnumerable<T> source, string context)
    {
        return source.Take(1).Single(context);
    }

    public static T Single<T>(this IEnumerable<T> source, Func<T, bool> filter, string context = null)
    {
        return source.Where(filter).Single(context);
    }

    [StringFormatMethod("formatString")]
    public static T Single<T>(this IEnumerable<T> source, string formatString = null, params object[] args)
    {
        var array = source.Take(2).ToArray();
        if (array.Length < 1)
            throw new InvalidOperationException(string.Format("sequence contains no elements, {0}", string.Format(formatString ?? "", args)));
        if (array.Length > 1)
            throw new InvalidOperationException(
                string.Format("sequence contains more than one element, {0}", string.Format(formatString ?? "", args)));
        return array[0];
    }

    [StringFormatMethod("formatString")]
    public static T SingleOrDefault<T>(this IEnumerable<T> source, string formatString, params object[] args)
    {
        return SingleOrDefault(source, string.Format(formatString, args));
    }

    public static T SingleOrDefault<T>(this IEnumerable<T> source, string context)
    {
        var array = source.Take(2).ToArray();
        if (array.Length < 1)
            return default(T);
        if (array.Length > 1)
            throw new InvalidOperationException(string.Format("sequence contains more than one element, {0}", context));
        return array[0];
    }

    public static bool SafeTrySingle<T>(this IEnumerable<T> source, out T result)
    {
        var slice = source.Take(2).ToArray();
        if (slice.Length == 1)
        {
            result = slice.Single();
            return true;
        }

        result = default(T);
        return false;
    }

    public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEnumerable<T> source,
                                                                         Func<T, TKey> keySelector,
                                                                         Func<T, TValue> valueSelector,
                                                                         string context = "",
                                                                         IEqualityComparer<TKey> keyComparer = null!)
    {
        var dictionary = new Dictionary<TKey, TValue>(keyComparer);
        foreach (var el in source)
        {
            TKey key = keySelector(el);
            try
            {
                dictionary.Add(key, valueSelector(el));
            }
            catch (ArgumentException)
            {
                throw new ArgumentException(string.Format("{0}: item [{1}] was already added to the dictionary", context, key));
            }
        }

        return dictionary;
    }

    public static Dictionary<TKey, T> ToDictionary<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, string context = "")
    {
        return source.ToDictionary(keySelector, x => x, context);
    }

    public static bool SafeTrySingle<T>(this IEnumerable<T> source, Func<T, bool> filter, out T result)
    {
        return source.Where(filter).SafeTrySingle(out result);
    }

    public static bool TryFirst<T>(this IEnumerable<T> source, Func<T, bool> filter, out T result)
    {
        return source.Where(filter).TryFirst(out result);
    }

    public static bool TryFirst<T>(this IEnumerable<T> source, out T result)
    {
        var slice = source.Take(1).ToArray();
        if (slice.Length == 1)
        {
            result = slice[0];
            return true;
        }

        result = default(T);
        return false;
    }

    public static int IndexOf<T>(this IEnumerable<T> source, T value)
    {
        return IndexOf(source, value, EqualsEqualityComparer<T>.Instance);
    }

    public static IEnumerable<int> Segment(int start, int finish)
    {
        var count = finish - start + 1;
        return count > 0 ? Enumerable.Range(start, count) : Enumerable.Empty<int>();
    }

    public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int firstIndex, int lastIndex)
    {
        return source.Skip(firstIndex).Take(lastIndex - firstIndex + 1);
    }

    public static int IndexOf<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> comparer)
    {
        return source.FirstIndexOf(x => comparer.Equals(x, value));
    }

    public static int FirstIndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var index = 0;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] value)
    {
        return source.Concat(value.AsEnumerable());
    }

    public static IEnumerable<T> Union<T>(this IEnumerable<T> source, params T[] value)
    {
        return source.Union(value.AsEnumerable());
    }

    public static IEnumerable<T> MinItemsBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IComparer<TKey> comparer = null)
    {
        return MaxItemsBy(source, keySelector, new DelegateComparer<TKey>((x, y) => -(comparer ?? Comparer<TKey>.Default).Compare(x, y)));
    }

    public static IEnumerable<T> MaxItemsBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IComparer<TKey> comparer = null)
    {
        var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            return Enumerable.Empty<T>();
        var max = enumerator.Current;
        var result = new List<T>(1) { max };
        var maxKey = keySelector(max);
        var comp = comparer ?? Comparer<TKey>.Default;
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            var itemKey = keySelector(current);
            var compResult = comp.Compare(itemKey, maxKey);
            if (compResult > 0)
            {
                max = current;
                result.Clear();
                result.Add(max);
                maxKey = itemKey;
            }
            else if (compResult == 0)
                result.Add(current);
        }

        return result;
    }

    public static IEnumerable<T> Sort<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => x);
    }

    [DebuggerStepThrough]
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    [DebuggerStepThrough]
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action, CancellationToken token)
    {
        foreach (var item in source)
        {
            token.ThrowIfCancellationRequested();
            action(item);
        }
    }

    [DebuggerStepThrough]
    public static void ForAll<T>(this ParallelQuery<T> source, Action<T> action, CancellationToken token)
    {
        source.ForAll(delegate(T obj)
        {
            token.ThrowIfCancellationRequested();
            action(obj);
        });
    }

    public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
            yield return item;
        }
    }

    public static IEnumerable<T> DoAfter<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            yield return item;
            action(item);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        var i = 0;
        foreach (var item in source)
            action(item, i++);
    }

    public static string JoinStrings(this IEnumerable<string> source, string separator)
    {
        return string.Join(separator, source.ToArray());
    }

    public static string JoinStrings<T>(this IEnumerable<T> source, string separator)
    {
        return source.Select(x => x.ToString()).JoinStrings(separator);
    }

    public static string JoinNotEmpty(this IEnumerable<string> source, string separator)
    {
        return source.NotEmptyStrings().JoinStrings(separator);
    }

    public static bool IsEmpty(this IEnumerable? source)
    {
        return source == null || !source.Cast<object>().Any();
    }

    /*public static IEnumerable<TResult> OuterJoin<TSource1, TSource2, TKey, TResult>(this IEnumerable<TSource1> source1,
                                                                                    IEnumerable<TSource2> source2,
                                                                                    Func<TSource1, TKey> source1KeySelector,
                                                                                    Func<TSource2, TKey> source2KeySelector,
                                                                                    Func<TSource1, TSource2, TResult> resultSelector)
    {
        var one = source1.ToDictionary(source1KeySelector);
        var two = source2.ToDictionary(source2KeySelector);
        return one.Keys.Union(two.Keys).Select(x => resultSelector(one.GetOrDefault(x), two.GetOrDefault(x)));
    }*/

    public static IEnumerable<TResult> LeftJoin<TSource1, TSource2, TKey, TResult>(this IEnumerable<TSource1> source1,
                                                                                   IEnumerable<TSource2> source2,
                                                                                   Func<TSource1, TKey> source1KeySelector,
                                                                                   Func<TSource2, TKey> source2KeySelector,
                                                                                   Func<TSource1, TSource2> defaultSelector,
                                                                                   Func<TSource1, TSource2, TResult> resultSelector)
    {
        return source1.GroupJoin(source2, source1KeySelector, source2KeySelector, (item1, source2Group) => new { item1, source2Group })
                      .SelectMany(group => group.source2Group.DefaultIfEmpty(defaultSelector(group.item1)),
                          (sourceItem, groupItem) => resultSelector(sourceItem.item1, groupItem)).ToArray();
    }

    public static IEnumerable<TResult> LeftJoin<TSource1, TSource2, TKey, TResult>(this IEnumerable<TSource1> source1,
                                                                                   IEnumerable<TSource2> source2,
                                                                                   Func<TSource1, TKey> source1KeySelector,
                                                                                   Func<TSource2, TKey> source2KeySelector,
                                                                                   Func<TSource1, TSource2, TResult> resultSelector)
    {
        return source1.LeftJoin(source2, source1KeySelector, source2KeySelector, item => default(TSource2), resultSelector);
    }

    public static IEnumerable<TResult> ZipExact<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first,
                                                                          IEnumerable<TSecond> second,
                                                                          Func<TFirst, TSecond, TResult> resultSelector)
    {
        using (var secondEnumerator = second.GetEnumerator())
        {
            foreach (var firstItem in first)
            {
                if (!secondEnumerator.MoveNext())
                    throw new InvalidOperationException("second exhaust");
                yield return resultSelector(firstItem, secondEnumerator.Current);
            }

            if (secondEnumerator.MoveNext())
                throw new InvalidOperationException("first exhaust");
        }
    }

    public static IEnumerable<Tuple<TFirst, TSecond>> ZipExact<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second)
    {
        return first.ZipExact(second, Tuple.Create);
    }

    public static IEnumerable<KeyValuePair<TFirst, TSecond>> EagerZip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second)
    {
        return first.EagerZip(second, (x, y) => new KeyValuePair<TFirst, TSecond>(x, y));
    }

    public static IEnumerable<TPair> EagerZip<TFirst, TSecond, TPair>(this IEnumerable<TFirst> first,
                                                                      IEnumerable<TSecond> second,
                                                                      Func<TFirst, TSecond, TPair> createPair)
    {
        using (var secondEnumerator = second.GetEnumerator())
        {
            var secondExhaust = false;
            foreach (var firstItem in first)
                yield return createPair(firstItem,
                    secondExhaust || (secondExhaust = !secondEnumerator.MoveNext()) ? default(TSecond) : secondEnumerator.Current);
            while (secondEnumerator.MoveNext())
                yield return createPair(default(TFirst), secondEnumerator.Current);
        }
    }


    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
    {
        var enumerator = source.GetEnumerator();
        var queue = new Queue<T>(count + 1);
        for (var i = 0; i < count; i++)
            if (!enumerator.MoveNext())
                yield break;
            else
                queue.Enqueue(enumerator.Current);
        while (enumerator.MoveNext())
        {
            queue.Enqueue(enumerator.Current);
            yield return queue.Dequeue();
        }
    }

    public static bool StartsWith<T>(this IEnumerable<T> source, IEnumerable<T> other)
    {
        return StartsWith(source, other, EqualityComparer<T>.Default);
    }

    private static bool StartsWith<T>(this IEnumerable<T> source, IEnumerable<T> other, IEqualityComparer<T> comparer)
    {
        if (source is IList<T> && other is IList<T>)
            return (source as IList<T>).StartsWith(other as IList<T>, comparer);
        return source.EagerZip(other, (x, y) => new { x, y }).TakeWhile(pair => !comparer.Equals(pair.y, default(T)))
                     .All(pair => comparer.Equals(pair.x, pair.y));
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source)
    {
        return source.Where(x => !ReferenceEquals(x, default(T)));
    }

    public static IEnumerable<T> WithoutNull<T>(this IEnumerable<T?> source)
    {
        return source.NotNull().Select(x => x!);
    }

    public static IEnumerable<string> NotEmptyStrings(this IEnumerable<string?> source)
    {
        return source.Where(x => !string.IsNullOrEmpty(x));
    }

    public static IQueryable<T> NotNull<T>(this IQueryable<T> source) where T : class
    {
        return source.Where(x => x != null);
    }

    public static IQueryable<T> NotNull<T>(this IQueryable<T?> source) where T : struct
    {
        return source.Where(x => x.HasValue).Cast<T>();
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source) where T : struct
    {
        return source.Where(x => x.HasValue).Cast<T>();
    }

    public static ParallelQuery<T> NotNull<T>(this ParallelQuery<T> source)
    {
        return source.Where(x => !ReferenceEquals(x, default(T)));
    }

    public static object[] AsObjectArray(object source)
    {
        var type = source.GetType();
        if (!type.IsArray)
            throw new InvalidCastException();
        return type.GetElementType().IsValueType ? ((IEnumerable)source).Cast<object>().ToArray() : (object[])source;
    }

    public static object[] CastToObjectArrayOf(this IEnumerable source, Type itemType)
    {
        return (object[])source.CastToArrayOf(itemType);
    }

    public static Array CastToArrayOf(this IEnumerable source, Type itemType)
    {
        var sourceArray = source.Cast<object>().ToArray();
        var result = Array.CreateInstance(itemType, sourceArray.Length);
        Array.Copy(sourceArray, result, sourceArray.Length);
        return result;
    }

    public static HashSet<T> ToSet<T>(this IEnumerable<T> source)
    {
        return new HashSet<T>(source);
    }

    public static IEnumerable<T> Common<T>(this IEnumerable<T> enum1, IEnumerable<T> enum2)
    {
        return enum1.Common(enum2, Comparer<T>.Default);
    }

    public static IEnumerable<T> Exclude<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> equalityComparer = null)
    {
        equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        return source.Where(x => !equalityComparer.Equals(x, item));
    }

    public static IEnumerable<T> Include<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> equalityComparer = null)
    {
        equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        return source.Any(x => equalityComparer.Equals(x, item)) ? source : source.Append(item);
    }

    public static IEnumerable<T> Common<T>(this IEnumerable<T> first, IEnumerable<T> second, IComparer<T> comparer)
    {
        return first.EagerZip(second).TakeWhile(pair => comparer.Compare(pair.Key, pair.Value) == 0).Select(pair => pair.Key);
    }

    public static NameValueCollection ToNameValueCollection<T>(this IEnumerable<T> source, Func<T, string> keySelector, Func<T, string> valueSelector)
    {
        var result = new NameValueCollection();
        source.ForEach(x => result.Add(keySelector(x), valueSelector(x)));
        return result;
    }

    public static IEnumerable<TResult> AccumulativeSelect<TSource, TResult>(this IEnumerable<TSource> source,
                                                                            TResult seed,
                                                                            Func<TResult, TSource, TResult> func)
    {
        var current = seed;
        return source.Select(item => current = func(current, item));
    }

    public static IEnumerable<T> Map<T>(this IEnumerable<T> source, Func<IEnumerable<T>, IEnumerable<T>> map)
    {
        return map(source);
    }

    public static IQueryable<T> If<T>(this IQueryable<T> source, bool condition, Func<IQueryable<T>, IQueryable<T>> map)
    {
        return condition ? map(source) : source;
    }

    public static IEnumerable<T> If<T>(this IEnumerable<T> source, bool condition, Func<IEnumerable<T>, IEnumerable<T>> map)
    {
        return condition ? map(source) : source;
    }

    public static IEnumerable<T> If<T>(this IEnumerable<T> source, Func<IEnumerable<T>, bool> condition, Func<IEnumerable<T>, IEnumerable<T>> map)
    {
        return condition(source) ? map(source) : source;
    }

    public static IEnumerable<TSource1> Sieve<TSource1, TSource2, TKey>(this IEnumerable<TSource1> source1,
                                                                        IEnumerable<TSource2> source2,
                                                                        Func<TSource1, TKey> getKey1,
                                                                        Func<TSource2, TKey> getKey2)
    {
        return source1.GroupJoin(source2, getKey1, getKey2, (First, SecondItems) => new { First, SecondItems }).Where(x => x.SecondItems.Any())
                      .Select(x => x.First);
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
    {
        return source.SelectMany(x => x);
    }

    public static T Previous<T>(this IEnumerable<T> source, T item)
    {
        return source.TakeWhile(x => !ReferenceEquals(x, item)).LastOrDefault();
    }

    public static T Next<T>(this IEnumerable<T> source, T item)
    {
        return source.Reverse().Previous(item);
    }

    public static string ConvertToString(this IEnumerable<char> source)
    {
        return new string(source.ToArray());
    }

    public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool inclusive)
    {
        foreach (var item in source)
            if (predicate(item))
                yield return item;
            else
            {
                if (inclusive) yield return item;
                yield break;
            }
    }

    public static ParallelQuery<T> Parallelize<T>(this IEnumerable<T> source)
    {
        return source.AsParallel().WithDegreeOfParallelism(32);
    }

    public static IEnumerable<Tuple<TLeft, TRight>> CartesianProduct<TLeft, TRight>(this IEnumerable<TLeft> lefts, IEnumerable<TRight> rights)
    {
        return lefts.SelectMany(l => rights.Select(r => Tuple.Create(l, r)));
    }

    public static IEnumerable<T> Until<T>(this IEnumerable<T> source, Func<T, bool> stopCondition)
    {
        foreach (var item in source)
        {
            yield return item;
            if (stopCondition(item))
                yield break;
        }
    }

    public static IEnumerable<T> Extract<T>(object source) where T : class
    {
        if (source == null)
            return null;
        if (source is T)
            return Return((T)source);
        if (source is IEnumerable<T>)
            return (IEnumerable<T>)source;
        return null;
    }

    private class Grouping<TKey, TValue> : IGrouping<TKey, TValue>
    {
        private readonly IEnumerable<TValue> values;

        public Grouping(TKey key, IEnumerable<TValue> values)
        {
            this.values = values;
            Key = key;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TKey Key { get; private set; }
    }

    public static IEnumerable<IGrouping<TKey, T>> GroupSequentially<TKey, T>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : struct
    {
        return source.GroupSequentially(keySelector, (key, values) => new CoreEnumerable.Grouping<TKey, T>(key, values));
    }

    /*public static IEnumerable<T> Union<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> keySelector)
    {
        return first.Union(second, Compare.By(keySelector));
    }*/

    public static IEnumerable<TResult> GroupSequentially<T, TKey, TResult>(this IEnumerable<T> source,
                                                                           Func<T, TKey> keySelector,
                                                                           Func<TKey, IEnumerable<T>, TResult> resultSelector) where TKey : struct
    {
        return source.GroupSequentially((arg1, arg2) => Equals(keySelector(arg1), keySelector(arg2)),
            enumerable => resultSelector(keySelector(enumerable.First()), enumerable));
    }

    public static IEnumerable<TResult> GroupSequentially<T, TResult>(this IEnumerable<T> source,
                                                                     Func<T, T, bool> isAdjacent,
                                                                     Func<IEnumerable<T>, TResult> resultSelector)
    {
        var accumulator = new List<T>();
        foreach (var item in source)
            if (accumulator.IsEmpty() || isAdjacent(accumulator.Last(), item))
                accumulator.Add(item);
            else
            {
                yield return resultSelector(accumulator.ToArray());
                accumulator.Clear();
                accumulator.Add(item);
            }

        if (accumulator.Any())
            yield return resultSelector(accumulator.ToArray());
    }

    public static void Dispose(this IEnumerable source)
    {
        var disposables = source.OfType<IDisposable>().ToArray();
        foreach (var disposable in disposables)
            disposable.Dispose();
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
    {
        return source ?? Enumerable.Empty<T>();
    }

    public static decimal SumOfDecimals(this IQueryable<decimal> source)
    {
        return source.Cast<decimal?>().Sum().GetValueOrDefault();
    }

    public static IEnumerable<TItem> Distinct<TItem, TKey>(this IEnumerable<TItem> source, Func<TItem, TKey> keySelector)
    {
        return source.Distinct(Compare.By(keySelector));
    }

    public static decimal? SumOfNullables(this IEnumerable<decimal?> source)
    {
        var sum = 0m;
        foreach (var item in source)
        {
            if (!item.HasValue)
                return null;
            sum += item.Value;
        }

        return sum;
    }

    public static double? SumOfNullables(this IEnumerable<double?> source)
    {
        var sum = 0d;
        foreach (var item in source)
        {
            if (!item.HasValue)
                return null;
            sum += item.Value;
        }

        return sum;
    }

    public static IEnumerable<T> OfType<T>(this IEnumerable<T> source, Type targetType)
    {
        return source.Where(x => targetType.IsInstanceOfType(x));
    }

    public static bool All<T>(this IEnumerable<T> source, Func<T, int, bool> filter)
    {
        return !source.Where((x, i) => !filter(x, i)).Any();
    }

    /*public static T[] ShuffleWith<T>(this IEnumerable<T> items, IShuffler shuffler)
    {
        var source = items.ToArray();
        var result = new T[source.Length];
        source.CopyTo(result, 0);
        shuffler.Shuffle(result);
        return result;
    }*/

    public static bool IsEquivalentTo<T>(this IEnumerable<T> source, IEnumerable<T> other, IEqualityComparer<T> comparer = null)
    {
        var set = new HashSet<T>(source, comparer ?? EqualityComparer<T>.Default);
        set.SymmetricExceptWith(other);
        return set.IsEmpty();
    }

    public static IEnumerable<T> Generate<T>(int count, Func<T> builder)
    {
        for (var i = 0; i < count; i++)
            yield return builder();
    }

    public static IEnumerable<T> Return<T>(T item)
    {
        yield return item;
    }

    /*public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> source, int count)
    {
        return source.SplitBy((_, elCount) => elCount >= count);
    }*/

    /*public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> source,
                                                         Func<T, bool> splitPredicate,
                                                         SplitBehavior splitBehavior = SplitBehavior.KeepSeparators)
    {
        return source.SplitBy((el, _) => splitPredicate(el), splitBehavior);
    }*/

    /*public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> source,
                                                         Func<T, int, bool> splitPredicate,
                                                         SplitBehavior splitBehavior = SplitBehavior.KeepSeparators)
    {
        var list = new List<T>();
        foreach (var current in source)
        {
            var isSeparator = splitPredicate(current, list.Count);
            if (list.Count > 0 && isSeparator)
            {
                yield return list;
                list = new List<T>();
            }

            if (!isSeparator || splitBehavior == SplitBehavior.KeepSeparators)
                list.Add(current);
        }

        if (list.Count > 0)
            yield return list;
    }*/

    public static PartitionResult<T> Partition<T>(this IEnumerable<T> source, Func<T, bool> condition)
    {
        var successes = new List<T>();
        var failures = new List<T>();
        foreach (var el in source)
        {
            if (condition(el))
                successes.Add(el);
            else
                failures.Add(el);
        }

        return new PartitionResult<T>
               {
                   Success = successes,
                   Failure = failures
               };
    }

    public class PartitionResult<T>
    {
        public IEnumerable<T> Success { get; set; }
        public IEnumerable<T> Failure { get; set; }
    }

    public static TAccumulative Foldr<T, TAccumulative>(this IEnumerable<T> source, Func<T, TAccumulative, TAccumulative> foldFn, TAccumulative seed)
    {
        var arr = source.ToArray();
        var result = seed;
        for (var i = arr.Length - 1; i >= 0; --i)
            result = foldFn(arr[i], result);
        return result;
    }

    public static T Foldr<T>(this IEnumerable<T> source, Func<T, T, T> foldFn)
    {
        var arr = source.ToArray();
        switch (arr.Length)
        {
            case 0:
                return default(T);
            case 1:
                return arr[0];
            default:
                var result = foldFn(arr[arr.Length - 2], arr[arr.Length - 1]);
                for (var i = arr.Length - 3; i >= 0; --i)
                    result = foldFn(arr[i], result);
                return result;
        }
    }

    public static IEnumerable<TItem> GetCollectionByKeyOrEmpty<TKey, TItem>(this Dictionary<TKey, IEnumerable<TItem>> source, TKey key)
    {
        if (source.TryGetValue(key, out var result))
            return result;

        return Enumerable.Empty<TItem>();
    }

    public static T LastOrDie<T>(this IList<T> @this)
    {
        if (@this?.Count > 0)
            return @this[@this.Count - 1];
        throw new ArgumentException("Список пуст. Последнего элемента нет");
    }

    public static IEnumerable<T> Pipe<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var value in source)
        {
            action(value);
            yield return value;
        }
    }

    public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
    {
        var set = new HashSet<T>(comparer);
        return source.Where(v => !set.Add(v));
    }
}