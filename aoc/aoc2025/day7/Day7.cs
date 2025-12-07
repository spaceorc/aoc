using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day7;

public class Day7(Map<char> map)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        var beams = map.All('S').ToArray();
        var result = 0L;
        while (true)
        {
            var next = beams.Select(x => x + V.Dir(Dir.Down)).ToArray();
            if (next.Any(n => !map.Inside(n)))
                break;
                
            var splitted = next.Where(n => map[n] == '^').ToArray();
            var unsplitted = next.Except(splitted).ToArray();
            result += splitted.Length;
            beams = unsplitted
                .Concat(
                    splitted
                        .SelectMany(n => new[]
                            {
                                n + V.Dir(Dir.Left),
                                n + V.Dir(Dir.Right),
                            }
                        )
                )
                .Distinct()
                .ToArray();
        }

        return result;
    }

    private long Part2()
    {
        var beams = map.All('S').ToDictionary(v => v, v => 1L);
        while (true)
        {
            var next = beams.Select(x => (v: x.Key + V.Dir(Dir.Down), count: x.Value)).ToArray();
            if (next.Any(n => !map.Inside(n.v)))
                break;
            
            var splitted = next.Where(n => map[n.v] == '^').ToArray();
            var unsplitted = next.Except(splitted).ToArray();
            
            var nextBeams = new Dictionary<V, long>();
            foreach (var (v, count) in unsplitted)
            {
                nextBeams.TryAdd(v, 0);
                nextBeams[v] += count;
            }
            foreach (var (v, count) in splitted)
            {
                var left = v + V.Dir(Dir.Left);
                var right = v + V.Dir(Dir.Right);
                
                nextBeams.TryAdd(left, 0);
                nextBeams[left] += count;
                
                nextBeams.TryAdd(right, 0);
                nextBeams[right] += count;
            }
            beams = nextBeams;
        }
        return beams.Values.Sum();
    }
}
