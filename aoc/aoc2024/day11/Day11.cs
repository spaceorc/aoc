using System.Collections.Generic;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2024.day11;

public class Day11([NonArray] long[] input)
{
    public void Solve()
    {
        Solve(25).Out("Part 1: ");
        Solve(75).Out("Part 2: ");
    }

    private long Solve(int n)
    {
        var results = new Dictionary<(long, int), long>();
        return input.Sum(x => Count(x, n, results));
    }

    private static long Count(long x, int n, Dictionary<(long, int), long> results)
    {
        if (n == 0)
            return 1;

        if (results.TryGetValue((x, n), out var result))
            return result;

        result = x == 0
            ? Count(1, n - 1, results)
            : x.ToString() is { } s && s.Length % 2 == 0
                ? Count(long.Parse(s[..(s.Length / 2)]), n - 1, results) + Count(long.Parse(s[(s.Length / 2)..]), n - 1, results)
                : Count(x * 2024, n - 1, results);
        results[(x, n)] = result;
        return result;
    }
}
