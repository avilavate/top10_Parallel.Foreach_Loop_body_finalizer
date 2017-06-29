using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top10_parallel_foreach
{
    class parser
    {
        public IEnumerable<string> parse(string file = @"./ratings.txt")
        {
            return File.ReadAllLines(file);
        }

        public string GetUserId(string line)
        {
            var records = line.Split(',');
            return records[1];
        }
    }
}
