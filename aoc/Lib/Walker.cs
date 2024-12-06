using System;
using System.Collections.Generic;

namespace aoc.Lib;

public record Walker(V Pos, Dir Dir)
{
    public Walker Forward(long k = 1) => this with { Pos = Pos + V.Dir(Dir) * k };
    public Walker TurnCW(long k = 1) => this with { Dir = Dir.RotateCW(k) };
    public Walker TurnCCW(long k = 1) => this with { Dir = Dir.RotateCCW(k) };
    public bool Inside<T>(Map<T> map) => map.Inside(Pos);

    public IEnumerable<Walker> Walk<T>(Map<T> map, Func<Walker, Walker?> getNext)
    {
        var w = this;
        while (true)
        {
            yield return w;
            w = getNext(w);
            if (w is null || !w.Inside(map))
                break;
        }
    }

    public IEnumerable<Walker?> WalkWithCycleCheck<T>(Map<T> map, Func<Walker, Walker?> getNext)
    {
        var used = new HashSet<Walker>();
        foreach (var w in Walk(map, getNext))
        {
            if (!used.Add(w))
            {
                yield return null;
                break;
            }
            yield return w;
        }
    }
}
