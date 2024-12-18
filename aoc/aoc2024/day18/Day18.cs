using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day18;

public class Day18(V[] input)
{
    private readonly Square square = new(V.Zero, new V(70, 70));

    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => GetPathLength(1024);

    private string Part2()
    {
        var take = Search.BinarySearchLowerBound(
            left: 1024,
            right: input.Length,
            isGreaterThanOrEqualTo: take => GetPathLength(take) < 0
        );
        var result = input.Take(take).Last();
        return $"{result.X},{result.Y}";
    }

    private long GetPathLength(int take)
    {
        var positions = input.Take(take).ToHashSet(); 
        return Search
                   .Bfs(
                       [V.Zero],
                       v => v.Area4().Where(n => n.InSquare(square) && !positions.Contains(n))
                   )
                   .FirstOrDefault(s => s.State == square.BottomRight)
                   ?.Distance ??
               -1;
    }
}
