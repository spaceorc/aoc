using System.Collections.Generic;
using System.Linq;
using aoc.aoc2024.day15;
using aoc.Lib;
using aoc.ParseLib;

namespace aoc;

public static class Program
{
    private static void Main()
    {
        Runner.Run<Day15>();
        // Runner.Run("day16.txt", Solve_16);
    }

    private static void Solve_16(Map<char> map)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1()
        {
            var start = map.All().Single(v => map[v] == 'S');
            var end = map.All().Single(v => map[v] == 'E');

            return Search
                .Dijkstra([new Walker(start, Dir.Right)], Next)
                .First(x => x.State.Pos == end)
                .Distance;
        }

        long Part2()
        {
            var start = map.All().Single(v => map[v] == 'S');
            var end = map.All().Single(v => map[v] == 'E');

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

        IEnumerable<(Walker state, long distance)> Next(Walker walker)
        {
            yield return (walker.TurnCW(), 1000L);
            yield return (walker.TurnCCW(), 1000L);
            if (map[walker.Forward().Pos] != '#')
                yield return (walker.Forward(), 1L);
        }
    }

}
