using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public record V(long X, long Y)
{
    public static readonly V Zero = new(0, 0);
    public static readonly V One = new(1, 1);

    public static readonly V[] up3 = [new(-1, -1), new(0, -1), new(1, -1)];
    public static readonly V[] down3 = [new(-1, 1), new(0, 1), new(1, 1)];
    public static readonly V[] left3 = [new(-1, -1), new(-1, 0), new(-1, 1)];
    public static readonly V[] right3 = [new(1, -1), new(1, 0), new(1, 1)];
    public static readonly V up = new(0, -1);
    public static readonly V down = new(0, 1);
    public static readonly V left = new(-1, 0);
    public static readonly V right = new(1, 0);
    public static readonly V[] dirs = [up, right, down, left];

    public static readonly V[] area4 = [new(1, 0), new(-1, 0), new(0, 1), new(0, -1)];
    public static readonly V[] area5 = [new(0, 0), ..area4];

    public static readonly V[] area8 =
    [
        new(1, 0), new(-1, 0), new(0, 1), new(0, -1),
        new(1, 1), new(-1, -1), new(-1, 1), new(1, -1),
    ];

    public static readonly V[] area9 =
    [
        new(0, 0), ..area8,
    ];

    public V Norm => new(Math.Sign(X), Math.Sign(Y));

    public static V Dir(Dir dir) => dirs[(int)dir];

    public static V operator +(V a, V b) => new(a.X + b.X, a.Y + b.Y);
    public static V operator *(V a, long k) => new(a.X * k, a.Y * k);
    public static V operator *(long k, V a) => a * k;
    public static V operator /(V a, long k) => new(a.X / k, a.Y / k);
    public static V operator %(V a, long k) => new(a.X % k, a.Y % k);
    public static V operator -(V a, V b) => new(a.X - b.X, a.Y - b.Y);
    public static V operator -(V a) => new(-a.X, -a.Y);

    public long CDistTo(V other) => (this - other).CLen();
    public long CLen() => Math.Max(Math.Abs(X), Math.Abs(Y));
    public long MDistTo(V other) => (this - other).MLen();
    public long MLen() => Math.Abs(X) + Math.Abs(Y);
    public static long XProd(V a, V b) => a.X * b.Y - a.Y * b.X;
    public static long DProd(V a, V b) => a.X * b.X + a.Y * b.Y;

    public override string ToString() => $"{X} {Y}";

    public IEnumerable<V> Area4() => area4.Select(x => this + x);
    public IEnumerable<V> Area5() => area5.Select(x => this + x);
    public IEnumerable<V> Area8() => area8.Select(x => this + x);

    public IEnumerable<V> CCircle(long r)
    {
        for (var y = -r; y <= r; y++)
        for (var x = -r; x <= r; x++)
            yield return this + new V(x, y);
    }

    public IEnumerable<V> MCircle(long r)
    {
        for (var y = -r; y <= r; y++)
        {
            var dx = r - Math.Abs(y);
            for (var x = -dx; x <= dx; x++)
                yield return this + new V(x, y);
        }
    }

    public IEnumerable<V> CCircumference(long r)
    {
        for (var x = -r; x <= r; x++)
        {
            yield return this + new V(x, -r);
            yield return this + new V(x, r);
        }

        for (var y = -r + 1; y <= r - 1; y++)
        {
            yield return this + new V(-r, y);
            yield return this + new V(r, y);
        }
    }

    public IEnumerable<V> MCircumference(long r)
    {
        for (var y = -r; y <= r; y++)
        {
            var dx = r - Math.Abs(y);
            yield return this + new V(-dx, y);
            if (dx != 0)
                yield return this + new V(dx, y);
        }
    }

    public bool InSquare(Square r) => X >= r.MinX && X <= r.MaxX && Y >= r.MinY && Y <= r.MaxY;

    public V Mod(long k) => new(X.Mod(k), Y.Mod(k));
    public V Mod(long x, long y) => new(X.Mod(x), Y.Mod(y));
    public V Mod(V other) => new(X.Mod(other.X), Y.Mod(other.Y));
    public V Sign() => new(Math.Sign(X), Math.Sign(Y));

    public V RotateCW() => new(-Y, X);
    public V RotateCCW() => new(Y, -X);

    public VRat ToRational() => new(X, Y);
}
