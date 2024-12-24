using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day21;

public class Day21(string[] input)
{
    private static readonly Map<char> numeric = Map<char>.Parse(
        """
        789
        456
        123
         0A
        """
    );

    private static readonly Map<char> directional = Map<char>.Parse(
        """
         ^A
        <v>
        """
    );

    public void Solve()
    {
        Solve(2).Out("Part 1: ");
        Solve(25).Out("Part 2: ");
    }

    private long Solve(int directionalsCount) =>
        input
            .Sum(
                code => GetAllSolutions(numeric, code)
                            .Select(s => GetSolutionLen(s, directionalsCount))
                            .Min() *
                        long.Parse(code.Replace("A", ""))
            );

    private static long GetSolutionLen(string solution, int depth)
    {
        var cache = new Dictionary<(string, int), long>();
        return Calc(solution, depth);

        long Calc(string s, int d)
        {
            if (d == 0)
                return s.Length;

            if (cache.TryGetValue((s, d), out var result))
                return result;

            var parts = s.Split('A').SkipLast(1).Select(x => x + "A").ToArray();
            return cache[(s, d)] = parts.Length == 1
                ? GetAllSolutions(directional, parts[0]).Select(sol => Calc(sol, d - 1)).Min()
                : parts.Select(p => Calc(p, d)).Sum();
        }
    }

    private static string[] GetAllSolutions(Map<char> keypad, string code) =>
        code.Prepend('A')
            .Zip(code, (s, e) => GetAllPaths(keypad, s, e))
            .CartesianProduct()
            .Select(p => string.Join("", p))
            .ToArray();

    private static string[] GetAllPaths(Map<char> keypad, char s, char e) =>
        Search.Bfs(
                [keypad.Single(s)],
                v => v.Area4().Where(n => keypad.Inside(n) && keypad[n] != ' ')
            )
            .ToArray() // make it not lazy, otherwise not all paths are calculated
            .Single(x => keypad[x.State] == e)
            .AllPaths()
            .ToArray()
            .Select(x => new string(x.SlidingWindow(2).Select(w => (w[1] - w[0]).ToDirChar()).Append('A').ToArray()))
            .ToArray();
}
