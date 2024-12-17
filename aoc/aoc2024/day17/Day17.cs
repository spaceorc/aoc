using System.Collections.Generic;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2024.day17;

public class Day17(
    [Template(
        """
        Register A: {a}
        Register B: {b}
        Register C: {c}
        """
    )]
    [NonArray]
    (int a, int b, int c) regs,
    [Template(
        """
        Program: {ops}
        """
    )]
    [NonArray]
    int[] ops
)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private IEnumerable<int> Run(int a) => new State(a, regs.b, regs.c, 0).Run(ops);

    private string Part1() => string.Join(',', Run(regs.a));

    private long Part2()
    {
        var opsVariants = ops
            .Skip(1)
            .Scan(
                GetVariants(ops[0]),
                (prev, op) => GetVariants(op)
                    .Where(v => prev.Any(cv => VariantsMatch(cv, v)))
                    .ToArray()
            )
            .ToArray();

        return opsVariants
            .Reverse()
            .Skip(1)
            .Scan(
                opsVariants[^1].Min(),
                (variant, prev) => prev.Where(cv => VariantsMatch(cv, variant)).Min()
            )
            .Aggregate(0L, (acc, v) => (acc << 3) | v);
    }

    private int[] GetVariants(int op) => Enumerable
        .Range(0, 0b1_000_000_000_000)
        .Where(a => Run(a).First() == op)
        .ToArray();

    private static bool VariantsMatch(int prev, int next) => prev >> 3 == (next & 0b000_111_111_111);

    private record State(int A, int B, int C, int Ip)
    {
        private const int adv = 0;
        private const int bxl = 1;
        private const int bst = 2;
        private const int jnz = 3;
        private const int bxc = 4;
        private const int @out = 5;
        private const int bdv = 6;
        private const int cdv = 7;

        private int Combo(int literal) => literal switch
        {
            >= 0 and <= 3 => literal,
            4 => A,
            5 => B,
            6 => C,
        };

        public State Next(int op, int arg) => op switch
        {
            adv => this with { A = A >> Combo(arg), Ip = Ip + 2 },
            bxl => this with { B = B ^ arg, Ip = Ip + 2 },
            bst => this with { B = Combo(arg) & 0b111, Ip = Ip + 2 },
            jnz => A != 0 ? this with { Ip = arg } : this with { Ip = Ip + 2 },
            bxc => this with { B = B ^ C, Ip = Ip + 2 },
            @out => this with { Ip = Ip + 2 },
            bdv => this with { B = A >> Combo(arg), Ip = Ip + 2 },
            cdv => this with { C = A >> Combo(arg), Ip = Ip + 2 },
        };

        public IEnumerable<int> Run(int[] ops)
        {
            var state = this;
            while (state.Ip < ops.Length)
            {
                var op = ops[state.Ip];
                var arg = ops[state.Ip + 1];
                if (op == @out)
                    yield return state.Combo(arg) & 0b111;
                state = state.Next(op, arg);
            }
        }
    }
}
