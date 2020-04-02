using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Deque<int> d = new Deque<int>();
            d.AddFront(4); 
            d.AddFront(5);

            d.AddFront(6);
            d.AddFront(7);
            d.AddFront(8);
            d.AddFront(9);

            d.AddBack(3);
            d.AddBack(2);
            d.AddBack(1);
            d.RemoveAt(1);
            d.Insert(1, 12);
            d.Insert(1, 13);
            bool b = d.Contains(13);
            bool b2 = d.Contains(14);
            int i = d.IndexOf(7);
            int i1 = d.IndexOf(0);
            int i2 = d.IndexOf(1);
            int i3 = d[1];
            d[1] = 200;
            int i4 = d[1];
            //Console.WriteLine(d[-1]);
            Console.WriteLine(d[9]);
            Console.WriteLine(d[10]);
            //d.GetFront();
        }
    }
}
