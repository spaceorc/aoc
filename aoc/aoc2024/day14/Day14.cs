using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2024.day14;

public class Day14([Atom("pv=, ")] (V p, V v)[] input)
{
    private readonly V size = new(101, 103);

    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => Calc(input.Generate(Move).Take(101).Last());
    private long Part2() => input.Generate(Move).TakeUntil(ContainsChristmasTree).SkipLast(1).Count();

    private (V p, V v)[] Move((V p, V v)[] state) => state.Select(s => s with { p = (s.p + s.v).Mod(size) }).ToArray();
    private long Calc((V p, V v)[] state) => Quadrants().Product(q => state.Count(s => q.Contains(s.p)));

    private R2[] Quadrants() =>
    [
        new(V.Zero, size / 2),
        new R2(V.Zero, size / 2).ShiftX(size.X / 2 + 1),
        new R2(V.Zero, size / 2).ShiftY(size.Y / 2 + 1),
        new R2(V.Zero, size / 2).Shift(size / 2 + new V(1, 1)),
    ];

    private bool ContainsChristmasTree((V p, V v)[] state) => BuildMap(state).Dump().Contains(new string('#', 20));

    private Map<char> BuildMap((V p, V v)[] state)
    {
        var map = new Map<char>((int)size.X, (int)size.Y);
        map.Fill('.');
        foreach (var (p, _) in state)
            map[p] = '#';
        return map;
    }
}
