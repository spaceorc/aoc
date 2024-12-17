using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day7;

public class Day7((long res, long[] args)[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => input.Where(x => Check(x.res, x.args, 2)).Sum(x => x.res);
    private long Part2() => input.Where(x => Check(x.res, x.args, 3)).Sum(x => x.res);

    private static bool Check(long res, long[] args, int opsCount) =>
        Combinatorics
            .Variants(args.Length - 1, opsCount)
            .Select(ops => Calc(args, ops))
            .Any(calculated => calculated == res);

    private static long Calc(long[] args, int[] ops) =>
        args[1..]
            .Zip(ops, (arg, op) => (arg, op))
            .Aggregate(args[0], (acc, x) => Op(acc, x.arg, x.op));

    private static long Op(long a, long b, int op) => op switch
    {
        0 => a + b,
        1 => a * b,
        2 => long.Parse(a.ToString() + b),
    };
}
