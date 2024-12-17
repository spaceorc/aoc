using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day4;

public class Day4(Map<char> input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1()
        {
            const string pattern = "XMAS";
            return input
                .All()
                .Sum(start => V.area8.Count(d => pattern.All((c, i) => input.At(start + d * i) == c)));
        }

        long Part2()
        {
            const string pattern = "MAS";
            var allDirs = new V[][]
            {
                [new V(-1, -1), new V(0, 0), new V(1, 1)],
                [new V(-1, 1), new V(0, 0), new V(1, -1)],
                [new V(1, -1), new V(0, 0), new V(-1, 1)],
                [new V(1, 1), new V(0, 0), new V(-1, -1)],
            };
            return input
                .All()
                .Count(center => allDirs.Count(dirs => pattern.All((c, i) => input.At(center + dirs[i]) == c)) == 2);
        }
    }
}
