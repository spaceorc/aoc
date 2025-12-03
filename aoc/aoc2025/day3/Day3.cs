using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2025.day3;

public class Day3(string[] input)
{
    public void Solve()
    {
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1()
    {
        return input.Select(Max2).Sum();

        long Max2(string s)
        {
            var max = long.MinValue;
            for (int i = 0; i < s.Length; i++)
            {
                for (int j = i + 1; j < s.Length; j++)
                {
                    var number = (s[i] - '0') * 10L + (s[j] - '0');
                    if (number > max)
                        max = number;
                }
            }

            return max;
        }
    }

    private long Part2()
    {
        return input.Select(Max12).Sum();

        long Max12(string s)
        {
            var cache = new Dictionary<(string, int, int), long>();
            return MaxN(s, 0, 12, cache);
        }

        long MaxN(string s, int startIndex, int N, Dictionary<(string, int, int), long> cache)
        {
            if (N == 0)
                return 0;
            
            if (startIndex == s.Length - N)
                return long.Parse(s[startIndex..]);
            
            var key = (s, startIndex, N);
            if (cache.TryGetValue(key, out var cachedValue))
                return cachedValue;

            var result = Math.Max(
                MaxN(s, startIndex + 1, N, cache),
                (s[startIndex] - '0') * Pow10(N - 1) + MaxN(s, startIndex + 1, N - 1, cache)
            );
            cache[key] = result;
            return result;
        }

        long Pow10(int exp)
        {
            long result = 1;
            for (int i = 0; i < exp; i++)
                result *= 10;
            return result;
        }
    }
}
