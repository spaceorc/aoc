using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2024.day13;

public class Day13(
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
    public void Solve()
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
}
