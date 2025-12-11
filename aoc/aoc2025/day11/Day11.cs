using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day11;

public class Day11((string device, string[] output)[] input)
{
    private readonly DefaultDict<string, List<string>> inverseGraph = input
        .SelectMany(x => x.output.Select(o => (from: o, to: x.device)))
        .GroupBy(x => x.from)
        .ToDictionary(g => g.Key, g => g.Select(x => x.to).ToList())
        .ToDefault([]);
    
    
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        return CountPaths("you", "out", []);
        
        long CountPaths(string from, string to, Dictionary<string, long> cache)
        {
            if (from == to)
                return 1;
            if (cache.TryGetValue(to, out var res))
                return res;
            var count = inverseGraph[to].Sum(neighbor => CountPaths(from, neighbor, cache));
            cache[to] = count;
            return count;
        }
    }

    private long Part2()
    {
        return CountPaths("svr", "out", 2, []);

        long CountPaths(string from, string to, int visited, Dictionary<(string to, int visited), long> cache)
        {
            if (from == to)
            {
                if (visited == 0)
                    return 1;
                return 0;
            }

            if (cache.TryGetValue((to, visited), out var res))
                return res;
            
            var count = 0L;
            foreach (var neighbor in inverseGraph[to])
            {
                var newVisited = to is "fft" or "dac" ? visited - 1 : visited;
                if (newVisited < 0)
                    throw new Exception("WTF?");
                count += CountPaths(from, neighbor, newVisited, cache);
            }
            cache[(to, visited)] = count;
            return count;
        }
    }
}
