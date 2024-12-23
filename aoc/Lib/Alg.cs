using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace aoc.Lib;

public static class Alg
{
    public static int BinarySearchLowerBound(int left, int right, Func<int, bool> isGreaterThanOrEqualTo)
    {
        while (left < right)
        {
            var mid = left + (right - left) / 2;
            if (isGreaterThanOrEqualTo(mid))
                right = mid;
            else
                left = mid + 1;
        }

        return left;
    }

    public static TResult RecursiveDynProg<TArg, TResult>(TArg arg, Func<TArg, Func<TArg, TResult>, TResult> calc) where TArg : notnull
    {
        var cache = new Dictionary<TArg, TResult>();
        return Calc(arg);

        TResult Calc(TArg a)
        {
            if (cache.TryGetValue(a, out var result))
                return result;

            return cache[a] = calc(a, Calc);
        }
    }

    public static IEnumerable<(TArg Arg, TResult Result)> IncrementalDynProg<TArg, TResult>(this IEnumerable<TArg> source, Func<TArg, Func<TArg, TResult>, TResult> calc) where TArg : notnull
    {
        var cache = new Dictionary<TArg, TResult>();
        foreach (var arg in source)
        {
            cache[arg] = calc(arg, a => cache[a]);
            yield return (arg, cache[arg]);
        }
    }

    // Bron-Kerbosch algorithm with pivot
    // https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm
    // algorithm BronKerbosch2(R, P, X) is
    //     if P and X are both empty then
    //         report R as a maximal clique
    //     choose a pivot vertex u in P ⋃ X
    //     for each vertex v in P \ N(u) do
    //         BronKerbosch2(R ⋃ {v}, P ⋂ N(v), X ⋂ N(v))
    //         P := P \ {v}
    //         X := X ⋃ {v}
    public static IEnumerable<IReadOnlySet<T>> FindMaxCliques<T>(Dictionary<T, List<T>> graph) where T : notnull
    {
        var R = ImmutableHashSet<T>.Empty;
        var P = graph.Keys.ToImmutableHashSet();
        var X = ImmutableHashSet<T>.Empty;
        return BronKerboschWithPivot(graph, R, P, X);
    }

    private static IEnumerable<IReadOnlySet<T>> BronKerboschWithPivot<T>(
        Dictionary<T, List<T>> graph,
        ImmutableHashSet<T> R,
        ImmutableHashSet<T> P,
        ImmutableHashSet<T> X
    ) where T : notnull
    {
        if (P.IsEmpty && X.IsEmpty)
        {
            yield return R;
            yield break;
        }

        var pivot = P.Concat(X).MaxBy(v => graph[v].Count)!;
        var candidates = P.Except(graph[pivot]);

        foreach (var v in candidates)
        {
            var newR = R.Add(v);
            var newP = P.Intersect(graph[v]);
            var newX = X.Intersect(graph[v]);

            foreach (var clique in BronKerboschWithPivot(graph, newR, newP, newX))
                yield return clique;

            P = P.Remove(v);
            X = X.Add(v);
        }
    }
}
