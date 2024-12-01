using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib;

namespace aoc;

public static class Program
{
    private static void Main()
    {
        Runner.RunFile("day1.txt", Solve_1);
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
