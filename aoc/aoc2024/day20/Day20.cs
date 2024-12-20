using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day20;

public class Day20(Map<char> map)
{
    public void Solve()
    {
        var start = map.All().Single(v => map[v] == 'S');
        var end = map.All().Single(v => map[v] == 'E');
        
        var distFromStart = Search.Bfs([start], v => v.Area4().Where(n => map[n] != '#')).ToDictionary(x => x.State, x => x.Distance);
        var distFromEnd = Search.Bfs([end], v => v.Area4().Where(n => map[n] != '#')).ToDictionary(x => x.State, x => x.Distance);
        var baseDist = distFromStart[end]; 
        
        Cheats(2).Count(c => baseDist - DistWithCheat(c) >= 100).Out("Part 1: ");
        Cheats(20).Count(c => baseDist - DistWithCheat(c) >= 100).Out("Part 2: ");
        return;

        long DistWithCheat((V s, V e) cheat) => distFromStart[cheat.s] + distFromEnd[cheat.e] + cheat.s.MDistTo(cheat.e);
        
        IEnumerable<(V s, V e)> Cheats(int cheatLimit) =>
            map.All()
                .Where(v => map[v] != '#')
                .SelectMany(s => map.All().Where(e => map[e] != '#' && e != s && e.MDistTo(s) <= cheatLimit).Select(e => (s, e)));
    }
}
