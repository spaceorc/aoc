using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day6;

public class Day6(string[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long SolveProblem((char op, long[] numbers) problem) =>
        problem.op switch
        {
            '+' => problem.numbers.Sum(),
            '*' => problem.numbers.Product(),
        };

    private long Part1()
    {
        return input
            .ToMapSeparated()
            .ColumnsValues()
            .Select(ParseProblem)
            .Sum(SolveProblem);

        (char op, long[] numbers) ParseProblem(string[] example) =>
        (
            op: example[^1][0],
            numbers: example
                .SkipLast(1)
                .Select(long.Parse)
                .ToArray()
        );
    }

    private long Part2()
    {
        return Map<char>.Parse(input)
            .ColumnsStrings()
            .Select(x => x.Trim())
            .Regions()
            .Select(ParseProblem)
            .Sum(SolveProblem);

        (char op, long[] numbers) ParseProblem(string[] example) =>
        (
            op: example[0][^1],
            numbers: example
                .Select((line, i) => i == 0 ? line[..^1] : line)
                .Select(l => l.Trim())
                .Select(long.Parse)
                .ToArray()
        );
    }
}
