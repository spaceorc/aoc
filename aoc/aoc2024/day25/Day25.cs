using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day25;

public class Day25(params Map<char>[] input)
{
    public void Solve()
    {
        var lockCodes = input.Where(m => m[V.Zero] == '#')
            .ToArray()
            .Select(m => m.Columns().Select(c => c.Count(v => m[v] == '#') - 1).ToArray())
            .ToList();

        var keyCodes = input.Where(m => m[V.Zero] == '.')
            .ToArray()
            .Select(m => m.Columns().Select(c => c.Count(v => m[v] == '#') - 1).ToArray())
            .ToList();

        lockCodes
            .SelectMany(_ => keyCodes, (lockCode, keyCode) => (lockCode, keyCode))
            .Count(p => p.lockCode.Zip(p.keyCode, (l, k) => l + k).All(x => x <= 5))
            .Out("Part 1: ");
    }
}
