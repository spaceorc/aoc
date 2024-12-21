using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public record SearchPathItem<TState>(TState State, long Distance, SearchPathItem<TState>? Prev)
{
    public List<SearchPathItem<TState>> Predecessors { get; } = Prev is null ? [] : [Prev];
    public IEnumerable<TState> AllPredecessors => Search.Bfs([this], x => x.Predecessors).Select(x => x.State.State);

    public IEnumerable<TState> PathBack()
    {
        for (var c = this; c != null; c = c.Prev)
            yield return c.State;
    }

    public IEnumerable<TState> Path() => PathBack().Reverse();

    private List<List<TState>>? cachedAllPaths;

    public List<List<TState>> AllPaths()
    {
        if (cachedAllPaths != null)
            return cachedAllPaths;

        var result = new List<List<TState>>();
        if (Predecessors.Count == 0)
        {
            result.Add([State]);
        }
        else
        {
            foreach (var path in Predecessors.SelectMany(predecessor => predecessor.AllPaths()))
            {
                result.Add(path.Append(State).ToList());
            }
        }
        cachedAllPaths = result;
        return result;
    }    
}
