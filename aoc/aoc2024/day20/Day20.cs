using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day20;

public class Day20(Map<char> map)
{
    public void Solve()
    {
        var start = map.All().Single(v => map[v] == 'S');
        var dist = Search.Bfs(
                [start],
                v => v.Area4().Where(n => map[n] != '#')
            )
            .ToDictionary(x => x.State, x => x.Distance);

        Cheats(2).Count(c => Profit(c) >= 100).Out("Part 1: ");
        Cheats(20).Count(c => Profit(c) >= 100).Out("Part 2: ");
        return;

        long Profit((V start, V end) cheat) => dist[cheat.end] - dist[cheat.start] - cheat.start.MDistTo(cheat.end);

        IEnumerable<(V start, V end)> Cheats(int radius) =>
            dist.Keys
                .SelectMany(s => s.MCircle(radius).Select(e => (s, e)))
                .Where(c => dist.ContainsKey(c.e));
    }
}
