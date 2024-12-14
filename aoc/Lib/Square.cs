using System;
using System.Collections.Generic;

namespace aoc.Lib;

public class Square(V topLeft, V bottomRight) : IEquatable<Square>
{
    public Square(long minX, long minY, long maxX, long maxY)
        : this(new V(minX, minY), new V(maxX, maxY))
    {
    }

    public V TopLeft { get; } = topLeft;
    public V BottomRight { get; } = bottomRight;
    public long MinX => TopLeft.X;
    public long MinY => TopLeft.Y;
    public long MaxX => BottomRight.X;
    public long MaxY => BottomRight.Y;

    public long Area => (MaxX - MinX + 1) * (MaxY - MinY + 1);

    public bool Equals(Square? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return MinX == other.MinX && MaxX == other.MaxX && MinY == other.MinY && MaxY == other.MaxY;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((Cube)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MinX, MaxX, MinY, MaxY);
    }

    public static bool operator ==(Square? left, Square? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Square? left, Square? right)
    {
        return !Equals(left, right);
    }

    public Square Grow(long delta) => new(TopLeft - new V(delta, delta), BottomRight + new V(delta, delta));
    public Square Shift(V delta) => new(TopLeft + delta, BottomRight + delta);
    public Square ShiftX(long delta) => new(TopLeft + new V(delta, 0), BottomRight + new V(delta, 0));
    public Square ShiftY(long delta) => new(TopLeft + new V(0, delta), BottomRight + new V(0, delta));
    public bool Contains(V v) => v.X >= MinX && v.X <= MaxX && v.Y >= MinY && v.Y <= MaxY;
    public bool IsEmpty() => MinX > MaxX || MinY > MaxY;

    public IEnumerable<V> All()
    {
        for (var y = MinY; y <= MaxY; y++)
        for (var x = MinX; x <= MaxX; x++)
            yield return new V(x, y);
    }

    public override string ToString()
    {
        return $"MinX: {MinX}, MinY: {MinY}, MaxX: {MaxX}, MaxY: {MaxY}";
    }
}
