using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day19;

public class Day19(string patterns, string[] designs)
{
    private readonly string[] patterns = patterns.Split(", ");

    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => designs.Count(d => GetPossibleWaysCount(d) > 0);
    private long Part2() => designs.Sum(GetPossibleWaysCount);

    private long GetPossibleWaysCount(string design) => GetPossibleWaysCount(design, 0, new Dictionary<int, long>());

    private long GetPossibleWaysCount(string design, int startFrom, Dictionary<int, long> results) =>
        startFrom == design.Length
            ? 1
            : results.TryGetValue(startFrom, out var result)
                ? result
                : results[startFrom] = patterns
                    .Where(pattern => design.AsSpan(startFrom).StartsWith(pattern))
                    .Sum(pattern => GetPossibleWaysCount(design, startFrom + pattern.Length, results));
}
