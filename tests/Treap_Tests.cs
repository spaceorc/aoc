using aoc.Lib;
using FluentAssertions;

public class Treap_Tests
{
    [Test]
    public void Test_Single()
    {
        var treap = new Treap<int>();
        treap.Add(1);
        treap.Remove(1);
        treap.Add(1);

        treap.Count.Should().Be(1);
        treap.ToArray().Should().Equal(1);
    }

    [Test]
    public void Test()
    {
        var treap = new Treap<int>();
        var values = Enumerable.Range(0, 1).ToArray();
        Random.Shared.Shuffle(values);
        foreach (var value in values)
            treap.Add(value);
        foreach (var value in values)
            treap.Remove(value);
        foreach (var value in values)
            treap.Add(value);

        treap.Count.Should().Be(values.Length);
        treap.ToArray().Should().Equal(Enumerable.Range(0, values.Length).ToArray());
    }
}
