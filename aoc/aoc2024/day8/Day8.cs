using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day8;

public class Day8(Map<char> map)
{
    private IEnumerable<V> Antennas => map.All().Where(v => map[v] is not '.');

    private IEnumerable<List<V>> AntennaPairs => Antennas
        .ToLookup(v => map[v])
        .SelectMany(g => g.ToArray().Combinations(2));

    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => AntennaPairs
        .SelectMany(vs => new[] { vs[0] * 2 - vs[1], vs[1] * 2 - vs[0] })
        .Where(map.Inside)
        .Distinct()
        .Count();

    private long Part2() => AntennaPairs
        .SelectMany(
            vs => Enumerable
                .Range(0, int.MaxValue)
                .Select(n => vs[1] + (vs[1] - vs[0]) * n)
                .TakeWhile(map.Inside)
                .Concat(
                    Enumerable
                        .Range(0, int.MaxValue)
                        .Select(n => vs[0] - (vs[1] - vs[0]) * n)
                        .TakeWhile(map.Inside)
                )
        )
        .Distinct()
        .Count();
}
