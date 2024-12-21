using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    private static readonly Dictionary<(string, int), long> cache2 = new();

    public void Solve()
    {
        // foreach (var s in SolveFor(directional, "<A"))
        // {
        // Console.WriteLine(s);   
        // }
        // return;
        Solve(2).Out("Part 1: ");
        // Solve(3).Out("Part 1: ");
        Solve(25).Out("Part 2: ");
    }

    private long Solve(int directionals) =>
        input
            .Select(code => new { code, sequence = SolveFor(code, directionals) })
            .Select(x => x.sequence * long.Parse(x.code.Replace("A", "")))
            .Sum();

    private static long SolveFor(string code, int directionals)
    {
        var numericSolutions = GetAllSolutions(numeric, code).Distinct().ToList();

        var all = new List<long>();
        for (var ii = 0; ii < numericSolutions.Count; ii++)
        {
            var numericSolution = numericSolutions[ii];
            foreach (var current in GetAllSolutions(directional, numericSolution))
            {
                var res = CalcFor(current, directionals - 1);
                // Console.WriteLine(res);
                all.Add(res);
            }
        }

        return all.Min();
    }

    private static long CalcFor(string solution, int depth)
    {
        if (cache2.TryGetValue((solution, depth), out var cached))
            return cached;

        if (depth == 0)
            return solution.Length;

        var parts = new List<string>();
        var startIndex = 0;
        while (startIndex < solution.Length)
        {
            var endIndex = solution.IndexOf('A', startIndex);
            parts.Add(solution.Substring(startIndex, endIndex - startIndex + 1));
            startIndex = endIndex + 1;
        }

        if (parts.Count == 0)
            throw new Exception("Invalid solution");

        var res = parts.Count > 1
            ? parts.Select(x => CalcFor(x, depth)).Sum()
            : GetAllSolutions(directional, parts[0]).Select(x => CalcFor(x, depth - 1)).Min();

        cache2[(solution, depth)] = res;
        return res;
    }

    private static List<string> GetAllSolutions(Map<char> keypad, string code)
    {
        var results = new List<StringBuilder> { new() };
        foreach (var (s, e) in code.Prepend('A').Zip(code))
        {
            var solutions = GetAllPaths(keypad, s, e);
            if (solutions.Length == 1)
            {
                foreach (var result in results)
                    result.Append(solutions[0]);
            }
            else
            {
                var newResults = new List<StringBuilder>();
                foreach (var result in results)
                {
                    foreach (var solution in solutions)
                    {
                        var newResult = new StringBuilder(result.ToString());
                        newResult.Append(solution);
                        newResults.Add(newResult);
                    }
                }

                results = newResults;
            }
        }

        return results.Select(x => x.ToString()).ToList();
    }

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
