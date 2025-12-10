using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace aoc.Lib;

public class Matrix
{
    private readonly Rational[,] data;

    private Matrix(Rational[,] data)
    {
        this.data = data;
    }

    public Rational this[int row, int col]
    {
        get => data[row, col];
        set => data[row, col] = value;
    }

    public int RowCount => data.GetLength(0);
    public int ColCount => data.GetLength(1);

    public Matrix Clone()
    {
        var d = new Rational[data.GetLength(0), data.GetLength(1)];
        for (var r = 0; r < d.GetLength(0); r++)
        for (var c = 0; c < d.GetLength(1); c++)
            d[r, c] = data[r, c];
        return new Matrix(d);
    }

    public static Matrix Zero(int size) => Zero(size, size);

    public static Matrix Zero(int rows, int cols)
    {
        var d = new Rational[rows, cols];
        for (var r = 0; r < rows; r++)
        for (var c = 0; c < cols; c++)
            d[r, c] = Rational.Zero;
        return new Matrix(d);
    }

    public static Matrix I(int size)
    {
        var m = Zero(size);
        for (var i = 0; i < size; i++)
            m[i, i] = 1;
        return m;
    }

    public static Matrix Rows(params V3Rat[] rows) => Rows(rows.Select(v => new[] { v.X, v.Y, v.Z }).ToArray());
    public static Matrix Rows(params V3[] rows) => Rows(rows.Select(v => v.ToRational()).ToArray());
    public static Matrix Rows(params VRat[] rows) => Rows(rows.Select(v => new[] { v.X, v.Y }).ToArray());
    public static Matrix Rows(params V[] rows) => Rows(rows.Select(v => v.ToRational()).ToArray());

    public static Matrix Cols(params V3Rat[] cols) => Cols(cols.Select(v => new[] { v.X, v.Y, v.Z }).ToArray());
    public static Matrix Cols(params V3[] cols) => Cols(cols.Select(v => v.ToRational()).ToArray());
    public static Matrix Cols(params VRat[] cols) => Cols(cols.Select(v => new[] { v.X, v.Y }).ToArray());
    public static Matrix Cols(params V[] cols) => Cols(cols.Select(v => v.ToRational()).ToArray());

    public static Matrix Rows(params Rational[][] rows)
    {
        var d = new Rational[rows.Length, rows[0].Length];
        for (var r = 0; r < rows.Length; r++)
        for (var c = 0; c < rows[0].Length; c++)
            d[r, c] = rows[r][c];
        return new Matrix(d);
    }

    public static Matrix Cols(params Rational[][] cols)
    {
        var d = new Rational[cols[0].Length, cols.Length];
        for (var c = 0; c < cols.Length; c++)
        for (var r = 0; r < cols[0].Length; r++)
            d[r, c] = cols[c][r];
        return new Matrix(d);
    }

    public static Matrix Row(V3Rat v) => Row(v.X, v.Y, v.Z);
    public static Matrix Row(V3 v) => Row(v.X, v.Y, v.Z);

    public static Matrix Row(VRat v) => Row(v.X, v.Y);
    public static Matrix Row(V v) => Row(v.X, v.Y);

    public static Matrix Row(params Rational[] values)
    {
        var d = new Rational[1, values.Length];
        for (var i = 0; i < values.Length; i++)
            d[0, i] = values[i];
        return new Matrix(d);
    }

    public static Matrix Col(V3Rat v) => Col(v.X, v.Y, v.Z);
    public static Matrix Col(V3 v) => Col(v.X, v.Y, v.Z);

    public static Matrix Col(VRat v) => Col(v.X, v.Y);
    public static Matrix Col(V v) => Col(v.X, v.Y);

    public static Matrix Col(params long[] values) => Col(values.Select(v => new Rational(v)).ToArray());
    
    public static Matrix Col(params BigInteger[] values) => Col(values.Select(v => new Rational(v)).ToArray());
    
