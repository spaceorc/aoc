using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace aoc.Lib;

public class Treap<T> : IEnumerable<T> where T : notnull
{
    public int Count { get; private set; }

    public Node? Root { get; private set; }

    public IEnumerator<T> GetEnumerator() => InOrder(Root).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Add(T value)
    {
        if (Contains(value))
            return false;

        var (left, right) = Split(Root, value);
        Root = Merge(Merge(left, new Node(value, Random.Shared.NextInt64(), null, null)), right);
        Count++;
        return true;
    }

    public bool Remove(T value)
    {
        if (!Contains(value))
            return false;

        var (left, right) = Split(Root, value);
        Root = Merge(left, right);
        Count--;
        return true;
    }

    public bool TryGetFirstGreaterOrEqual(T value, [NotNullWhen(true)] [MaybeNullWhen(false)] out T result)
    {
        var node = Root;
        var found = false;
        result = default!;

        while (node != null)
        {
            var cmp = Comparer<T>.Default.Compare(value, node.Value);
            if (cmp == 0)
            {
                result = node.Value;
                return true;
            }

            if (cmp < 0)
            {
                found = true;
                result = node.Value;
                node = node.Left;
            }
            else
                node = node.Right;
        }

        return found;
    }

    private static IEnumerable<T> InOrder(Node? node)
    {
        if (node == null)
            yield break;

        foreach (var value in InOrder(node.Left))
            yield return value;

        yield return node.Value;

        foreach (var value in InOrder(node.Right))
            yield return value;
    }

    public bool Contains(T value)
    {
        return Contains(Root, value);
    }

    private static bool Contains(Node? node, T value)
    {
        if (node == null)
            return false;

        var cmp = Comparer<T>.Default.Compare(value, node.Value);
        return cmp == 0 || Contains(cmp < 0 ? node.Left : node.Right, value);
    }

    public static (Node? left, Node? right) Split(Node? node, T value)
    {
        if (node == null)
            return (null, null);

        var cmp = Comparer<T>.Default.Compare(value, node.Value);
        switch (cmp)
        {
            case 0:
                return (node.Left, node.Right);
            case < 0:
            {
                var (left, right) = Split(node.Left, value);
                return (left, Merge(right, node with { Left = null }));
            }
            case > 0:
            {
                var (left, right) = Split(node.Right, value);
                return (Merge(node with { Right = null }, left), right);
            }
        }
    }

    public static Node? Merge(Node? left, Node? right)
    {
        if (left == null)
            return right;
        if (right == null)
            return left;

        return left.Priority > right.Priority
            ? left with { Right = Merge(left.Right, right) }
            : right with { Left = Merge(left, right.Left) };
    }

    public record Node(T Value, long Priority, Node? Left, Node? Right);
}
