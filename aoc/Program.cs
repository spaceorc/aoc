using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using aoc.Lib;
using aoc.ParseLib;
using aoc.ParseLib.Attributes;

namespace aoc;

public static class Program
{
    private static void Main()
    {
        Runner.RunFile("day16.txt", Solve_16);
        // Runner.RunFile("day15.txt", Solve_15);
        // Runner.RunFile("day14.txt", Solve_14);
        // Runner.RunFile("day13.txt", Solve_13);
        // Runner.RunFile("day12.txt", Solve_12);
        // Runner.RunFile("day11.txt", Solve_11);
        // Runner.RunFile("day10.txt", Solve_10);
        // Runner.RunFile("day9.txt", Solve_9);
        // Runner.RunFile("day8.txt", Solve_8);
        // Runner.RunFile("day7.txt", Solve_7);
        // Runner.RunFile("day6.txt", Solve_6);
        // Runner.RunFile("day5.txt", Solve_5);
        // Runner.RunFile("day4.txt", Solve_4);
        // Runner.RunFile("day3.txt", Solve_3);
        // Runner.RunFile("day2.txt", Solve_2);
        // Runner.RunFile("day1.txt", Solve_1);
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
                .SelectMany(x => x.AllPrevsBack())
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
                Move(move switch
                {
                    '>' => V.right,
                    '<' => V.left,
                    '^' => V.up,
                    'v' => V.down,
                });
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
                Move(move switch
                {
                    '>' => V.right,
                    '<' => V.left,
                    '^' => V.up,
                    'v' => V.down,
                });
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
                    if (AllBoxesInDir(box, dir) is not {} boxesToMove)
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
                var allBoxes = new List<V>{box};
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
        
        (V p, V v)[] Move((V p, V v)[] state) => state.Select(s => s with {p = (s.p + s.v).Mod(size)}).ToArray();
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
                      ) != 0
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

    private static void Solve_11([NonArray] long[] input)
    {
        Solve(25).Out("Part 1: ");
        Solve(75).Out("Part 2: ");
        return;

        long Solve(int n)
        {
            var results = new Dictionary<(long, int), long>();
            return input.Sum(x => Count(x, n, results));
        }

        long Count(long x, int n, Dictionary<(long, int), long> results)
        {
            if (n == 0)
                return 1;

            if (results.TryGetValue((x, n), out var result))
                return result;

            result = x == 0
                ? Count(1, n - 1, results)
                : x.ToString() is { } s && s.Length % 2 == 0
                    ? Count(long.Parse(s[..(s.Length / 2)]), n - 1, results) + Count(long.Parse(s[(s.Length / 2)..]), n - 1, results)
                    : Count(x * 2024, n - 1, results);
            results[(x, n)] = result;
            return result;
        }
    }

    private static void Solve_10(Map<char> map)
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

    private static void Solve_9(string input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1()
        {
            var data = new List<int>();
            var id = 0;
            var isFree = false;
            foreach (var c in input)
            {
                data.AddRange(isFree ? Enumerable.Repeat(-1, c - '0') : Enumerable.Repeat(id++, c - '0'));
                isFree = !isFree;
            }

            var total = data.Count(x => x != -1);
            var freeIndex = data.IndexOf(-1);
            for (var i = data.Count - 1; i >= 0; i--)
            {
                if (data[i] == -1)
                    continue;
                (data[freeIndex], data[i]) = (data[i], data[freeIndex]);
                freeIndex = data.IndexOf(-1, freeIndex + 1);
                if (freeIndex >= total)
                    break;
            }

            return data.Select((t, i) => t == -1 ? 0 : t * (long)i).Sum();
        }

        long Part2()
        {
            var freeBlocksBySize = Enumerable.Range(0, 10).Select(_ => new Heap<int>()).ToArray();

            var files = new List<(int id, int size, int pos)>();
            var id = 0;
            var isFree = false;
            var pos = 0;
            foreach (var c in input)
            {
                if (!isFree)
                    files.Add((id++, c - '0', pos));
                else if (c != '0')
                    freeBlocksBySize[c - '0'].Add(pos);

                pos += c - '0';
                isFree = !isFree;
            }

            var newFiles = new List<(int id, int size, int pos)>();
            for (var i = files.Count - 1; i >= 0; i--)
            {
                var file = files[i];

                var matchingFreeBlocks = freeBlocksBySize
                    .Select((freeBlocks, size) => (freeBlocks, size))
                    .Where(x => x.size >= file.size && x.freeBlocks.Count > 0 && x.freeBlocks.Min < file.pos)
                    .ToArray();

                if (matchingFreeBlocks.Length == 0)
                {
                    newFiles.Add(file);
                    continue;
                }

                var freeBlock = matchingFreeBlocks.MinBy(x => x.freeBlocks.Min);
                var newPos = freeBlock.freeBlocks.DeleteMin();
                newFiles.Add(file with { pos = newPos });

                if (freeBlock.size > file.size)
                    freeBlocksBySize[freeBlock.size - file.size].Add(newPos + file.size);
            }

            return newFiles.Select(f => Enumerable.Range(0, f.size).Select(i => (f.pos + i) * (long)f.id).Sum()).Sum();
        }
    }

    private static void Solve_8(Map<char> map)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        IEnumerable<V> Antennas() => map.All().Where(v => map[v] is not '.');

        IEnumerable<List<V>> AntennaPairs() => Antennas()
            .ToLookup(v => map[v])
            .SelectMany(g => g.ToArray().Combinations(2));

        long Part1() => AntennaPairs()
            .SelectMany(vs => new[] { vs[0] * 2 - vs[1], vs[1] * 2 - vs[0] })
            .Where(map.Inside)
            .Distinct()
            .Count();

