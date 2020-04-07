using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var d = new Deque<string>();
            IDeque<string> rev = new ReverseDeque<string>(d);
            d.Insert(0, null);
            d.Insert(0, null);
            d.Insert(0, "a");
            d.Insert(0, null);
            d.Insert(0, "a");

            Console.WriteLine(d.IndexOf("a"));
            Console.WriteLine(rev.IndexOf(null));
            rev.RemoveAt(rev.Count - 1);

            foreach(var i in d)
            {
                Console.WriteLine(i);
                d.Insert(1, "b");
            }

            foreach(var j in rev)
            {
                Console.WriteLine(j);
            }
        }
    }
}
