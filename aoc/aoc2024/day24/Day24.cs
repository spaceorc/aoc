using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public string Part2()
    {
        return string.Join(",", new string[]
        {
            "z07",
            "swt",
            "pqc",
            "z13",
            "wsv",
            "rjm",
            "bgs",
            "z31",
        }.Order());
        // x0 ^ y0 -> z0
        // x0 & y0 -> carrier1
        //
        // x ^ y -> sum1
        // sum1 ^ carrier -> z
        // x & y -> next1
        // sum1 & carrier -> next2
        // next1 | next2 -> nextCarrier
        //
        // xp ^ yp -> sum1p
        // sum1p ^ carrierP -> zp
        // xp & yp -> next1p
        // sum1p & carrierP -> next2p
        // next1p | next2p -> zLast
        

        var badZ = wires
            .Where(w => w.r.StartsWith('z') && w.op != "XOR" && w.r[1..] != "45")
            .Select(w => w.r)
            .ToArray();

        Console.WriteLine(string.Join(", ", badZ));

        var sum1 = wires
            .Where(w => w.op == "XOR" && IsXY(w) && !IsZero(w))
            .Select(w => w.r)
            .ToArray();

        var badSum1 = sum1
            .Where(s => wires.Any(w => (w.a == s || w.b == s) && w.op == "OR"))
            .ToArray();
        
        Console.WriteLine(string.Join(", ", badSum1));

        var next1 = wires
            .Where(w => w.op == "AND" && IsXY(w) && !IsZero(w))
            .Select(w => w.r)
            .ToArray();

        var badNext1 = next1
            .Where(s => wires.Any(w => (w.a == s || w.b == s) && w.op != "OR"))
            .ToArray();
        
        Console.WriteLine(string.Join(", ", badNext1));
        
        var carrier = wires
            .Where(w => w.op == "OR" || (w.op == "AND" && IsXY(w) && IsZero(w)))
            .Select(w => w.r)
            .ToArray();
        
        var badCarrier = carrier
            .Where(s => wires.Any(w => (w.a == s || w.b == s) && w.op == "OR"))
            .ToArray();
        
        Console.WriteLine(string.Join(", ", badCarrier));


        // var sum1 = wires
        //     .Where(w => w.op == "XOR" && (w.a.StartsWith('x') || w.a.StartsWith('y')))
        //     .OrderBy(w => w.a[1..])
        //     .Select((w, i) => (i, w.r))
        //     .ToDictionary(x => x.i, x => x.r);
        //
        // wires.Where(w => w.r.StartsWith('z'))


        return "";
        
        bool IsXY((string a, string op, string b, string r) wire) =>
            (wire.a.StartsWith('x') || wire.a.StartsWith('y')) && (wire.b.StartsWith('x') || wire.b.StartsWith('y'));

        bool IsZero((string a, string op, string b, string r) wire) => wire.a[1..] == "00";
            
    }

    public long Part1()
    {
        var signalsDict = signals.ToDictionary(x => x.wire, x => x.bit);
        var wiresDict = wires.ToDictionary(x => x.r, x => (x.a, x.op, x.b));

        return wires.Select(w => w.r)
            .Where(r => r.StartsWith('z'))
            .OrderDescending()
            .Select(GetValue)
            .Aggregate(0L, (acc, value) => acc << 1 | value);

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
}
