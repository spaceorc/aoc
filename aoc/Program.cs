using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using aoc.aoc2024.day1;
using aoc.aoc2024.day10;
using aoc.aoc2024.day11;
using aoc.aoc2024.day17;
using aoc.aoc2024.day18;
using aoc.aoc2024.day2;
using aoc.aoc2024.day3;
using aoc.aoc2024.day4;
using aoc.aoc2024.day5;
using aoc.aoc2024.day6;
using aoc.aoc2024.day7;
using aoc.aoc2024.day8;
using aoc.aoc2024.day9;
using aoc.Lib;
using aoc.ParseLib;
using aoc.ParseLib.Attributes;

namespace aoc;

public static class Program
{
    private static void Main()
    {
        Runner.Run<Day11>();
        // Runner.Run("day16.txt", Solve_16);
        // Runner.Run("day15.txt", Solve_15);
        // Runner.Run("day14.txt", Solve_14);
        // Runner.Run("day13.txt", Solve_13);
        // Runner.Run("day12.txt", Solve_12);
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

    private static void Solve_14([Atom("pv=, ")] (V p, V v)[] input)
    {
        var size = new V(101, 103);

        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1() => Calc(input.Generate(Move).Take(101).Last());
        long Part2() => input.Generate(Move).TakeUntil(ContainsChristmasTree).SkipLast(1).Count();

        (V p, V v)[] Move((V p, V v)[] state) => state.Select(s => s with { p = (s.p + s.v).Mod(size) }).ToArray();
        long Calc((V p, V v)[] state) => Quadrants().Product(q => state.Count(s => q.Contains(s.p)));

        R2[] Quadrants() =>
        [
            new R2(V.Zero, size / 2),
            new R2(V.Zero, size / 2).ShiftX(size.X / 2 + 1),
            new R2(V.Zero, size / 2).ShiftY(size.Y / 2 + 1),
            new R2(V.Zero, size / 2).Shift(size / 2 + new V(1, 1)),
        ];

        bool ContainsChristmasTree((V p, V v)[] state) => BuildMap(state).Dump().Contains(new string('#', 20));

        Map<char> BuildMap((V p, V v)[] state)
        {
            var map = new Map<char>((int)size.X, (int)size.Y);
            map.Fill('.');
            foreach (var (p, _) in state)
                map[p] = '#';
            return map;
        }
    }

    private static void Solve_13(
        [Template(
            """
            Button A: {a}
            Button B: {b}
            Prize: {target}
            """
        )]
        [Atom("XY+,=", Target = "a")]
        [Atom("XY+,=", Target = "b")]
        [Atom("XY+,=", Target = "target")]
        params (V a, V b, V target)[] input
    )
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1() => input.Sum(x => Solve(x.a, x.b, x.target));
        long Part2() => input.Sum(x => Solve(x.a, x.b, x.target + new V(10000000000000, 10000000000000)));

        long Solve(V a, V b, V target)
        {
            var m = Matrix.Rows(
                [a.X, b.X],
                [a.Y, b.Y]
            );
            var t = Matrix.Col(
                target.X,
                target.Y
            );
            if (m.Invert() is not { } mInv)
                return 0;

            var n = mInv * t;
            var na = n[0, 0];
            var nb = n[1, 0];

            if (!na.IsInt() || !nb.IsInt() || na < 0 || nb < 0)
                return 0;

            return 3 * na.ToLong() + nb.ToLong();
        }
    }

    private static void Solve_12(Map<char> map)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1() => Zones().Sum(z => Perimeter(z) * z.Count);
        long Part2() => Zones().Sum(z => CornersCount(z) * z.Count);

        long Perimeter(HashSet<V> zone) => zone.SelectMany(v => v.Area4()).Count(v => !zone.Contains(v));
        long CornersCount(HashSet<V> zone) => WalkAround(zone).Sum(RingCornersCount);

        long RingCornersCount(List<V> ring) => ring.Count(
            (v, i) => V.XProd(
                          ring[(i + 1) % ring.Count] - v,
                          ring[(i + 2) % ring.Count] - ring[(i + 1) % ring.Count]
                      ) !=
                      0
        );

        List<HashSet<V>> Zones()
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

        List<List<V>> WalkAround(HashSet<V> zone)
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
}
