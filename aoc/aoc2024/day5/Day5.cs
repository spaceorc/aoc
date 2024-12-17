using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day5;

public class Day5((long before, long after)[] rules, long[][] updates)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        return updates
            .Where(Check)
            .Sum(u => u[u.Length / 2]);
    }

    private long Part2()
    {
        return updates
            .Where(u => !Check(u))
            .Select(Correct)
            .Sum(u => u[u.Length / 2]);
    }

    private bool Check(long[] update)
    {
        var posByPage = update.WithIndex().ToDictionary(e => e.item, e => e.index);
        return rules
            .Where(r => posByPage.ContainsKey(r.before) && posByPage.ContainsKey(r.after))
            .All(r => posByPage[r.before] < posByPage[r.after]);
    }

    private long[] Correct(long[] update)
    {
        var deps = rules
            .Where(r => update.Contains(r.before) && update.Contains(r.after))
            .ToLookup(r => r.after, r => r.before);

        return update.TopSort(e => deps[e]).ToArray();
    }
}
