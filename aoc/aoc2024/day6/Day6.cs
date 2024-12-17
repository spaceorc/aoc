using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day6;

public class Day6(Map<char> map)
{
    private readonly Walker start = new(map.All().Single(v => map[v] == '^'), Dir.Up);

    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() =>
        start
            .Walk(map, w => Next(w))
            .Select(x => x.Pos)
            .Distinct()
            .Count();

    private long Part2() =>
        start
            .Walk(map, w => Next(w))
            .Select(x => x.Pos)
            .Distinct()
            .Count(
                obstacle => start
                    .WalkWithCycleCheck(map, w => Next(w, obstacle))
                    .Last() is null
            );


    private Walker? Next(Walker w, V? obstacle = null) =>
        !w.Forward().Inside(map) ? null
        : w.Forward().Pos == obstacle || map[w.Forward().Pos] == '#' ? w.TurnCW()
        : w.Forward();
}
