using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day4;

public class Day4(Map<char> input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => Accessible().Length;

    private long Part2()
    {
        var total = 0L;
        while (true)
        {
            var toClear = Accessible();
            if (toClear.Length == 0)
                break;
            total += toClear.Length;
            input.Fill('.', toClear);
        }

        return total;
    }

    private V[] Accessible() =>
        input
            .All('@')
            .Where(v => input.Area8(v).Count(n => input[n] == '@') < 4)
            .ToArray();
}
