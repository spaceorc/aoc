using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day6;

public class Day6(Map<char> map)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1() =>
            Start()
                .Walk(map, w => Next(w))
                .Select(x => x.Pos)
                .Distinct()
                .Count();

        long Part2() =>
            Start()
                .Walk(map, w => Next(w))
                .Select(x => x.Pos)
                .Distinct()
                .Count(
                    obstacle => Start()
                        .WalkWithCycleCheck(map, w => Next(w, obstacle))
                        .Last() is null
                );


        Walker Start() => new(map.All().Single(v => map[v] == '^'), Dir.Up);

        Walker? Next(Walker w, V? obstacle = null) =>
            !w.Forward().Inside(map) ? null
            : w.Forward().Pos == obstacle || map[w.Forward().Pos] == '#' ? w.TurnCW()
            : w.Forward();
    }
}