        long Part2() => AntennaPairs()
            .SelectMany(
                vs => Enumerable
                    .Range(0, int.MaxValue)
                    .Select(n => vs[1] + (vs[1] - vs[0]) * n)
                    .TakeWhile(map.Inside)
                    .Concat(
                        Enumerable
                            .Range(0, int.MaxValue)
                            .Select(n => vs[0] - (vs[1] - vs[0]) * n)
                            .TakeWhile(map.Inside)
                    )
            )
            .Distinct()
            .Count();
    }

    private static void Solve_7((long res, long[] args)[] input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1() => input.Where(x => Check(x.res, x.args, 2)).Sum(x => x.res);
        long Part2() => input.Where(x => Check(x.res, x.args, 3)).Sum(x => x.res);

        bool Check(long res, long[] args, int opsCount) =>
            Combinatorics
                .Variants(args.Length - 1, opsCount)
                .Select(ops => Calc(args, ops))
                .Any(calculated => calculated == res);

        long Calc(long[] args, int[] ops) =>
            args[1..]
                .Zip(ops, (arg, op) => (arg, op))
                .Aggregate(args[0], (acc, x) => Op(acc, x.arg, x.op));

        long Op(long a, long b, int op) => op switch
        {
            0 => a + b,
            1 => a * b,
            2 => long.Parse(a.ToString() + b),
            _ => throw new InvalidOperationException(),
        };
    }

    private static void Solve_6(Map<char> map)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1() =>
            Start()
                .Walk(map, w => Next(w))
                .Select(x => x.Pos)
                .Distinct()
                .Count();

        long Part2() =>
            Start()
                .Walk(map, w => Next(w))
                .Select(x => x.Pos)
                .Distinct()
                .Count(
                    obstacle => Start()
                        .WalkWithCycleCheck(map, w => Next(w, obstacle))
                        .Last() is null
                );


        Walker Start() => new(map.All().Single(v => map[v] == '^'), Dir.Up);

        Walker? Next(Walker w, V? obstacle = null) =>
            !w.Forward().Inside(map) ? null
            : w.Forward().Pos == obstacle || map[w.Forward().Pos] == '#' ? w.TurnCW()
            : w.Forward();
    }

    private static void Solve_5((long before, long after)[] rules, long[][] updates)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1()
        {
            return updates
                .Where(Check)
                .Sum(u => u[u.Length / 2]);
        }

        long Part2()
        {
            return updates
                .Where(u => !Check(u))
                .Select(Correct)
                .Sum(u => u[u.Length / 2]);
        }

        bool Check(long[] update)
        {
            var posByPage = update.WithIndex().ToDictionary(e => e.item, e => e.index);
            return rules
                .Where(r => posByPage.ContainsKey(r.before) && posByPage.ContainsKey(r.after))
                .All(r => posByPage[r.before] < posByPage[r.after]);
        }

        long[] Correct(long[] update)
        {
            var deps = rules
                .Where(r => update.Contains(r.before) && update.Contains(r.after))
                .ToLookup(r => r.after, r => r.before);

            return update.TopSort(e => deps[e]).ToArray();
        }
    }

    private static void Solve_4(Map<char> input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1()
        {
            const string pattern = "XMAS";
            return input
                .All()
                .Sum(start => V.area8.Count(d => pattern.All((c, i) => input.At(start + d * i) == c)));
        }

        long Part2()
        {
            const string pattern = "MAS";
            var allDirs = new V[][]
            {
                [new V(-1, -1), new V(0, 0), new V(1, 1)],
                [new V(-1, 1), new V(0, 0), new V(1, -1)],
                [new V(1, -1), new V(0, 0), new V(-1, 1)],
                [new V(1, 1), new V(0, 0), new V(-1, -1)],
            };
            return input
                .All()
                .Count(center => allDirs.Count(dirs => pattern.All((c, i) => input.At(center + dirs[i]) == c)) == 2);
        }
    }

    private static void Solve_3(string input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1() =>
            new Regex(@"mul\((\d{1,3}),(\d{1,3})\)")
                .Matches(input)
                .Sum(m => long.Parse(m.Groups[1].Value) * long.Parse(m.Groups[2].Value));

        long Part2() =>
            new Regex(@"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)")
                .Matches(input)
                .Aggregate(
                    (res: 0L, enabled: true),
                    (acc, m) => m.Groups[0].Value switch
                    {
                        "do()" => (acc.res, true),
                        "don't()" => (acc.res, false),
                        _ => (acc.enabled ? acc.res + long.Parse(m.Groups[1].Value) * long.Parse(m.Groups[2].Value) : acc.res, acc.enabled),
                    }
                )
                .res;
    }

    private static void Solve_2(long[][] input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        bool IsSafe(long[] levels) =>
            levels.SlidingWindow(2).All(w => w[1] - w[0] is >= 1 and <= 3) ||
            levels.SlidingWindow(2).All(w => w[0] - w[1] is >= 1 and <= 3);

        bool IsSafeWithoutOneLevel(long[] levels) =>
            Enumerable.Range(0, levels.Length).Any(i => IsSafe(levels.ExceptIndex(i).ToArray()));

        long Part1() => input.Count(IsSafe);
        long Part2() => input.Count(x => IsSafeWithoutOneLevel(x) || IsSafe(x));
    }

    private static void Solve_1((long, long)[] input)
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
        return;

        long Part1()
        {
            var a = input.Select(x => x.Item1).Order().ToArray();
            var b = input.Select(x => x.Item2).Order().ToArray();
            return a.Zip(b, (x, y) => Math.Abs(x - y)).Sum();
        }

        long Part2()
        {
            var counts = input.Select(x => x.Item2).ToLookup(x => x);
            return input.Sum(x => x.Item1 * counts[x.Item1].Count());
        }
    }
}
