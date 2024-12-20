using System;
using System.Collections.Generic;

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
}
