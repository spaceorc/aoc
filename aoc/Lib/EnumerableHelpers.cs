using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public static class EnumerableHelpers
{
    public static IEnumerable<TSource> Scan<TSource>(this IEnumerable<TSource> source,
        Func<TSource, TSource, TSource> transformation)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (transformation == null) throw new ArgumentNullException(nameof(transformation));

        return ScanImpl(source, transformation, e => e.MoveNext() ? (true, e.Current) : default);
    }
    
    public static IEnumerable<TState> Scan<TSource, TState>(this IEnumerable<TSource> source,
        TState seed, Func<TState, TSource, TState> transformation)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (transformation == null) throw new ArgumentNullException(nameof(transformation));

        return ScanImpl(source, transformation, _ => (true, seed));
    }

    private static IEnumerable<TState> ScanImpl<TSource, TState>(IEnumerable<TSource> source,
        Func<TState, TSource, TState> transformation,
        Func<IEnumerator<TSource>, (bool, TState)> seeder)
    {
        using var e = source.GetEnumerator();

        var (seeded, aggregator) = seeder(e);

        if (!seeded)
            yield break;

        yield return aggregator;

        while (e.MoveNext())
        {
            aggregator = transformation(aggregator, e.Current);
            yield return aggregator;
        }
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> items)
    {
        return items.SelectMany(x => x);
    }

    public static IEnumerable<T> Generate<T>(this T start, Func<T, T> next)
    {
        while (true)
        {
            yield return start;
            start = next(start);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    /// <summary>
    /// Works only if next item is strictly determined by previous item - so, it we meet the same item again - it's a cycle
    /// </summary>
    public static T ElementAtWithCycleTrack<T>(this IEnumerable<T> items, long index, IEqualityComparer<T>? comparer = null) where T : notnull
    {
        comparer ??= EqualityComparer<T>.Default;
        var prevIndexes = new Dictionary<T, long>(comparer);
        var prevValues = new Dictionary<long, T>();
        foreach (var item in items.WithIndex())
        {
            if (item.index == index)
                return item.item;

            if (!prevIndexes.TryGetValue(item.item, out var prevIndex))
            {
                prevIndexes.Add(item.item, item.index);
                prevValues.Add(item.index, item.item);
            }
            else
            {
                var cycle = item.index - prevIndex;
                var skip = (index - item.index) % cycle;
                return prevValues[prevIndex + skip];
            }
        }

        throw new InvalidOperationException($"There is no item with index {index}");
    }

    public static IEnumerable<T[]> SlidingWindow<T>(this IEnumerable<T> items, int windowSize)
    {
        var queue = new Queue<T>(windowSize);
        foreach (var item in items)
        {
            if (queue.Count == windowSize)
                queue.Dequeue();

            queue.Enqueue(item);

            if (queue.Count == windowSize)
                yield return queue.ToArray();
        }
    }

    public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> items, int n, int startFrom = 0)
    {
        return items.Skip(startFrom).Chunk(n).Select(x => x.First());
    }

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> items)
    {
        return items.Select((v, i) => (v, i));
    }

    public static IEnumerable<T> Without<T>(this IEnumerable<T> items, params T[] values)
    {
        return items.Where(x => !values.Contains(x));
    }

    public static IEnumerable<T> ExceptIndex<T>(this IEnumerable<T> items, int index)
    {
        return items.Where((_, i) => i != index);
    }

    public static long Product<T>(this IEnumerable<T> items, Func<T, long> selector)
    {
        return items.Select(selector).Product();
    }

    public static long Product(this IEnumerable<long> items)
    {
        return items.Aggregate(1L, (a, b) => a * b);
    }

    public static int Product(this IEnumerable<int> items)
    {
        return items.Aggregate(1, (a, b) => a * b);
    }

    public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        foreach (var item in items)
        {
            yield return item;
            if (predicate(item))
                break;
        }
    }
}
