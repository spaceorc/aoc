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
}
