using System;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day1;

public class Day1((long, long)[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        var a = input.Select(x => x.Item1).Order().ToArray();
        var b = input.Select(x => x.Item2).Order().ToArray();
        return a.Zip(b, (x, y) => Math.Abs(x - y)).Sum();
    }

    private long Part2()
    {
        var counts = input.Select(x => x.Item2).ToLookup(x => x);
        return input.Sum(x => x.Item1 * counts[x.Item1].Count());
    }
}
