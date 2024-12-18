using System.Collections.Generic;
using System.Linq;
using aoc.aoc2024.day14;
using aoc.Lib;
using aoc.ParseLib;
using aoc.ParseLib.Attributes;

namespace aoc;

public static class Program
{
    private static void Main()
    {
        Runner.Run<aoc2024.day14.Day14>();
        // Runner.Run("day16.txt", Solve_16);
        // Runner.Run("day15.txt", Solve_15);
    }

    private static void Solve_16(Map<char> map)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1()
        {
            var start = map.All().Single(v => map[v] == 'S');
            var end = map.All().Single(v => map[v] == 'E');

            return Search
                .Dijkstra([new Walker(start, Dir.Right)], Next)
                .First(x => x.State.Pos == end)
                .Distance;
        }

        long Part2()
        {
            var start = map.All().Single(v => map[v] == 'S');
            var end = map.All().Single(v => map[v] == 'E');

            var search = Search
                .Dijkstra([new Walker(start, Dir.Right)], Next)
                .ToArray();

            var distance = search
                .First(x => x.State.Pos == end)
                .Distance;

            return search
                .Where(x => x.State.Pos == end && x.Distance == distance)
                .SelectMany(x => x.AllPredecessors)
                .Select(x => x.Pos)
                .Distinct()
                .Count();
        }

        IEnumerable<(Walker state, long distance)> Next(Walker walker)
        {
            yield return (walker.TurnCW(), 1000L);
            yield return (walker.TurnCCW(), 1000L);
            if (map[walker.Forward().Pos] != '#')
                yield return (walker.Forward(), 1L);
        }
    }

    private static void Solve_15(Map<char> inputMap, string moves)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1()
        {
            var map = inputMap.Clone();
            foreach (var move in moves.Where(m => m is '>' or '<' or '^' or 'v'))
            {
                Move(
                    move switch
                    {
                        '>' => V.right,
                        '<' => V.left,
                        '^' => V.up,
                        'v' => V.down,
                    }
                );
            }

            return map.All().Where(v => map[v] == 'O').Sum(v => v.X + v.Y * 100);

            void Move(V dir)
            {
                var robot = map.All().Single(v => map[v] == '@');
                var ray = map.Ray(robot, dir).TakeWhile(v => map[v] != '#').TakeUntil(v => map[v] == '.').ToArray();
                if (map[ray[^1]] != '.')
                    return;
                map[robot] = '.';
                map[robot + dir] = '@';
                if (map[ray[^1]] == '.')
                    map[ray[^1]] = 'O';
            }
        }

        long Part2()
        {
            var walls = inputMap.All().Where(v => inputMap[v] == '#').SelectMany(v => new[] { v with { X = v.X * 2 }, v with { X = v.X * 2 + 1 } }).ToHashSet();
            var robot = inputMap.All().Where(v => inputMap[v] == '@').Select(v => v with { X = v.X * 2 }).Single();
            var boxes = inputMap.All().Where(v => inputMap[v] == 'O').Select(v => v with { X = v.X * 2 }).ToHashSet();

            foreach (var move in moves.Where(m => m is '>' or '<' or '^' or 'v'))
            {
                Move(
                    move switch
                    {
                        '>' => V.right,
                        '<' => V.left,
                        '^' => V.up,
                        'v' => V.down,
                    }
                );
            }

            return boxes.Sum(v => v.X + v.Y * 100);

            V[] BoxCoords(V box) => [box, box + new V(1, 0)];
            V? BoxAt(V v) => boxes.Contains(v) ? v : boxes.Contains(v - new V(1, 0)) ? v - new V(1, 0) : null;
            bool BoxAtWall(V box) => BoxCoords(box).Any(walls.Contains);

            void Move(V dir)
            {
                if (walls.Contains(robot + dir))
                    return;
                if (BoxAt(robot + dir) is { } box)
                {
                    if (AllBoxesInDir(box, dir) is not { } boxesToMove)
                        return;
                    boxes.ExceptWith(boxesToMove);
                    boxes.UnionWith(boxesToMove.Select(v => v + dir));
                }

                robot += dir;
            }

            V[]? BoxesInDir(V box, V dir) => BoxAtWall(box + dir)
                ? null
                : BoxCoords(box + dir)
                    .Select(BoxAt)
                    .Where(b => b is not null && b != box)!
                    .ToArray<V>();

            V[]? AllBoxesInDir(V box, V dir)
            {
                var allBoxes = new List<V> { box };
                for (var i = 0; i < allBoxes.Count; i++)
                {
                    if (BoxesInDir(allBoxes[i], dir) is not { } next)
                        return null;
                    allBoxes.AddRange(next);
                }

                return allBoxes.ToArray();
            }
        }
    }
}
