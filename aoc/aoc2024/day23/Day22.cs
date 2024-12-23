using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2024.day23;

public class Day23([Atom("-")] (string a, string b)[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        var graph = new Dictionary<string, HashSet<string>>();
        foreach (var (a, b) in input)
        {
            if (!graph.ContainsKey(a))
                graph[a] = [];
            graph[a].Add(b);

            if (!graph.ContainsKey(b))
                graph[b] = [];
            graph[b].Add(a);
        }
        
        return graph.Keys
            .Where(a => a.StartsWith('t'))
            .SelectMany(a => graph[a], (a, b) => (a, b))
            .SelectMany(ab => graph[ab.b], (ab, c) => (ab.a, ab.b, c))
            .Where(abc => graph[abc.c].Contains(abc.a))
            .Select(abc => new[] { abc.a, abc.b, abc.c }.Order().ToArray())
            .Distinct(ArrayEqualityComparer.Create<string>())
            .Count();
    }

    private string Part2()
    {
        var sw = Stopwatch.StartNew();
        var graph = new Dictionary<string, List<string>>();
        foreach (var (a, b) in input)
        {
            if (StringComparer.Ordinal.Compare(a, b) < 0)
            {
                if (!graph.ContainsKey(a))
                    graph[a] = [];
                graph[a].Add(b);
            }
            else
            {
                if (!graph.ContainsKey(b))
                    graph[b] = [];
                graph[b].Add(a);
            }
        }

        foreach (var v in graph.Values.Flatten().ToArray())
        {
            if (!graph.ContainsKey(v))
                graph[v] = [];
        }

        string[] bestComponent = [];
        foreach (var start in graph.Keys.TopSort(x => graph[x]).Reverse())
        {
            if (graph[start].Count + 1 < bestComponent.Length)
                continue;
            
            var component = graph[start]
                .ToDictionary(
                    x => x,
                    _ => new HashSet<string[]>(ArrayEqualityComparer.Create<string>())
                    {
                        new[] { start },
                    }
                );
            while (true)
            {
                var nextComponent = new Dictionary<string, HashSet<string[]>>();
                
                foreach (var (a, sets) in component)
                foreach (var set in sets)
                {
                    foreach (var b in graph[a])
                    {
                        if (component.TryGetValue(b, out var prevSets) && prevSets.Contains(set))
                        {
                            if (!nextComponent.TryGetValue(b, out var nextSets))
                                nextComponent[b] = nextSets = new HashSet<string[]>(ArrayEqualityComparer.Create<string>());
                            nextSets.Add(set.Append(a).ToArray());
                        }
                    }
                }
                
                if (nextComponent.Count == 0)
                    break;
                
                component = nextComponent;
            }
            
            if (component.Count == 0)
                continue;
            
            var chosen = component.First().Value.First().Append(component.First().Key).Order().ToArray();
            if (chosen.Length > bestComponent.Length)
                bestComponent = chosen;
        }

        Console.WriteLine(sw.Elapsed);
        return string.Join(",", bestComponent);
    }
}
