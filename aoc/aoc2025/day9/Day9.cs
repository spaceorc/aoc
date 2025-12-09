using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day9;

public class Day9(V[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        return new[] { input, input }
            .CartesianProduct()
            .Select(vs => vs.BoundingBox())
            .Max(b => b.Area);
    }

    private long Part2()
    {
        var borderBoxes = input
            .Append(input[0])
            .SlidingWindow(2)
            .Select(seg => seg.BoundingBox())
            .ToArray();

        return new[] { input, input }
            .CartesianProduct()
            .Select(vs => vs.BoundingBox())
            .Where(b => !borderBoxes.Any(bb => bb.Intersects(b.Shrink(1))))
            .Where(b => !b.Shrink(1).TopLeft.IsInsidePolygon(input))
            .Max(b => b.Area);
    }
}
