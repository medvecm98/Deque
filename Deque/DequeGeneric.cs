using System;
using System.Collections.Generic;

namespace Deque
{
    interface IDeque<T> : IList<T>
    {
        void AddFront(T input);
        void AddBack(T input);
        T GetFront();
        T GetBack();
    }
    public class Deque<T> : IDeque<T>
    {
    }
}
