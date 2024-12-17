using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day9;

public class Day9(string input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        var data = new List<int>();
        var id = 0;
        var isFree = false;
        foreach (var c in input)
        {
            data.AddRange(isFree ? Enumerable.Repeat(-1, c - '0') : Enumerable.Repeat(id++, c - '0'));
            isFree = !isFree;
        }

        var total = data.Count(x => x != -1);
        var freeIndex = data.IndexOf(-1);
        for (var i = data.Count - 1; i >= 0; i--)
        {
            if (data[i] == -1)
                continue;
            (data[freeIndex], data[i]) = (data[i], data[freeIndex]);
            freeIndex = data.IndexOf(-1, freeIndex + 1);
            if (freeIndex >= total)
                break;
        }

        return data.Select((t, i) => t == -1 ? 0 : t * (long)i).Sum();
    }

    private long Part2()
    {
        var freeBlocksBySize = Enumerable.Range(0, 10).Select(_ => new Heap<int>()).ToArray();

        var files = new List<(int id, int size, int pos)>();
        var id = 0;
        var isFree = false;
        var pos = 0;
        foreach (var c in input)
        {
            if (!isFree)
                files.Add((id++, c - '0', pos));
            else if (c != '0')
                freeBlocksBySize[c - '0'].Add(pos);

            pos += c - '0';
            isFree = !isFree;
        }

        var newFiles = new List<(int id, int size, int pos)>();
        for (var i = files.Count - 1; i >= 0; i--)
        {
            var file = files[i];

            var matchingFreeBlocks = freeBlocksBySize
                .Select((freeBlocks, size) => (freeBlocks, size))
                .Where(x => x.size >= file.size && x.freeBlocks.Count > 0 && x.freeBlocks.Min < file.pos)
                .ToArray();

            if (matchingFreeBlocks.Length == 0)
            {
                newFiles.Add(file);
                continue;
            }

            var freeBlock = matchingFreeBlocks.MinBy(x => x.freeBlocks.Min);
            var newPos = freeBlock.freeBlocks.DeleteMin();
            newFiles.Add(file with { pos = newPos });

            if (freeBlock.size > file.size)
                freeBlocksBySize[freeBlock.size - file.size].Add(newPos + file.size);
        }

        return newFiles.Select(f => Enumerable.Range(0, f.size).Select(i => (f.pos + i) * (long)f.id).Sum()).Sum();
    }
}
