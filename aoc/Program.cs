using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using aoc.Lib;
using aoc.ParseLib;

namespace aoc;

public static class Program
{
    private static void Main()
    {
        Runner.RunFile("day9.txt", Solve_9);
        // Runner.RunFile("day8.txt", Solve_8);
        // Runner.RunFile("day7.txt", Solve_7);
        // Runner.RunFile("day6.txt", Solve_6);
        // Runner.RunFile("day5.txt", Solve_5);
        // Runner.RunFile("day4.txt", Solve_4);
        // Runner.RunFile("day3.txt", Solve_3);
        // Runner.RunFile("day2.txt", Solve_2);
        // Runner.RunFile("day1.txt", Solve_1);
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
            var freeBlocks = Enumerable.Range(0, 10).Select(_ => new Heap<int>()).ToArray();
            var files = new List<(int id, int size, int pos)>();
            var id = 0;
            var isFree = false;
            var pos = 0;
            foreach (var c in input)
            {
                if (isFree)
                {
                    if (c != '0')
                    {
                        freeBlocks[c - '0'].Add(pos);
                        pos += c - '0';
                    }
                }
                else
                {
                    files.Add((id++, c - '0', pos));
                    pos += c - '0';
                }

                isFree = !isFree;
            }

            var newFiles = new List<(int id, int size, int pos)>();
            for (var i = files.Count - 1; i >= 0; i--)
            {
                var file = files[i];

                var matchingFreeBlocks = freeBlocks
                    .WithIndex()
                    .Where(x => x.index >= file.size && x.item.Count > 0 && x.item.Min < file.pos)
                    .ToArray();
                if (matchingFreeBlocks.Length == 0)
                {
                    newFiles.Add(file);
                    continue;
                }

                var freeBlock = matchingFreeBlocks.MinBy(x => x.item.Min);
                var newPos = freeBlock.item.DeleteMin();
                newFiles.Add(file with { pos = newPos });

                if (freeBlock.index > file.size)
                    freeBlocks[freeBlock.index - file.size].Add(newPos + file.size);
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
