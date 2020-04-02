using System;
using System.Collections;
using System.Collections.Generic;

interface IDeque<T> : IList<T>
{
    void AddFront(T input);
    void AddBack(T input);
    T GetFront();
    T GetBack();
}



public class Deque<T> : IDeque<T>
{
    const int DATABLOCK_LENGTH = 4;
    const int DEFAULT_MAP_SIZE = 2;

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
            int totalIndex = RearHead.MapPosition * DATABLOCK_LENGTH + RearHead.DataBlockOffset + 1 + index;
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("Index was outside the range of Deque.");
            return QueueMap[totalIndex / DATABLOCK_LENGTH][totalIndex % DATABLOCK_LENGTH];
        }
        set
        {
            int totalIndex = GetTotalIndex(index);
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("Index was outside the range of Deque.");
            this.QueueMap[totalIndex / DATABLOCK_LENGTH][totalIndex % DATABLOCK_LENGTH] = value;
        }
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
        T[][] newArray;
        if (MapCount + 1 >= Size)
        {
            Size *= 2;
            newArray = new T[Size][];
            Array.Copy(QueueMap, RearHead.MapPosition, newArray, (Size - MapCount) / 2, FrontHead.MapPosition - RearHead.MapPosition + 1);
            FrontHead.MapPosition -= RearHead.MapPosition - ((Size - MapCount) / 2);
            RearHead.MapPosition -= RearHead.MapPosition - ((Size - MapCount) / 2);
        }
        else
        {
            newArray = new T[Size][];
            Array.Copy(QueueMap, RearHead.MapPosition, newArray, (Size - MapCount) / 2, FrontHead.MapPosition - RearHead.MapPosition + 1);
            FrontHead.MapPosition -= RearHead.MapPosition - ((Size - MapCount) / 2);
            RearHead.MapPosition -= RearHead.MapPosition - ((Size - MapCount) / 2);
        }
        QueueMap = newArray;
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
            if (RearHead.MapPosition - 1 < 0)
                ReallocateMap();
            RearHead.MapPosition--;
            RearHead.DataBlockOffset = DATABLOCK_LENGTH - 1;
        }
        else
        {
            RearHead.DataBlockOffset--;
        }
    }

    public virtual void AddFront(T input)
    {
        if (FrontHead.MapPosition < 0 || FrontHead.MapPosition >= Size || (QueueMap[FrontHead.MapPosition] == null))
            AllocateNewDataBlock(FrontHead);
        QueueMap[FrontHead.MapPosition][FrontHead.DataBlockOffset] = input;
        Count++;
        MoveFrontHead();
    }

    public virtual void AddBack(T input)
    {
        if (RearHead.MapPosition < 0 || RearHead.MapPosition >= Size || (QueueMap[RearHead.MapPosition] == null))
            AllocateNewDataBlock(RearHead);
        QueueMap[RearHead.MapPosition][RearHead.DataBlockOffset] = input;
        Count++;
        MoveRearHead();
    }

    public virtual T GetFront()
    {
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
        if (FrontHead.DataBlockOffset == 0)
            QueueMap[FrontHead.MapPosition] = null;
        else
            QueueMap[FrontHead.MapPosition][FrontHead.DataBlockOffset] = default(T);
        Count--;
        return toReturn;
    }

    public virtual T GetBack()
    {
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
        if (RearHead.DataBlockOffset == (DATABLOCK_LENGTH - 1))
            QueueMap[RearHead.MapPosition] = null;
        else
            QueueMap[RearHead.MapPosition][RearHead.DataBlockOffset] = default(T);
        Count--;
        return toReturn;

    }

    public int IndexOf(T item)
    {
        int r = 0;
        for (long i = RearHead.MapPosition * DATABLOCK_LENGTH + RearHead.DataBlockOffset + 1;
                  i < FrontHead.MapPosition * DATABLOCK_LENGTH + FrontHead.DataBlockOffset;
                  i++)
        {
            if (QueueMap[i / DATABLOCK_LENGTH][i % DATABLOCK_LENGTH].Equals(item))
            {
                return r;
            }
            r++;
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException();
        int totalIndex = GetTotalIndex(index);
        HeadPosition headPosition = new HeadPosition()
        {
            MapPosition = totalIndex / DATABLOCK_LENGTH,
            DataBlockOffset = (totalIndex % DATABLOCK_LENGTH)
        };

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

    public int GetTotalIndex(int index)
    {
        return RearHead.MapPosition * DATABLOCK_LENGTH + RearHead.DataBlockOffset + 1 + index;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException();
        int totalIndex = GetTotalIndex(index);
        HeadPosition headPosition = new HeadPosition()
        {
            MapPosition = totalIndex / DATABLOCK_LENGTH,
            DataBlockOffset = (totalIndex % DATABLOCK_LENGTH)
        };
        for (var head = headPosition.IncNew();
                head != FrontHead;
                head.Inc())
        {
            QueueMap[headPosition.MapPosition][headPosition.DataBlockOffset] = QueueMap[head.MapPosition][head.DataBlockOffset];
            headPosition.Inc();
        }
        GetFront();
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
        for (long i = RearHead.MapPosition * DATABLOCK_LENGTH + RearHead.DataBlockOffset + 1;
                  i < FrontHead.MapPosition * DATABLOCK_LENGTH + FrontHead.DataBlockOffset;
                  i++)
        {
            if (QueueMap[i / DATABLOCK_LENGTH][i % DATABLOCK_LENGTH].Equals(item))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public class DequeEnumerator : IEnumerator<T>
    {
        int index = -1;

        public T Current
        {
            get
            {
                return default(T);
            }
        }

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}

public static class DequeTest
{
    public static IList<T> GetReverseView<T>(Deque<T> d)
    {
        return d;
    }
}