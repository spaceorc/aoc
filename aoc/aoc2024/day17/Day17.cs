using System;
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
        string.Join(",", Part1()).Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part2()
    {
        var variants = new List<List<int>>();
        for (int o = 0; o <= 7; o++)
        {
            variants.Add(new List<int>());
            for (var a = 0; a <= 0b111_111_111_111; a++)
            {
                var b = a & 0b111 ^ 1;
                b ^= a >> b;
                b ^= 6;
                b &= 0b111;
                if (b == o)
                    variants[^1].Add(a);
            }
        }

        var opsVariants = new List<List<int>>
        {
            variants[ops[0]],
        };

        for (var i = 1; i < ops.Length; i++)
        {
            var nextVariants = variants[ops[i]]
                .Where(v => opsVariants[^1].Any(cv => cv >> 3 == (v & 0b000_111_111_111)))
                .ToList();
            opsVariants.Add(nextVariants);
        }

        for (int i = opsVariants.Count - 1; i >= 0; i--)
        {
            opsVariants[i] = opsVariants[i].Order().Take(1).ToList();
            if (i > 0)
                opsVariants[i - 1] = opsVariants[i - 1]
                    .Where(cv => opsVariants[i].Any(v => cv >> 3 == (v & 0b000_111_111_111)))
                    .ToList();
        }

        var result = (long)opsVariants[^1][0];
        for (int i = opsVariants.Count - 2; i >= 0; i--)
        {
            result <<= 3;
            result |= opsVariants[i][0] & 0b111L;
        }


        return result;
    }

    private List<int> Part1()
    {
        var ip = 0;
        var a = regs.a;
        var b = regs.b;
        var c = regs.c;

        var result = new List<int>();
        while (ip < ops.Length)
        {
            var op = ops[ip++];
            var literal = ops[ip++];
            var combo = literal switch
            {
                >= 0 and <= 3 => literal,
                4 => a,
                5 => b,
                6 => c,
                _ => int.MaxValue,
            };
            switch (op)
            {
                case 0:
                    // adv
                    a >>= combo;
                    break;

                case 1:
                    // bxl
                    b ^= literal;
                    break;

                case 2:
                    // bst
                    b = combo & 0b111;
                    break;

                case 3:
                    // jnz
                    if (a != 0)
                        ip = literal;
                    break;

                case 4:
                    // bxc
                    b ^= c;
                    break;

                case 5:
                    // out
                    result.Add(combo & 0b111);
                    break;

                case 6:
                    // bdv
                    b = a >> combo;
                    break;

                case 7:
                    // cdv
                    c = a >> combo;
                    break;

                default:
                    throw new InvalidOperationException($"Invalid op code {op}");
            }
        }

        return result;
    }
}
