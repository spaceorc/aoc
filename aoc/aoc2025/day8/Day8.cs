using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day8;

public class Day8(V3[] input)
{
    private readonly int iterations = input.Length > 100 ? 1000 : 10;
    
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        var distances2 = new Dictionary<(int, int), long>();
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = i + 1; j < input.Length; j++)
            {
                var dist2 = input[i].Dist2To(input[j]);
                distances2[(i, j)] = dist2;
            }
        }
        var ordered = distances2.OrderBy(x => x.Value).Select(x => x.Key).ToArray();

        var componentById = Enumerable.Range(0, input.Length).ToDictionary(x => x, x => x);
        var idsByComponent = Enumerable.Range(0, input.Length).ToDictionary(x => x, x => new HashSet<int> { x });
        
        for (int i = 0; i < iterations; i++)
        {
            var (a, b) = ordered[i];
            var compA = componentById[a];
            var compB = componentById[b];
            if (compA != compB)
            {
                // Merge components
                var (toKeep, toMerge) = idsByComponent[compA].Count >= idsByComponent[compB].Count
                    ? (compA, compB)
                    : (compB, compA);
                
                foreach (var id in idsByComponent[toMerge])
                {
                    componentById[id] = toKeep;
                    idsByComponent[toKeep].Add(id);
                }
                idsByComponent.Remove(toMerge);
            }
        }

        return idsByComponent.Select(x => x.Value.Count).OrderDescending().Take(3).Product();
    }

    private long Part2()
    {
        var distances2 = new Dictionary<(int, int), long>();
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = i + 1; j < input.Length; j++)
            {
                var dist2 = input[i].Dist2To(input[j]);
                distances2[(i, j)] = dist2;
            }
        }
        var ordered = distances2.OrderBy(x => x.Value).Select(x => x.Key).ToArray();

        var componentById = Enumerable.Range(0, input.Length).ToDictionary(x => x, x => x);
        var idsByComponent = Enumerable.Range(0, input.Length).ToDictionary(x => x, x => new HashSet<int> { x });
        
        foreach (var (a, b) in ordered)
        {
            var compA = componentById[a];
            var compB = componentById[b];
            if (compA != compB)
            {
                // Merge components
                var (toKeep, toMerge) = idsByComponent[compA].Count >= idsByComponent[compB].Count
                    ? (compA, compB)
                    : (compB, compA);
                
                foreach (var id in idsByComponent[toMerge])
                {
                    componentById[id] = toKeep;
                    idsByComponent[toKeep].Add(id);
                }
                idsByComponent.Remove(toMerge);

                if (idsByComponent.Count == 1)
                    return input[a].X * input[b].X;
            }
        }
        
        throw new Exception("Unreachable");
    }
}
