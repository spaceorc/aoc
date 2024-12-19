using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2024.day19;

public class Day19([NonArray] string[] patterns, string[] designs)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        Part2Alt1().Out("Part 2 alt1: ");
        Part2Alt2().Out("Part 2 alt2: ");
        Part2Alt3().Out("Part 2 alt3: ");
    }

    private long Part1() => designs.Count(d => GetWaysCount(d) > 0);
    private long Part2() => designs.Sum(GetWaysCount);
    private long Part2Alt1() => designs.Sum(GetWaysCountAlt1);
    private long Part2Alt2() => designs.Sum(GetWaysCountAlt2);
    private long Part2Alt3() => designs.Sum(GetWaysCountAlt3);
    
    private long GetWaysCount(string design)
    {
        return Get(0, new Dictionary<int, long>());
        
        long Get(int startFrom, Dictionary<int, long> results) =>
            startFrom == design.Length
                ? 1
                : results.TryGetValue(startFrom, out var result)
                    ? result
                    : results[startFrom] = patterns
                        .Where(pattern => design.AsSpan(startFrom).StartsWith(pattern))
                        .Sum(pattern => Get(startFrom + pattern.Length, results));
    }

    private long GetWaysCountAlt1(string design) =>
        Enumerable.Range(0, design.Length + 1)
            .IncrementalDynProg<int, long>(
                (prefix, getResult) => prefix == 0
                    ? 1L
                    : patterns
                        .Where(pattern => design.AsSpan(0, prefix).EndsWith(pattern))
                        .Sum(pattern => getResult(prefix - pattern.Length))
            )
            .Last();

    private long GetWaysCountAlt2(string design) =>
        Alg.RecursiveDynProg<int, long>(
            0,
            (startFrom, getResult) => startFrom == design.Length
                ? 1L
                : patterns
                    .Where(pattern => design.AsSpan(startFrom).StartsWith(pattern))
                    .Sum(pattern => getResult(startFrom + pattern.Length))
        );

    private long GetWaysCountAlt3(string design) =>
        Alg.RecursiveDynProg<int, long>(
            design.Length,
            (prefix, getResult) => prefix == 0
                ? 1L
                : patterns
                    .Where(pattern => design.AsSpan(0, prefix).EndsWith(pattern))
                    .Sum(pattern => getResult(prefix - pattern.Length))
        );
}
