using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day22;

public class Day22(long[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => input.Sum(x => Next(x, 2000));

    private long Part2()
    {
        var changeToPrice = new Dictionary<long[], long>(ArrayEqualityComparer.Create<long>()).ToDefault();

        for (var i = 0; i < input.Length; i++)
        {
            var values = input[i].Generate(Next).Take(2001).ToArray();
            var usedChanges = new HashSet<long[]>(ArrayEqualityComparer.Create<long>());
            foreach (var fiveLastPrices in values.Select(x => x % 10).SlidingWindow(5))
            {
                var change = fiveLastPrices.Zip(fiveLastPrices.Skip(1), (a, b) => b - a).ToArray();
                var price = fiveLastPrices[^1];
                if (usedChanges.Add(change))
                    changeToPrice[change] += price;
            }
        }

        return changeToPrice.Values.Max();
    }

    private static long Next(long x)
    {
        x = ((x << 6) ^ x) % 0x1000000;
        x = ((x >> 5) ^ x) % 0x1000000;
        x = ((x << 11) ^ x) % 0x1000000;
        return x;
    }

    private static long Next(long x, long n)
    {
        var cur = x;
        for (var i = 0; i < n; i++)
            cur = Next(cur);
        return cur;
    }
}
