using System;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day12;

public class Day12(params string[][] input)
{
    private string[][] shapes = input.SkipLast(1).Select(x => x.Skip(1).ToArray()).ToArray();

    private (V size, int[] counts)[] regions = input[^1]
        .Select(x => x.Split(['x', ':', ' '], StringSplitOptions.RemoveEmptyEntries))
        .Select(x => x.Select(int.Parse).ToArray())
        .Select(x => (size: new V(x[0], x[1]), counts: x.Skip(2).ToArray()))
        .ToArray();
        
    
    public void Solve()
    {
        Part1().Out("Part 1 (and only): ");
    }

    private long Part1()
    {
        var counter = 0;
        for (int i = 0; i < regions.Length; i++)
        {
            if (TryFit(regions[i]))
            {
                counter++;
            }
        }
        return counter;
        
        bool TryFit((V size, int[] counts) region)
        {
            var box2 = new[]
            {
                4,
                5,
                4,
                6,
                6,
                6,
            };
            var bands = Enumerable.Range(0, (int)region.size.X / 3).Select(_ => region.size.Y).ToArray();
            var counts = region.counts;
            for (var s = 0; s < counts.Length; s++)
            {
                var count = counts[s];
                var left = count;
                while (left > 0)
                {
                    var found = false;
                    if (left >= 2)
                    {
                        for (int b = 0; b < bands.Length; b++)
                        {
                            if (bands[b] >= box2[s])
                            {
                                bands[b] -= box2[s];
                                left -= 2;
                                found = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int b = 0; b < bands.Length; b++)
                        {
                            if (bands[b] >= 3)
                            {
                                bands[b] -= 3;
                                left -= 1;
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                        return false;
                }
            }

            return true;
        }
    }

    private long Part2()
    {
        return 0;
    }
}
