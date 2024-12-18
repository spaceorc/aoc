using aoc.Lib;

namespace aoc;

public record R2(V Start, V Len)
{
    public V End => Start + Len;
    public bool IsEmpty => Len.X <= 0 || Len.Y <= 0;
    public R2 Shift(V delta) => this with { Start = Start + delta };
    public R2 ShiftX(long delta) => this with { Start = Start + new V(delta, 0) };
    public R2 ShiftY(long delta) => this with { Start = Start + new V(0, delta) };
    public static R2 FromStartEnd(V start, V end) => new(start, end - start);
    public bool Contains(V v) => v.X >= Start.X && v.X < End.X && v.Y >= Start.Y && v.Y < End.Y;
}
