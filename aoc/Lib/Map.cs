using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public class Map<T>
{
    public readonly T[] data;
    public readonly int sizeX;
    public readonly int sizeY;
    public readonly int totalCount;

    public Map(int size)
        : this(size, size)
    {
    }

    public Map(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        totalCount = sizeX * sizeY;
        data = new T[totalCount];
    }

    public T this[V v]
    {
        get => data[v.Y * sizeX + v.X];
        set => data[v.Y * sizeX + v.X] = value;
    }

    public T this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }

    public V TopCenter => (TopLeft + TopRight) / 2;
    public V BottomCenter => (BottomLeft + BottomRight) / 2;
    public V LeftCenter => (TopLeft + BottomLeft) / 2;
    public V RightCenter => (TopRight + BottomRight) / 2;
    public V TopLeft => new(0, 0);
    public V TopRight => new(sizeX - 1, 0);
    public V BottomLeft => new(0, sizeY - 1);
    public V BottomRight => new(sizeX - 1, sizeY - 1);

    public T At(V v, T defaultValue = default!) => Inside(v) ? this[v] : defaultValue;
    public void Clear() => Array.Fill(data, default);
    public void Fill(T value) => Array.Fill(data, value);
    public bool Inside(V v) => v is { X: >= 0, Y: >= 0 } && v.X < sizeX && v.Y < sizeY;

    public IEnumerable<V> All()
    {
        for (var y = 0; y < sizeY; y++)
        for (var x = 0; x < sizeX; x++)
            yield return new V(x, y);
    }

    public IEnumerable<V> All(T value) => All()
        .Where(v => EqualityComparer<T>.Default.Equals(this[v], value));

    public V Single(T value) => All(value).Single();

    public IEnumerable<V> AllBut(T value) => All()
        .Where(v => !EqualityComparer<T>.Default.Equals(this[v], value));

    public IEnumerable<V> AllButBorder()
    {
        for (var y = 1; y < sizeY - 1; y++)
        for (var x = 1; x < sizeX - 1; x++)
            yield return new V(x, y);
    }

    public IEnumerable<V> TopBorder()
    {
        for (var x = 0; x < sizeX; x++)
            yield return new V(x, 0);
    }

    public IEnumerable<V> BottomBorder()
    {
        for (var x = 0; x < sizeX; x++)
            yield return new V(x, sizeY - 1);
    }

    public IEnumerable<V> LeftBorder()
    {
        for (var y = 0; y < sizeY; y++)
            yield return new V(0, y);
    }

    public IEnumerable<V> RightBorder()
    {
        for (var y = 0; y < sizeY; y++)
            yield return new V(sizeX - 1, y);
    }

    public Square Square() => new(V.Zero, BottomRight);

    public IEnumerable<V> Area4(V v)
    {
        return V.area4.Select(dv => v + dv).Where(Inside);
    }

    public IEnumerable<V> Area5(V v)
    {
        return V.area5.Select(dv => v + dv).Where(Inside);
    }

    public IEnumerable<V> Area8(V v)
    {
        return V.area8.Select(dv => v + dv).Where(Inside);
    }

    public IEnumerable<V> Ray(V start, V direction)
    {
        var v = start;
        while (Inside(v))
        {
            yield return v;
            v += direction;
        }
    }

    public IEnumerable<V> Column(long x)
    {
        for (var y = 0; y < sizeY; y++)
            yield return new V(x, y);
    }

    public IEnumerable<T> ColumnValues(long x)
    {
        return Column(x).Select(v => this[v]);
    }

    public string ColumnString(long x, string separator = "")
    {
        return string.Join(separator, ColumnValues(x));
    }

    public IEnumerable<V[]> Columns()
    {
        for (var x = 0; x < sizeX; x++)
            yield return Column(x).ToArray();
    }

    public IEnumerable<string> ColumnsStrings()
    {
        for (var x = 0; x < sizeX; x++)
            yield return ColumnString(x);
    }

    public IEnumerable<V> Row(long y)
    {
        for (var x = 0; x < sizeX; x++)
            yield return new V(x, y);
    }

    public IEnumerable<T> RowValues(long y)
    {
        return Row(y).Select(v => this[v]);
    }

    public string RowString(long y, string separator = "")
    {
        return string.Join(separator, RowValues(y));
    }

    public IEnumerable<V[]> Rows()
    {
        for (var y = 0; y < sizeY; y++)
            yield return Row(y).ToArray();
    }

    public IEnumerable<string> RowsStrings(string separator = "")
    {
        for (var y = 0; y < sizeY; y++)
            yield return RowString(y, separator);
    }

    public IEnumerable<T> ValuesAt(IEnumerable<V> vs)
    {
        return vs.Select(v => this[v]);
    }

    public IDisposable ChangeAt(V v, T newValue)
    {
        var original = this[v];
        this[v] = newValue;
        return new DisposableAction(() => this[v] = original);
    }

    public IDisposable ChangeAt(V v, Func<T, T> getNewValue)
    {
        return ChangeAt(v, getNewValue(this[v]));
    }

    public void Swap(V a, V b)
    {
        if (a != b)
            (this[a], this[b]) = (this[b], this[a]);
    }

    public Map<T> Clone()
    {
        var clone = new Map<T>(sizeX, sizeY);
        Array.Copy(data, clone.data, totalCount);
        return clone;
    }

    public void CopyTo(Map<T> other)
    {
        Array.Copy(data, other.data, totalCount);
    }

    public override int GetHashCode()
    {
        return data.Aggregate(0, (acc, item) => HashCode.Combine(acc, item?.GetHashCode()));
    }

    public static Map<T> Parse(string s)
    {
        return ToMap(s.Split('\n'));
    }

    private static Map<T> ToMap(IEnumerable<string> lines)
    {
        return ToMap(
            lines,
            c =>
            {
                if (typeof(T) == typeof(char))
                    return (T)(object)c;
                if (typeof(T) == typeof(int))
                    return (T)(object)(c - '0');
                if (typeof(T) == typeof(long))
                    return (T)(object)(long)(c - '0');
                throw new InvalidOperationException($"Unsupported type {typeof(T)}");
            }
        );
    }

    private static Map<T> ToMap(IEnumerable<string> lines, Func<char, T> selector)
    {
        var linesArr = lines as IList<string> ?? lines.ToArray();
        var result = new Map<T>(linesArr[0].Length, linesArr.Count);
        for (var y = 0; y < result.sizeY; y++)
        for (var x = 0; x < result.sizeX; x++)
            result[new V(x, y)] = selector(linesArr[y][x]);

        return result;
    }

    public string Dump(string separator = "")
    {
        return string.Join("\n", RowsStrings(separator));
    }

    public void Print()
    {
        Console.WriteLine(Dump());
    }
}
