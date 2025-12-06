using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day6;

public class Day6(string[] input)
{
    

    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        // var parsed = input.Select(l => l.Split().Select(long.Parse).ToArray()).ToArray();
        // record Problem(long[] Numbers, char Op);
        //
        // private Problem[] problems = Enumerable.Range(0, input[0].Length)
        //     .Select(i =>
        //         {
        //             /*123 328  51 64
        //                 45 64  387 23
        //                  6 98  215 314
        //                *   +   *   +  */
        //             var numbers = input.SkipLast(1).Select(row => long.Parse(row[i])).ToArray();
        //             var op = input[^1][i][0];
        //             return new Problem(numbers, op);
        //         }
        //     )
        //     .ToArray();
        
        // return problems.Select(Solve).Sum();
        //
        // static long Solve(Problem p) => p.Op switch
        // {
        //     '+' => p.Numbers.Sum(),
        //     '*' => p.Numbers.Aggregate(1L, (a, b) => a * b),
        // };
        return 0L;
    }

    private long Part2()
    {
        var operators = input[^1].Split([' '], StringSplitOptions.RemoveEmptyEntries).Select(op => op[0]).ToArray();
        var problems = operators.Select(_ => new List<(long digit, int position)>()).ToArray();
        for (int i = 0; i < input.Length - 1; i++)
        {
            var problemIndex = 0;
            var started = false;
            for (int k = 0; k < input[i].Length; k++)
            {
                if (input[i][k] == ' ')
                {
                    if (started)
                    {
                        problemIndex++;
                        started = false;
                    }
                }
                else
                {
                    started = true;
                    var digit = input[i][k] - '0';
                    problems[problemIndex].Add((digit, k));
                }
            }
        }
        
        return problems.Select((problem, i) => Solve(problem, operators[i])).Sum();
        
        long Solve(List<(long digit, int position)> problem, char op)
        {
            var numbers = problem
                .GroupBy(x => x.position, x => x.digit)
                .Select(g => long.Parse(string.Join("", g)))
                .ToArray();
            
            return op switch
            {
                '+' => numbers.Sum(),
                '*' => numbers.Aggregate(1L, (a, b) => a * b),
                _ => throw new InvalidOperationException($"Unknown operator {op}"),
            };
        }
    }
}
