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
            d.RemoveAt(4);
            d.Insert(4, 12);
            d.Insert(4, 13);
            //d.GetFront();
        }
    }
}
