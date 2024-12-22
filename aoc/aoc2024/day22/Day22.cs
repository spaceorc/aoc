using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc.aoc2024.day22;

public class Day22(long[] input)
{
    public void Solve()
    {
        // CalcVariant2(123, [6, -1, -1, 0]).Out("x: ");
        // return;
        Part1().Out("Part 1: ");
        Part2().Out("Part 2: ");
    }

    private long Part1() => input.Sum(x => Next(x, 2000));

    private long Part2()
    {
        // var changeToPrice = new Dictionary<long[], long>(ArrayEqualityComparer.Create<long>()).ToDefault();
        //
        // for (int i = 0; i < input.Length; i++)
        // {
        //     var values = input[i].Generate(Next).Take(2001).ToArray();
        //     var usedChanges = new HashSet<long[]>(ArrayEqualityComparer.Create<long>());
        //     foreach (var p in values.Select(x => x % 10).SlidingWindow(5))
        //     {
        //         var change = new[]
        //         {
        //             p[1] - p[0],
        //             p[2] - p[1],
        //             p[3] - p[2],
        //             p[4] - p[3],
        //         };
        //         var price = p[4];
        //         if (usedChanges.Add(change))
        //             changeToPrice[change] += price;   
        //     }
        // }
        //
        // return changeToPrice.Values.Max();

        var nexts = Enumerable.Range(0, 0x1000000).Select(x => Next(x)).ToArray();
        
        var changes = new long[0x1000000];
        for (long i = 0; i < 0x1000000; i++)
        {
            var (p, n) = (i, Next(i));
            var c = (n % 10 - p % 10) + 9;
            (p, n) = (n, Next(n));
            c = c * 19 + (n % 10 - p % 10) + 9;
            (p, n) = (n, Next(n));
            c = c * 19 + (n % 10 - p % 10) + 9;
            (p, n) = (n, Next(n));
            c = c * 19 + (n % 10 - p % 10) + 9;
            changes[n] = c;
        }
        
        var clients = input.Select(ii => Next(ii, 4)).ToArray();
        
        var clientChangePrice = new long[clients.Length, 19*19*19*19];
        for (int i = 0; i < clientChangePrice.GetLength(0); i++)
        for (int j = 0; j < clientChangePrice.GetLength(1); j++)
        {
            clientChangePrice[i, j] = -1;
        }
        
        // var cc = new[] { -2, 1, -1, 3 };
        // Console.WriteLine((cc[0] + 9) * 19 * 19 * 19 + (cc[1] + 9) * 19 * 19 + (cc[2] + 9) * 19 + (cc[3] + 9));
        
        for (long i = 0; i <= 2000 - 4; i++)
        {
            if (i % 0x10000 == 0)
                Console.WriteLine(i);
            
            for (int j = 0; j < clients.Length; j++)
            {
                var p = clients[j];
                var c = changes[p];
                if (clientChangePrice[j, c] == -1)
                    clientChangePrice[j, c] = p % 10;
                clients[j] = nexts[clients[j]];
            }
        }
        //
        // for (int i = 0; i < clients.Length; i++)
        // {
        //     Console.WriteLine(clientChangePrice[i, 51787]);
        // }
        
        var maxTotalPrice = 0L;
        for (int i = 0; i < clientChangePrice.GetLength(1); i++)
        {
            var totalPrice = 0L;
            for (int j = 0; j < clientChangePrice.GetLength(0); j++)
            {
                if (clientChangePrice[j, i] != -1)
                    totalPrice += clientChangePrice[j, i];
            }
        
            maxTotalPrice = Math.Max(maxTotalPrice, totalPrice);
        }
        
        return maxTotalPrice;

        // var values = 0L.Generate(Next).Take(0x1000000).ToArray();
        // var prices = Enumerable.Range(0, 0x1000000).Select(x => x % 10).ToArray();
        // var changes = prices.Select((p, i) => p - prices[i.Mod(prices.Length)]).ToArray();
        // var changeHashes = changes.Select(
        //         (_, i) =>
        //             (changes[(i - 0).Mod(4)] + 9) * 1 +
        //             (changes[(i - 1).Mod(4)] + 9) * 19 +
        //             (changes[(i - 2).Mod(4)] + 9) * 19 * 19 +
        //             (changes[(i - 3).Mod(4)] + 9) * 19 * 19 * 19
        //     )
        //     .ToArray();
        //
        // var clients = input.Select(ii => Next(ii, 4)).ToArray();
        //
        // for (int i = 0; i < 0x1000000; i++)
        // {
        //
        //     for (int j = 0; j < clients.Length; j++)
        //     {  
        //         
        //         
        //     }
        //     
        // }


        // return 0l;

        // return Combinatorics
        //     .Variants(4, 19)
        //     .Max(x => CalcVariant(x.Select(y => y - 9L).ToArray()));
    }

    private static long Next(long x)
    {
        x = ((x << 6) ^ x) % 0x1000000;
        x = ((x >> 5) ^ x) % 0x1000000;
        x = ((x << 11) ^ x) % 0x1000000;
        return x;
    }

    private static long Next(long x, long n)
    {
        var cur = x;
        for (var i = 0; i < n; i++)
            cur = Next(cur);
        return cur;
    }
}
