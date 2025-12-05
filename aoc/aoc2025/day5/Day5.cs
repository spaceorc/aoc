using System;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2025.day5;

public class Day5([Atom("-")]Day5.Range[] ranges, long[] samples)
{
    public record Range(long start, long end)
    {
        public bool Intersects(Range other) => start <= other.end && end >= other.start;
        public Range Merge(Range other) => new(Math.Min(start, other.start), Math.Max(end, other.end));
    }

    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        return samples.Count(IsFresh);
        
        bool IsFresh(long sample) => ranges.Any(r => sample >= r.start && sample <= r.end);
    }

    private long Part2()
    {
        while (true)
        {
            var mergedAny = false;
            for (int i = 0; i < ranges.Length; i++)
            {
                for (int j = 0; j < ranges.Length; j++)
                {
                    if (i == j || ranges[i].end == -1 || ranges[j].end == -1)
                        continue;
                    
                    if (ranges[i].Intersects(ranges[j]))
                    {
                        var merged = ranges[i].Merge(ranges[j]);
                        ranges[i] = merged;
                        ranges[j] = new Range(0, -1); // Mark as merged
                        mergedAny = true;
                    }
                }
            }
            if (!mergedAny)
                break;
        }

        return ranges.Where(r => r.end != -1).Sum(r => r.end - r.start + 1);
    }
}
