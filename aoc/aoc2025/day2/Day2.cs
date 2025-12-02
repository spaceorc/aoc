using System.Collections.Generic;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2025.day2;

public class Day2([NonArray] [Atom("-,")] (long from, long to)[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private static bool IsRepeatedPattern(long id, int times)
    {
        var idString = id.ToString();
        if (idString.Length % times != 0)
            return false;
        var patternLength = idString.Length / times;
        var pattern = idString[..patternLength];
        for (var i = patternLength; i < idString.Length; i += patternLength)
        {
            if (idString.Substring(i, patternLength) != pattern)
                return false;
        }

        return true;
    }

    private IEnumerable<long> All() => input.SelectMany(x => EnumerableHelpers.RangeFromTo(x.from, x.to));

    private long Part1() =>
        All()
            .Where(id => IsRepeatedPattern(id, times: 2))
            .Sum();

    private long Part2() =>
        All()
            .Where(id => Enumerable.Range(2, id.ToString().Length - 1).Any(times => IsRepeatedPattern(id, times)))
            .Sum();
}
