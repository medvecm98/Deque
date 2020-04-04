using System;
using System.Collections;
using System.Collections.Generic;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("XUnitTestProject1")]

public interface IDeque<T> : IList<T>
{
    void AddFront(T input);
    void AddBack(T input);
    T GetFront();
    T GetBack();
}



public class Deque<T> : IDeque<T>
{
    const int DATABLOCK_LENGTH = 256;
    const int DEFAULT_MAP_SIZE = 16;

    internal class HeadPosition
    {
        public int MapPosition;
        public int DataBlockOffset;

        public void Inc()
        {
            if (DataBlockOffset + 1 == DATABLOCK_LENGTH)
            {
                MapPosition++;
                DataBlockOffset = 0;
            }
            else
            {
                DataBlockOffset++;
            }
        }

        public void Dec()
        {
            if (DataBlockOffset - 1 < 0)
            {
                MapPosition--;
                DataBlockOffset = DATABLOCK_LENGTH - 1;
            }
            else
            {
                DataBlockOffset--;
            }
        }

        public HeadPosition IncNew()
        {
            if (DataBlockOffset + 1 == DATABLOCK_LENGTH)
            {
                return new HeadPosition() { MapPosition = MapPosition + 1, DataBlockOffset = 0 };
            }
            else
            {
                return new HeadPosition() { MapPosition = MapPosition, DataBlockOffset = DataBlockOffset + 1 };
            }
        }
        public HeadPosition DecNew()
        {
            if (DataBlockOffset - 1 < 0)
            {
                return new HeadPosition() { MapPosition = MapPosition - 1, DataBlockOffset = DATABLOCK_LENGTH - 1 };
            }
            else
            {
                return new HeadPosition() { MapPosition = MapPosition, DataBlockOffset = DataBlockOffset - 1 };
            }
        }

        public static bool operator !=(HeadPosition hp1, HeadPosition hp2)
        {
            if ((hp1.MapPosition == hp2.MapPosition) && (hp1.DataBlockOffset == hp2.DataBlockOffset))
                return false;
            return true;
        }

        public static bool operator ==(HeadPosition hp1, HeadPosition hp2)
        {
            if ((hp1.MapPosition == hp2.MapPosition) && (hp1.DataBlockOffset == hp2.DataBlockOffset))
                return true;
            return false;
        }

        public static int operator -(HeadPosition hp1, HeadPosition hp2)
        {
            int ihp1 = hp1.MapPosition * DATABLOCK_LENGTH + hp1.DataBlockOffset;
            int ihp2 = hp2.MapPosition * DATABLOCK_LENGTH + hp2.DataBlockOffset;

            return ihp1 - ihp2;
        }

        public override bool Equals(object obj)
        {
            if (obj is HeadPosition hp)
            {
                if ((this.MapPosition == hp.MapPosition) && (this.DataBlockOffset == hp.DataBlockOffset))
                    return true;
                return false;
            }
            return ((object)this).Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 149 + MapPosition;
            hash = hash * 149 + DataBlockOffset;
            return hash;
        }
    }

    int Size { get; set; }
    HeadPosition FrontHead { get; set; }
    HeadPosition RearHead { get; set; }
    T[][] QueueMap { get; set; }

    public int Count { get; private set; }
    public int MapCount { get; private set; }

    public virtual bool IsReadOnly => false;

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("Index was outside the range of Deque.");
            int totalIndex = GetTotalIndex(index);

