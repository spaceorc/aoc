using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day2;

public class Day2(long[][] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private static bool IsSafe(long[] levels) =>
        levels.SlidingWindow(2).All(w => w[1] - w[0] is >= 1 and <= 3) ||
        levels.SlidingWindow(2).All(w => w[0] - w[1] is >= 1 and <= 3);

    private static bool IsSafeWithoutOneLevel(long[] levels) =>
        Enumerable.Range(0, levels.Length).Any(i => IsSafe(levels.ExceptIndex(i).ToArray()));

    private long Part1() => input.Count(IsSafe);
    private long Part2() => input.Count(x => IsSafeWithoutOneLevel(x) || IsSafe(x));
}
