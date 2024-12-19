using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day16;

public class Day16(Map<char> map)
{
    private readonly V end = map.All().Single(v => map[v] == 'E');
    private readonly V start = map.All().Single(v => map[v] == 'S');

    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() =>
        Search
            .Dijkstra([new Walker(start, Dir.Right)], Next)
            .First(x => x.State.Pos == end)
            .Distance;

    private long Part2()
    {
        var search = Search
            .Dijkstra([new Walker(start, Dir.Right)], Next)
            .ToArray();

        var distance = search
            .First(x => x.State.Pos == end)
            .Distance;

        return search
            .Where(x => x.State.Pos == end && x.Distance == distance)
            .SelectMany(x => x.AllPredecessors)
            .Select(x => x.Pos)
            .Distinct()
            .Count();
    }

    private IEnumerable<(Walker state, long distance)> Next(Walker walker)
    {
        yield return (walker.TurnCW(), 1000L);
        yield return (walker.TurnCCW(), 1000L);
        if (map[walker.Forward().Pos] != '#')
            yield return (walker.Forward(), 1L);
    }
}
