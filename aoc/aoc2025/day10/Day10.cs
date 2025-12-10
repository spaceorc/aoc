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
            
            var buttonCounts = item
                .buttons
                .Select((_, b) => context.IntConst("buttonCount_" + b))
                .ToArray();
            
            var sums = item
                .buttons
                .SelectMany((js, bi) => js.Select(ji => (ji, bi)))
                .GroupBy(x => x.ji, x => buttonCounts[x.bi])
                .OrderBy(g => g.Key)
                .Select(g => g.Sum())
                .ToArray();
            
            var totalButtonPresses = buttonCounts.Sum();
            
            foreach (var buttonCount in buttonCounts) 
                optimizer.Assert(buttonCount >= 0);
            
            for (var j = 0; j < item.joltage.Length; j++)
                optimizer.Assert(sums[j] == item.joltage[j]);
            

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
