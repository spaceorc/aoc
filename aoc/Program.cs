using System;
using System.Linq;
using System.Text.RegularExpressions;
using aoc.Lib;
using aoc.ParseLib;

namespace aoc;

public static class Program
{
    private static void Main()
    {
        Runner.RunFile("input.txt", Solve_4);
        // Runner.RunFile("day3.txt", Solve_3);
        // Runner.RunFile("day2.txt", Solve_2);
        // Runner.RunFile("day1.txt", Solve_1);
    }

    private static void Solve_4(string[] input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");

        long Part1()
        {
            return 0L;
        }

        long Part2()
        {
            return 0L;
        }
    }

    private static void Solve_3(string input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");

        long Part1() =>
            new Regex(@"mul\((\d{1,3}),(\d{1,3})\)")
                .Matches(input)
                .Sum(m => long.Parse(m.Groups[1].Value) * long.Parse(m.Groups[2].Value));

        long Part2() =>
            new Regex(@"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)")
                .Matches(input)
                .Aggregate(
                    (res: 0L, enabled: true),
                    (acc, m) => m.Groups[0].Value switch
                    {
                        "do()" => (acc.res, true),
                        "don't()" => (acc.res, false),
                        _ => (acc.enabled ? acc.res + long.Parse(m.Groups[1].Value) * long.Parse(m.Groups[2].Value) : acc.res, acc.enabled),
                    }
                )
                .res;
    }

    private static void Solve_2(long[][] input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");

        bool IsSafe(long[] levels) =>
            levels.SlidingWindow(2).All(w => w[1] - w[0] is >= 1 and <= 3) ||
            levels.SlidingWindow(2).All(w => w[0] - w[1] is >= 1 and <= 3);

        bool IsSafeWithoutOneLevel(long[] levels) =>
            Enumerable.Range(0, levels.Length).Any(i => IsSafe(levels.ExceptIndex(i).ToArray()));

        long Part1() => input.Count(IsSafe);
        long Part2() => input.Count(x => IsSafeWithoutOneLevel(x) || IsSafe(x));
    }

    private static void Solve_1((long, long)[] input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");

        long Part1()
        {
            var a = input.Select(x => x.Item1).Order().ToArray();
            var b = input.Select(x => x.Item2).Order().ToArray();
            return a.Zip(b, (x, y) => Math.Abs(x - y)).Sum();
        }

        long Part2()
        {
            var counts = input.Select(x => x.Item2).ToLookup(x => x);
            return input.Sum(x => x.Item1 * counts[x.Item1].Count());
        }
    }
}
