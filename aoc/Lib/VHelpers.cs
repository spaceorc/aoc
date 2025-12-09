using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public static class VHelpers
{
    public static Square BoundingBox(this IEnumerable<V> vectors)
    {
        var list = vectors.ToList();
        var minX = list.Min(x => x.X);
        var maxX = list.Max(x => x.X);
        var minY = list.Min(x => x.Y);
        var maxY = list.Max(x => x.Y);
        return new Square(minX, minY, maxX, maxY);
    }

    public static IEnumerable<V> MakeLine(V a, V b)
    {
        var d = b - a;
        if (d == new V(0, 0))
        {
            yield return a;
            yield break;
        }

        var gcd = MathHelpers.Gcd(Math.Abs(d.X), Math.Abs(d.Y));
        d /= gcd;

        for (var v = a; v != b; v += d)
            yield return v;
        yield return b;
    }
    
    public static bool IsInsidePolygon(this V point, IEnumerable<V> polygon)
    {
        var pts = polygon as IList<V> ?? polygon.ToList();
        var n = pts.Count;
        var windingNumber = 0;
        for (var i = 0; i < n; i++)
        {
            var a = pts[i];
            var b = pts[(i + 1) % n];
            var xProd = V.XProd(point - a, b - a);
            
            if (
                xProd == 0 && // collinear
                Math.Min(a.X, b.X) <= point.X && point.X <= Math.Max(a.X, b.X) &&
                Math.Min(a.Y, b.Y) <= point.Y && point.Y <= Math.Max(a.Y, b.Y)
            )
            {
                return true; // on the bound
            }
            
            if (a.Y <= point.Y)
            {
                if (b.Y > point.Y && xProd > 0)
                    windingNumber++;
            }
            else
            {
                if (b.Y <= point.Y && xProd < 0)
                    windingNumber--;
            }
        }
        return windingNumber > 0;
    }

    public static Dir ToDir(this V v) =>
        v == V.up ? Dir.Up :
        v == V.down ? Dir.Down :
        v == V.left ? Dir.Left :
        v == V.right ? Dir.Right :
        throw new Exception($"Invalid direction {v}");

    public static char ToDirChar(this V v) => v.ToDir().ToChar();
}
