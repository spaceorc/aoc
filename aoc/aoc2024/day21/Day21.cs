using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day21;

public class Day21(string[] input)
{
    public void Solve()
    {
        var result = 0L;
        foreach (var code in input)
        {
            var sequence = SolveFor(code);
            result += sequence.Length * long.Parse(code.Replace("A", ""));
        }

        result.Out("Part 1: ");
    }

    private static string SolveFor(string code)
    {
        var keypads = new string[]
        {
            """
            789
            456
            123
             0A
            """,

            """
             ^A
            <v>
            """,

            """
             ^A
            <v>
            """,
        };

        var current = new[] { code };
        foreach (var keypad in keypads)
        {
            current = current.SelectMany(c => SolveFor(keypad, c)).ToArray();
            current = current.GroupBy(x => x.Length).OrderBy(x => x.Key).First().ToArray();
        }

        return current.First();
    }

    private static IEnumerable<string> SolveFor(string map, string code)
    {
        var allCodes = code.Prepend('A').SlidingWindow(2).Select(w => SolveFor(map, w[0], w[1]).ToList()).ToList();
        return allCodes.Aggregate(
            new[] { "" },
            (acc, next) => acc.SelectMany(a => next.Select(n => a + n)).ToArray()
        );
    }

    private static IEnumerable<string> SolveFor(string map, char s, char e)
    {
        var keypad = Map<char>.Parse(map);
        var start = keypad.Single(s);

        var allPaths = Search.Bfs(
                [start],
                v => v.Area4().Where(n => keypad.Inside(n) && keypad[n] != ' ')
            )
            .ToArray()
            .First(x => keypad[x.State] == e)
            .AllPaths()
            .ToArray();
        return allPaths
            .Select(
                x =>
                    new string(
                        x.SlidingWindow(2)
                            .Select(w => GetDir(w[1] - w[0]))
                            .Append('A')
                            .ToArray()
                    )
            );
    }

    private static char GetDir(V v)
    {
        return v switch
        {
            { X: 0, Y: -1 } => '^',
            { X: 0, Y: 1 } => 'v',
            { X: -1, Y: 0 } => '<',
            { X: 1, Y: 0 } => '>',
            _ => throw new Exception($"Invalid direction {v}")
        };
    }
}
