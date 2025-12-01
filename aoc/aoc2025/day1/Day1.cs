using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2025.day1;

public class Day1([Template("(?<d>.)(?<dist>.*)", IsRegex = true)] (char dir, long dist)[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() =>
        input
            .Select(x => x.dir == 'L' ? -x.dist : x.dist)
            .Scan(50L, (cur, shift) => (cur + shift).Mod(100))
            .Count(x => x == 0);

    private long Part2()
    {
        var current = 50L;
        var res = 0L;
        foreach (var (dir, dist) in input)
        {
            res += dist / 100;
            var shift = (dir == 'L' ? -1 : 1) * dist.Mod(100);
            var next = current + shift;
            if (current != 0 && next is <= 0 or >= 100)
                res++;
            current = next.Mod(100);
        }
        
        return res;
    }
}
