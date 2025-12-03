using System;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day3;

public class Day3(string[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => input.Select(line => MaxN(line, 2)).Sum();
    private long Part2() => input.Select(line => MaxN(line, 12)).Sum();

    private static long MaxN(string s, int N) =>
        Alg.RecursiveDynProg<(int startIndex, int n), long>(
            (0, N),
            (tuple, getResult) =>
            {
                var (startIndex, n) = tuple;
                if (n == 0)
                    return 0L;

                if (startIndex == s.Length - n)
                    return long.Parse(s[startIndex..]);

                return Math.Max(
                    getResult((startIndex + 1, n)),
                    (s[startIndex] - '0') * MathHelpers.Pow(10, n - 1) + getResult((startIndex + 1, n - 1))
                );
            }
        );
}
