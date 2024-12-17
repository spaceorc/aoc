using System.Linq;
using System.Text.RegularExpressions;
using aoc.Lib;

namespace aoc.aoc2024.day3;

public class Day3(string input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() =>
        new Regex(@"mul\((\d{1,3}),(\d{1,3})\)")
            .Matches(input)
            .Sum(m => long.Parse(m.Groups[1].Value) * long.Parse(m.Groups[2].Value));

    private long Part2() =>
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
