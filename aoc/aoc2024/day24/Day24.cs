using System.Linq;
using aoc.Lib;
using aoc.ParseLib.Attributes;

namespace aoc.aoc2024.day24;

public class Day24((string wire, long bit)[] signals, [Atom(" ->")] (string a, string op, string b, string r)[] wires)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 1: ");
    }

    private long Part1()
    {
        var signalsDict = signals.ToDictionary(x => x.wire, x => x.bit);
        var wiresDict = wires.ToDictionary(x => x.r, x => (x.a, x.op, x.b));

        return wires.Select(w => w.r)
            .Where(r => r.StartsWith('z'))
            .OrderDescending()
            .Select(GetValue)
            .Aggregate(0L, (acc, value) => (acc << 1) | value);

        long GetValue(string wire)
        {
            if (signalsDict.TryGetValue(wire, out var signal))
                return signal;

            var (a, op, b) = wiresDict[wire];
            return op switch
            {
                "AND" => GetValue(a) & GetValue(b),
                "OR" => GetValue(a) | GetValue(b),
                "XOR" => GetValue(a) ^ GetValue(b),
            };
        }
    }

    private string Part2()
    {
        /* === START ===
         * X0-+--XOR---->Z0
         *     \/
         *     /\
         * Y0-+--AND---->C_OUT_0
         *
         *
         * === MIDDLE ===
         * C_IN-----------+--XOR------------>Z
         *                 \/
         *                 /\
         * X--+--XOR-->S1-+--AND-->C2-+
         *     \/                      \
         *     /\                       \
         * Y--+--AND-->C1----------------OR-->C_OUT
         *
         *
         * === END ===
         * C_IN-----------+--XOR------------>Z
         *                 \/
         *                 /\
         * X--+--XOR-->S1-+--AND-->C2-+
         *     \/                      \
         *     /\                       \
         * Y--+--AND-->C1----------------OR-->Z_LAST
         */

        var S1 = wires
            .Where(w => w.op == "XOR" && IsXY(w) && !IsZero(w))
            .Select(w => w.r)
            .ToArray();

        var C1 = wires
            .Where(w => w.op == "AND" && IsXY(w) && !IsZero(w))
            .Select(w => w.r)
            .ToArray();

        var badS1 = S1
            .Where(s => wires.Any(w => HasArg(w, s) && w.op == "OR"))
            .ToArray();

        var badC1 = C1
            .Where(c => wires.Any(w => HasArg(w, c) && w.op != "OR"))
            .ToArray();

        var badZ_AND_XY = wires
            .Where(w => w.r.StartsWith('z') && IsXY(w) && w.op == "AND")
            .ToArray();

        var badZ_AND_XY_Pairs = badZ_AND_XY
            .Select(w => wires.Single(other => HasArgs(other, $"x{w.r[1..]}", $"y{w.r[1..]}") && other.op == "XOR"))
            .Select(w => wires.Single(other => HasArg(other, w.r) && other.op == "XOR"))
            .ToArray();

        var badZ_AND_NotXY = wires
            .Where(w => w.r.StartsWith('z') && !IsXY(w) && w.op == "AND")
            .ToArray();

        var badZ_AND_NotXY_Pairs = badZ_AND_NotXY
            .Select(w => wires.Single(other => HasArgs(other, w.a, w.b) && other.op == "XOR"))
            .ToArray();

        var lastZ = wires
            .Where(w => w.r.StartsWith('z'))
            .Select(w => w.r)
            .Max();

        var badZ_OR = wires
            .Where(w => w.r.StartsWith('z') && w.op == "OR" && w.r != lastZ)
            .ToArray();

        var badZ_OR_Pairs = badZ_OR
            .Select(w => wires.Single(other => HasArgs(other, $"x{w.r[1..]}", $"y{w.r[1..]}") && other.op == "XOR"))
            .Select(w => wires.Single(other => HasArg(other, w.r) && other.op == "XOR"))
            .ToArray();

        string[] result =
        [
            ..badC1,
            ..badS1,
            ..badZ_AND_XY.Select(w => w.r),
            ..badZ_AND_XY_Pairs.Select(w => w.r),
            ..badZ_AND_NotXY.Select(w => w.r),
            ..badZ_AND_NotXY_Pairs.Select(w => w.r),
            ..badZ_OR.Select(w => w.r),
            ..badZ_OR_Pairs.Select(w => w.r),
        ];

        return string.Join(",", result.Order());

        bool IsXY((string a, string op, string b, string r) wire) =>
            (wire.a.StartsWith('x') && wire.b.StartsWith('y')) || (wire.a.StartsWith('y') && wire.b.StartsWith('x'));

        bool HasArgs((string a, string op, string b, string r) wire, string a, string b) =>
            (wire.a == a && wire.b == b) || (wire.a == b && wire.b == a);

        bool IsZero((string a, string op, string b, string r) wire) => wire.a[1..] == "00";

        bool HasArg((string a, string op, string b, string r) wire, string arg) => wire.a == arg || wire.b == arg;
    }
}
