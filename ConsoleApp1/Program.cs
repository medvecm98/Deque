using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var d = new Deque<int>();
            IDeque<int> rev = new ReverseDeque<int>(d);
            d.Insert(d.Count, 2);
            d.Insert(d.Count, 2);
            d.RemoveAt(0);
        }
    }
}
