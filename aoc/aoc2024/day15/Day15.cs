using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day15;

public class Day15(Map<char> inputMap, string moves)
{
    private readonly char[] moves = moves.Where(m => m is '>' or '<' or '^' or 'v').ToArray();
    
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        var map = inputMap.Clone();
        foreach (var move in moves)
        {
            Move(
                move switch
                {
                    '>' => V.right,
                    '<' => V.left,
                    '^' => V.up,
                    'v' => V.down,
                }
            );
        }

        return map.All().Where(v => map[v] == 'O').Sum(v => v.X + v.Y * 100);

        void Move(V dir)
        {
            var robot = map.All().Single(v => map[v] == '@');
            var ray = map.Ray(robot, dir).TakeWhile(v => map[v] != '#').TakeUntil(v => map[v] == '.').ToArray();
            if (map[ray[^1]] != '.')
                return;
            map[robot] = '.';
            map[robot + dir] = '@';
            if (map[ray[^1]] == '.')
                map[ray[^1]] = 'O';
        }
    }

    private long Part2()
    {
        var walls = inputMap.All().Where(v => inputMap[v] == '#').SelectMany(v => new[] { v with { X = v.X * 2 }, v with { X = v.X * 2 + 1 } }).ToHashSet();
        var robot = inputMap.All().Where(v => inputMap[v] == '@').Select(v => v with { X = v.X * 2 }).Single();
        var boxes = inputMap.All().Where(v => inputMap[v] == 'O').Select(v => v with { X = v.X * 2 }).ToHashSet();

        foreach (var move in moves)
        {
            Move(
                move switch
                {
                    '>' => V.right,
                    '<' => V.left,
                    '^' => V.up,
                    'v' => V.down,
                }
            );
        }

        return boxes.Sum(v => v.X + v.Y * 100);

        V[] BoxCoords(V box) => [box, box + new V(1, 0)];
        V? BoxAt(V v) => boxes.Contains(v) ? v : boxes.Contains(v - new V(1, 0)) ? v - new V(1, 0) : null;
        bool BoxAtWall(V box) => BoxCoords(box).Any(walls.Contains);

        void Move(V dir)
        {
            if (walls.Contains(robot + dir))
                return;
            if (BoxAt(robot + dir) is { } box)
            {
                if (AllBoxesInDir(box, dir) is not { } boxesToMove)
                    return;
                boxes.ExceptWith(boxesToMove);
                boxes.UnionWith(boxesToMove.Select(v => v + dir));
            }

            robot += dir;
        }

        V[]? BoxesInDir(V box, V dir) => BoxAtWall(box + dir)
            ? null
            : BoxCoords(box + dir)
                .Select(BoxAt)
                .Where(b => b is not null && b != box)!
                .ToArray<V>();

        V[]? AllBoxesInDir(V box, V dir)
        {
            var allBoxes = new List<V> { box };
            for (var i = 0; i < allBoxes.Count; i++)
            {
                if (BoxesInDir(allBoxes[i], dir) is not { } next)
                    return null;
                allBoxes.AddRange(next);
            }

            return allBoxes.ToArray();
        }
    }
}