            return QueueMap[totalIndex / DATABLOCK_LENGTH][totalIndex % DATABLOCK_LENGTH];
        }
        set
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("Index was outside the range of Deque.");
            int totalIndex = GetTotalIndex(index);

            this.QueueMap[totalIndex / DATABLOCK_LENGTH][totalIndex % DATABLOCK_LENGTH] = value;
        }
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + Size;
        hash = hash * 23 + Count;
        hash = hash * 23 + MapCount;
        hash = hash * 23 + RearHead.MapPosition;
        hash = hash * 23 + RearHead.DataBlockOffset;
        hash = hash * 23 + FrontHead.MapPosition;
        hash = hash * 23 + FrontHead.DataBlockOffset;
        for (int i = 0; i < Count; i++)
        {
            hash = hash * 23 + this[i].GetHashCode();
        }
        return hash;
    }

    public Deque()
    {
        Size = DEFAULT_MAP_SIZE;
        Count = 0;
        MapCount = 0;
        QueueMap = new T[DEFAULT_MAP_SIZE][];
        FrontHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = (DATABLOCK_LENGTH / 2) };
        RearHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = (DATABLOCK_LENGTH / 2) - 1 };
    }

    private void ReallocateMap()
    {
        Size *= 2;
        T[][] toReturn = new T[Size][];
        for (int i = RearHead.MapPosition; i <= FrontHead.MapPosition; i++)
        {
            toReturn[i + ((Size - MapCount) / 2)] = QueueMap[i];
        }
        RearHead.MapPosition = RearHead.MapPosition + ((Size - MapCount) / 2);
        FrontHead.MapPosition = FrontHead.MapPosition + ((Size - MapCount) / 2);
        QueueMap = toReturn;
    }

    private void AllocateNewDataBlock(HeadPosition where)
    {
        MapCount++;
        QueueMap[where.MapPosition] = new T[DATABLOCK_LENGTH];
    }

    private void MoveFrontHead()
    {
        if ((FrontHead.DataBlockOffset + 1) == DATABLOCK_LENGTH)
        {
            if (FrontHead.MapPosition + 1 >= Size)
                ReallocateMap();
            FrontHead.MapPosition++;
            FrontHead.DataBlockOffset = 0;
        }
        else
        {
            FrontHead.DataBlockOffset++;
        }
    }

    private void MoveRearHead()
    {
        if ((RearHead.DataBlockOffset - 1) < 0)
        {
            if (RearHead.MapPosition - 1 <= 0)
                ReallocateMap();
            RearHead.MapPosition--;
            RearHead.DataBlockOffset = DATABLOCK_LENGTH - 1;
        }
        else
        {
            RearHead.DataBlockOffset--;
        }
    }

    public void AddFront(T input)
    {
        if (FrontHead.MapPosition >= Size)
            ReallocateMap();
        if (QueueMap[FrontHead.MapPosition] == null)
            AllocateNewDataBlock(FrontHead);
        QueueMap[FrontHead.MapPosition][FrontHead.DataBlockOffset] = input;
        Count++;
        MoveFrontHead();
    }

    public void AddBack(T input)
    {
        if (RearHead.MapPosition <= 0)
            ReallocateMap();
        if (QueueMap[RearHead.MapPosition] == null)
            AllocateNewDataBlock(RearHead);
        QueueMap[RearHead.MapPosition][RearHead.DataBlockOffset] = input;
        Count++;
        MoveRearHead();
    }

    public T GetFront()
    {
        if (Count == 0)
            throw new InvalidOperationException("Deque empty.");
        if ((FrontHead.DataBlockOffset - 1) < 0)
        {
            FrontHead.MapPosition--;
            FrontHead.DataBlockOffset = DATABLOCK_LENGTH - 1;
        }
        else
        {
            FrontHead.DataBlockOffset--;
        }
        T toReturn = QueueMap[FrontHead.MapPosition][FrontHead.DataBlockOffset];
        //if (FrontHead.DataBlockOffset == 0)
        //    QueueMap[FrontHead.MapPosition] = null;
        //else
        QueueMap[FrontHead.MapPosition][FrontHead.DataBlockOffset] = default(T);
        Count--;
        return toReturn;
    }

    public T GetBack()
    {
        if (Count == 0)
            throw new InvalidOperationException("Deque empty.");
        if ((RearHead.DataBlockOffset + 1) == DATABLOCK_LENGTH)
        {
            RearHead.MapPosition++;
            RearHead.DataBlockOffset = 0;
        }
        else
        {
            RearHead.DataBlockOffset++;
        }
        T toReturn = QueueMap[RearHead.MapPosition][RearHead.DataBlockOffset];
        //if (RearHead.DataBlockOffset == (DATABLOCK_LENGTH - 1))
        //    QueueMap[RearHead.MapPosition] = null;
        //else
        QueueMap[RearHead.MapPosition][RearHead.DataBlockOffset] = default(T);
        Count--;
        return toReturn;

    }

    public int IndexOf(T item)
    {
        int j = 0;
        for (var i = RearHead.IncNew(); i != FrontHead; i.Inc())
        {
            if (QueueMap[i.MapPosition][i.DataBlockOffset] == null)
            {
                if (item == null)
                    return j;
            }
            else if (QueueMap[i.MapPosition][i.DataBlockOffset].Equals(item))
                return j;
            j++;
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > Count)
            throw new ArgumentOutOfRangeException();
        if (this.IsReadOnly)
            throw new NotSupportedException("Deque is readonly.");

        if (index == 0)
        {
            AddBack(item);
            return;
        }

        if (index == Count)
        {
            AddFront(item);
            return;
        }

        int totalIndex = GetTotalIndex(index);
        HeadPosition headPosition = new HeadPosition()
        {
            MapPosition = totalIndex / DATABLOCK_LENGTH,
            DataBlockOffset = (totalIndex % DATABLOCK_LENGTH)
        };

        if (headPosition - RearHead < FrontHead - headPosition)
        {
            T last = QueueMap[headPosition.MapPosition][headPosition.DataBlockOffset];
            QueueMap[headPosition.MapPosition][headPosition.DataBlockOffset] = item;
            for (var head = headPosition.IncNew();
                     head != FrontHead;
                     head.Inc())
            {
                T temp = QueueMap[head.MapPosition][head.DataBlockOffset];
                QueueMap[head.MapPosition][head.DataBlockOffset] = last;
                last = temp;
            }
            AddFront(last);
        }
        else
        {
            headPosition.Dec();
            T last = QueueMap[headPosition.MapPosition][headPosition.DataBlockOffset];
            QueueMap[headPosition.MapPosition][headPosition.DataBlockOffset] = item;
            for (var head = headPosition.DecNew();
                     head != RearHead;
                     head.Dec())
            {
                T temp = QueueMap[head.MapPosition][head.DataBlockOffset];
                QueueMap[head.MapPosition][head.DataBlockOffset] = last;
                last = temp;
            }
            AddBack(last);
        }
    }

    public int GetTotalIndex(int index)
    {
        return RearHead.MapPosition * DATABLOCK_LENGTH + RearHead.DataBlockOffset + 1 + index;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException();

        if (index == 0)
        {
            GetBack();
            return;
        }

        if (index == Count - 1)
        {
            GetFront();
            return;
        }

        int totalIndex = GetTotalIndex(index);
        HeadPosition headPosition = new HeadPosition()
        {
            MapPosition = totalIndex / DATABLOCK_LENGTH,
            DataBlockOffset = (totalIndex % DATABLOCK_LENGTH)
        };

        if (headPosition - RearHead < FrontHead - headPosition)
        {
            for (var head = headPosition.IncNew();
                head != FrontHead;
                head.Inc())
            {
                QueueMap[headPosition.MapPosition][headPosition.DataBlockOffset] = QueueMap[head.MapPosition][head.DataBlockOffset];
                headPosition.Inc();
            }
            GetFront();
        }
        else
        {
            for (var head = headPosition.DecNew();
                head != RearHead;
                head.Dec())
            {
                QueueMap[headPosition.MapPosition][headPosition.DataBlockOffset] = QueueMap[head.MapPosition][head.DataBlockOffset];
                headPosition.Dec();
            }
            GetBack();
        }
    }

    public void Add(T item)
    {
        this.AddFront(item);
    }

    public void Clear()
    {
        Size = DEFAULT_MAP_SIZE;
        Count = 0;
        MapCount = 0;
        QueueMap = new T[DEFAULT_MAP_SIZE][];
        FrontHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = (DATABLOCK_LENGTH / 2) };
        RearHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = (DATABLOCK_LENGTH / 2) - 1 };
    }

    public bool Contains(T item)
    {
        if (IndexOf(item) == -1)
            return false;
        return true;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array.Rank != 1)
            throw new ArgumentException("Array is multidimensional.");
        if (Count > (array.Length - arrayIndex))
            throw new ArgumentException("Not enough space is left in destination array.");
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException("Index is less than zero.");
        if (array == null)
            throw new ArgumentNullException("Input array is null.");

        int j = 0;
        for (int i = arrayIndex; j < Count; i++)
        {
            array[i] = this[j];
            j++;
        }
    }

    public bool Remove(T item)
    {
        int i;
        if ((i = IndexOf(item)) != -1)
        {
            this.RemoveAt(i);
            return true;
        }
        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new DequeEnumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

public class DequeEnumerator<T> : IEnumerator<T>
{
    int index = -1;
    int hash = 0;
    IDeque<T> deque;

    public T Current
    {
        get
        {

            if (index < 0 || index >= deque.Count)
                throw new InvalidOperationException();
            return deque[index];
        }
    }

    object IEnumerator.Current => throw new NotImplementedException();

    public void Dispose()
    {
        deque = null;
    }

    public bool MoveNext()
    {
        index++;
        if (hash != deque.GetHashCode())
            throw new InvalidOperationException();
        if (index < 0 || index >= deque.Count)
            return false;
        return true;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public DequeEnumerator(IDeque<T> input)
    {
        deque = input;
        hash = deque.GetHashCode();
    }
}

public class ReverseDeque<T> : IDeque<T>, IEnumerable
{
    public int Count
    {
        get
        {
            return deque.Count;
        }
    }

    public bool IsReadOnly => false;

    public ReverseDeque(Deque<T> input)
    {
        deque = input;
    }
    Deque<T> deque;
    public T this[int index]
    {
        get
        {
            return deque[deque.Count - 1 - index];
        }
        set
        {
            deque[deque.Count - 1 - index] = value;
        }
    }

    public void Add(T item)
    {
        deque.AddBack(item);
    }

    public void AddBack(T input)
    {
        deque.AddFront(input);
    }

    public void AddFront(T input)
    {
        deque.AddBack(input);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array.Rank != 1)
            throw new ArgumentException("Array is multidimensional.");
        if (Count > (array.Length - arrayIndex))
            throw new ArgumentException("Not enough space is left in destination array.");
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException("Index is less than zero.");
        if (array == null)
            throw new ArgumentNullException("Input array is null.");
        int j = Count;
        for (int i = arrayIndex; j >= 0; i++)
        {
            array[i] = this[j];
            j--;
        }
    }

    public T GetBack() => deque.GetFront();

    public IEnumerator<T> GetEnumerator()
    {
        return new DequeEnumerator<T>(this);
    }

    public T GetFront() => deque.GetBack();

    public int IndexOf(T item)
    {
        for (int i = 0; i < Count; i++)
        {
            if (this[i] == null)
            {
                if (item == null)
                    return i;
            }
            else if (this[i].Equals(item))
                return i;
        }
        return -1;
    }


    public void Insert(int index, T item)
    {
        if (deque.Count == 0)
            deque.Insert(0, item);
        else
            deque.Insert(deque.Count - 1 - index, item);
    }

    public bool Remove(T item) => deque.Remove(item);

    public void RemoveAt(int index)
    {
        deque.RemoveAt(deque.Count - 1 - index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        deque.Clear();
    }

    public bool Contains(T item) => deque.Contains(item);

}


public static class DequeTest
{
    public static IList<T> GetReverseView<T>(Deque<T> d)
    {
        return new ReverseDeque<T>(d);
    }
}