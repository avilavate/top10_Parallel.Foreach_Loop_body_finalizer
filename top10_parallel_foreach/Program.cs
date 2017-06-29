using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top10_parallel_foreach
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Parallel.ForEach with loop init, loop body & loop finalizer");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            parser p = new parser();
            sw.Restart();
            Dictionary<string, int> result = new Dictionary<string, int>();
            var loopResult = Parallel.ForEach(p.parse(),
                () =>
                {
                    var localDict = new Dictionary<string, int>();
                    return localDict;
                },
                (line, localtate, localDict) =>
                {
                    parser pl = new parser();
                    var userId = pl.GetUserId(line);
                    if (localDict.ContainsKey(userId)) localDict[userId]++;
                    else localDict.Add(userId, 1);
                    return localDict;
                },
                (localDict) =>
                {
                    lock (result)
                    {
                        foreach (var item in localDict)
                        {
                            if (result.ContainsKey(item.Key)) result[item.Key]+=item.Value;
                            else result.Add(item.Key, 1);
                        }
                    }
                });
            long timems = sw.ElapsedMilliseconds;
            Console.WriteLine();
            Console.WriteLine("** Top 10 users reviewing movies:");
            if (loopResult.IsCompleted)
            {
                var q = (from review in result.AsParallel()
                         orderby review.Value descending,
                         review.Key ascending
                         select new
                         {
                             userId = review.Key,
                             reviews = review.Value
                         }).
                       Take(10).
                       ToList();

                foreach (var item in q)
                {
                    Console.WriteLine($"{item.userId} : {item.reviews}");
                }


                double time = timems / 1000.0;  // convert milliseconds to secs

                Console.WriteLine();
                Console.WriteLine("** Done! Time: {0:0.000} secs", time);
                Console.WriteLine();
            }
        }
    }
}
