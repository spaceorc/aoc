using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day10;

public class Day10(Map<char> map)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        IEnumerable<V> Heads() => map.All().Where(v => map[v] == '0');
        IEnumerable<V> Slopes() => map.All().Where(v => map[v] == '9');
        IEnumerable<V> Next(V v) => v.Area4().Where(n => map.Inside(n) && map[n] == map[v] + 1);

        long Part1() => Heads()
            .Select(ScoreMap)
            .Select(score => Slopes().Count(s => score[s] != 0))
            .Sum();

        long Part2() => Heads()
            .Select(ScoreMap)
            .Select(score => Slopes().Sum(s => score[s]))
            .Sum();

        Map<int> ScoreMap(V head)
        {
            var queue = new Queue<V>();
            queue.Enqueue(head);
            var score = new Map<int>(map.sizeX, map.sizeY)
            {
                [head] = 1,
            };
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                foreach (var next in Next(cur))
                {
                    if (score[next] == 0)
                        queue.Enqueue(next);
                    score[next] += score[cur];
                }
            }

            return score;
        }
    }
}
