using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;
using Spaceorc.Z3Wrap.Core;
using Spaceorc.Z3Wrap.Expressions.Common;
using Spaceorc.Z3Wrap.Expressions.Numerics;

namespace aoc.aoc2025.day10;

public class Day10([Template("[{diagram}] {buttons} {{{joltage}}}")] [Split("()", Target = "buttons")] (string diagram, int[][] buttons, long[] joltage)[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    long GetTargetMask(string diagram)
    {
        long target = 0;
        for (var i = 0; i < diagram.Length; i++)
            if (diagram[i] == '#')
                target |= 1L << i;
        return target;
    }

    private long Part1()
    {
        return input.Select(SolveOne).Sum();

        long GetButtonMask(int[] button)
        {
            long mask = 0;
            for (var i = 0; i < button.Length; i++)
                mask |= 1L << button[i];
            return mask;
        }

        long SolveOne((string diagram, int[][] buttons, long[] _) item)
        {
            var targetMask = GetTargetMask(item.diagram);
            var buttonMasks = item.buttons.Select(GetButtonMask).ToArray();
            var minCount = int.MaxValue;
            foreach (var variant in Combinatorics.Variants(buttonMasks.Length, 2))
            {
                long combinedMask = 0;
                for (var i = 0; i < buttonMasks.Length; i++)
                    if (variant[i] == 1)
                        combinedMask ^= buttonMasks[i];
                if (combinedMask == targetMask)
                {
                    var count = variant.Count(x => x == 1);
                    if (count < minCount)
                        minCount = count;
                }
            }

            return minCount;
        }
    }

    private long Part2()
    {
        return input.Select(SolveOne).Sum();

        long SolveOne((string _, int[][] buttons, long[] joltage) item)
        {
            using var context = new Z3Context();
            using var scope = context.SetUp();
            using var optimizer = context.CreateOptimizer();

            var joltage = item.joltage.Select(j => context.Int(j)).ToArray();
            var buttonCount = 
                new IntExpr[item.buttons.Length];
            
            var totals = joltage.Select(_ => new List<IntExpr>()).ToArray();

            for (int b = 0; b < item.buttons.Length; b++)
            {
                var x = context.IntConst("buttonCount_" + b);
                optimizer.Assert(x >= 0);
                buttonCount[b] = x;

                foreach (var j in item.buttons[b])
                    totals[j].Add(x);
            }

            for (int j = 0; j < item.joltage.Length; j++)
            {
                var sumExpr = context.Add([..totals[j]]);
                optimizer.Assert(sumExpr == joltage[j]);
            }

            var totalButtonPresses = context.Add([..buttonCount]);

            var objective = optimizer.Minimize(totalButtonPresses);
            var result = optimizer.Check();
            if (result != Z3Status.Satisfiable)
                throw new InvalidOperationException("No solution found");

            var optimalValue = optimizer.GetLower(objective);
            var model = optimizer.GetModel();
            var value = model.GetIntValue(optimalValue);
            return (long)value;
        }
    }
}