    public static Matrix Col(params Rational[] values)
    {
        var d = new Rational[values.Length, 1];
        for (var i = 0; i < values.Length; i++)
            d[i, 0] = values[i];
        return new Matrix(d);
    }

    public static Matrix operator *(Rational k, Matrix a) => a * k;

    public static Matrix operator *(Matrix a, Rational k)
    {
        var result = a.Clone();
        for (var r = 0; r < a.RowCount; r++)
        for (var c = 0; c < a.ColCount; c++)
            result[r, c] *= k;
        return result;
    }

    public Matrix Mul(Rational k) => this * k;
    public Matrix Mul(Matrix other) => this * other;

    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.ColCount != b.RowCount)
            throw new InvalidOperationException($"Cannot multiply matrices {a.RowCount}*{a.ColCount} and {b.RowCount}*{b.ColCount}");

        var res = Zero(a.RowCount, b.ColCount);
        for (var r = 0; r < a.RowCount; r++)
        for (var c = 0; c < b.ColCount; c++)
        for (var ac = 0; ac < a.ColCount; ac++)
            res[r, c] += a[r, ac] * b[ac, c];

        return res;
    }

    public override string ToString()
    {
        var strings = new string[RowCount, ColCount];
        for (var r = 0; r < RowCount; r++)
        for (var c = 0; c < ColCount; c++)
            strings[r, c] = data[r, c].ToString();

        var colWidths = Enumerable
            .Range(0, ColCount)
            .Select(c => Enumerable.Range(0, RowCount).Max(r => strings[r, c].Length))
            .ToArray();

        return string.Join(
            '\n',
            Enumerable
                .Range(0, RowCount)
                .Select(r => string.Join(" ", Enumerable.Range(0, ColCount).Select(c => strings[r, c].PadLeft(colWidths[c]))))
        );
    }

    public Matrix? Invert()
    {
        var result = Clone();
        return result.DoInvert();
    }

    private Matrix? DoInvert()
    {
        if (RowCount != ColCount)
            throw new InvalidOperationException("Cannot invert non-square matrix");

        var size = RowCount;
        var I = Matrix.I(size);
        for (var r = 0; r < size; r++)
        {
            if (this[r, r] == 0)
            {
                for (var nr = r + 1; nr < size; nr++)
                for (var c = 0; c < size; c++)
                {
                    this[r, c] += this[nr, c];
                    I[r, c] += I[nr, c];
                }
            }

            if (this[r, r] == 0)
                return null;

            for (var nr = r; nr < size; nr++)
            {
                var div = this[nr, r];
                if (div == 0 || div == 1)
                    continue;
                for (var c = 0; c < size; c++)
                {
                    this[nr, c] /= div;
                    I[nr, c] /= div;
                }
            }

            for (var nr = r + 1; nr < size; nr++)
            {
                if (this[nr, r] == 0)
                    continue;
                for (var c = 0; c < size; c++)
                {
                    this[nr, c] -= this[r, c];
                    I[nr, c] -= I[r, c];
                }
            }
        }

        for (var r = 0; r < size; r++)
        for (var r2 = 0; r2 < r; r2++)
        {
            var mult = this[r2, r];
            for (var c = 0; c < size; c++)
            {
                this[r2, c] -= mult * this[r, c];
                I[r2, c] -= mult * I[r, c];
            }
        }

        return I;
    }

    public V3Rat ColAsV3(int col) => new(data[0, col], data[1, col], data[2, col]);
    public V3Rat RowAsV3(int row) => new(data[row, 0], data[row, 1], data[row, 2]);

    public VRat ColAsV(int col) => new(data[0, col], data[1, col]);
    public VRat RowAsV(int row) => new(data[row, 0], data[row, 1]);
    
    public IEnumerable<Rational> Col(int col)
    {
        for (var r = 0; r < RowCount; r++)
            yield return data[r, col];
    }
}
