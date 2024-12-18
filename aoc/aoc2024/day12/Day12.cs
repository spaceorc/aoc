using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day12;

public class Day12(Map<char> map)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => Zones().Sum(z => Perimeter(z) * z.Count);
    private long Part2() => Zones().Sum(z => CornersCount(z) * z.Count);

    private static long Perimeter(HashSet<V> zone) => zone.SelectMany(v => v.Area4()).Count(v => !zone.Contains(v));
    
    private static long CornersCount(HashSet<V> zone) => WalkAround(zone).Sum(RingCornersCount);

    private static long RingCornersCount(List<V> ring) => ring.Count(
        (_, i) => V.XProd(
            ring[(i + 1) % ring.Count] - ring[(i + 0) % ring.Count],
            ring[(i + 2) % ring.Count] - ring[(i + 1) % ring.Count]) != 0
    );

    private List<HashSet<V>> Zones()
    {
        var used = new HashSet<V>();
        var list = new List<HashSet<V>>();
        foreach (var start in map.All())
        {
            if (used.Contains(start))
                continue;
            var zone = Search.Bfs(
                    [start],
                    v => v.Area4().Where(n => map.Inside(n) && map[n] == map[start])
                )
                .Select(x => x.State)
                .ToHashSet();
            used.UnionWith(zone);
            list.Add(zone);
        }

        return list;
    }

    private static List<List<V>> WalkAround(HashSet<V> zone)
    {
        var boundaries = zone.SelectMany(
                v => new[]
                {
                    (start: v, end: v + V.right),
                    (start: v + V.right, end: v + V.right + V.down),
                    (start: v + V.right + V.down, end: v + V.down),
                    (start: v + V.down, end: v),
                }
            )
            .ToHashSet();
        var boundariesToRemove = boundaries.Where(b => boundaries.Contains((b.end, b.start))).ToArray();
        foreach (var boundary in boundariesToRemove)
            boundaries.Remove(boundary);

        var startToEnd = boundaries.ToLookup(b => b.start, b => b.end);

        var rings = new List<List<V>>();
        var used = new HashSet<(V start, V end)>();

        foreach (var startGroup in startToEnd)
        {
            if (startGroup.All(n => used.Contains((startGroup.Key, n))))
                continue;

            var ring = new List<V>();
            var cur = startGroup.Key;
            while (true)
            {
                ring.Add(cur);
                var next = startToEnd[cur].First(n => !used.Contains((cur, n)));
                used.Add((cur, next));
                cur = next;
                if (cur == ring[0])
                    break;
            }

            rings.Add(ring);
        }

        return rings;
    }
}
