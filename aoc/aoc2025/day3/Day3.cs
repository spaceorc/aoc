using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day3;

public class Day3(string[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => input.Select(line => MaxN(line, 2)).Sum();
    private long Part2() => input.Select(line => MaxN(line, 12)).Sum();

    private static long MaxN(string s, int N)
    {
        var cache = new Dictionary<(int, int), long>();
        return MaxN(s, 0, N, cache);
    }

    private static long MaxN(string s, int startIndex, int N, Dictionary<(int, int), long> cache)
    {
        if (N == 0)
            return 0;
            
        if (startIndex == s.Length - N)
            return long.Parse(s[startIndex..]);
            
        var key = (startIndex, N);
        if (cache.TryGetValue(key, out var cachedValue))
            return cachedValue;

        var result = Math.Max(
            MaxN(s, startIndex + 1, N, cache),
            (s[startIndex] - '0') * MathHelpers.Pow(10, N - 1) + MaxN(s, startIndex + 1, N - 1, cache)
        );
        cache[key] = result;
        return result;
    }
}
