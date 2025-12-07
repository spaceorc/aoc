using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2025.day5;

public class Day5([Atom("-")](long start, long end)[] ranges, long[] samples)
{
    private readonly R[] ranges = ranges.Select(r => R.FromStartEnd(r.start, r.end + 1)).ToArray();
    
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => samples.Count(s => ranges.Any(r => r.Contains(s)));
    private long Part2() => ranges.Pack().Sum(r => r.Len);
}
