using System.Collections.Generic;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2024.day23;

public class Day23([Atom("-")] (string a, string b)[] input)
{
    public void Solve()
    {
        var graph = new Dictionary<string, List<string>>();
        foreach (var (a, b) in input)
        {
            if (!graph.ContainsKey(a))
                graph[a] = [];
            graph[a].Add(b);

            if (!graph.ContainsKey(b))
                graph[b] = [];
            graph[b].Add(a);
        }

        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1() => graph.Keys
            .Where(a => a.StartsWith('t'))
            .SelectMany(a => graph[a], (a, b) => (a, b))
            .SelectMany(ab => graph[ab.b], (ab, c) => (ab.a, ab.b, c))
            .Where(abc => graph[abc.c].Contains(abc.a))
            .Select(abc => new[] { abc.a, abc.b, abc.c }.Order().ToArray())
            .Distinct(ArrayEqualityComparer.Create<string>())
            .Count();

        string Part2()
        {
            var clique = Alg.FindMaxCliques(graph).MaxBy(x => x.Count)!;
            return string.Join(",", clique.Order());
        }
    }
}
