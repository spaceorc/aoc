using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public class ArrayEqualityComparer<T>(IEqualityComparer<T>? elementComparer = null) : IEqualityComparer<T[]>
{
    private readonly IEqualityComparer<T> elementComparer = elementComparer ?? EqualityComparer<T>.Default;

    public bool Equals(T[]? x, T[]? y)
    {
        if (x == null && y == null)
            return true;
        if (x == null || y == null)
            return false;
        if (x.Length != y.Length)
            return false;
        for (var i = 0; i < x.Length; i++)
        {
            if (!elementComparer.Equals(x[i], y[i]))
                return false;
        }

        return true;
    }

    public int GetHashCode(T[] obj)
    {
        return obj.Aggregate(obj.Length, (current, item) => HashCode.Combine<int, int>(current, ReferenceEquals(item, null) ? 0 : elementComparer.GetHashCode(item)));
    }
}

public static class ArrayEqualityComparer
{
    public static ArrayEqualityComparer<T> Create<T>() => new();
}
