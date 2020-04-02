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

internal class HeadPosition
{
    public int MapPosition;
    public int DataBlockOffset;
}

public class Deque<T> : IDeque<T>
{
    const int DATABLOCK_LENGTH = 4;
    const int DEFAULT_MAP_SIZE = 2;

    int Size { get; set; }
    HeadPosition FrontHead { get; set; }
    HeadPosition RearHead { get; set; }
    T[][] QueueMap { get; set; }

    public int Count { get; private set; }

    public virtual bool IsReadOnly => false;

    public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Deque()
    {
        Size = DEFAULT_MAP_SIZE;
        Count = 0;
        QueueMap = new T[DEFAULT_MAP_SIZE][];
        FrontHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = (DATABLOCK_LENGTH / 2) };
        RearHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = (DATABLOCK_LENGTH / 2) - 1 };
    }

    private void ReallocateMap()
    {
        T[][] newArray;
        if (Count == Size)
        {
            Size *= 2;
            newArray = new T[Size][];
            Array.Copy(QueueMap, RearHead.MapPosition, newArray, (Size - Count) / 2, FrontHead.MapPosition - RearHead.MapPosition + 1);
            FrontHead.MapPosition -= RearHead.MapPosition - ((Size - Count) / 2);
            RearHead.MapPosition -= RearHead.MapPosition - ((Size - Count) / 2);
        }
        else
        {
            newArray = new T[Size][];
            Array.Copy(QueueMap, RearHead.MapPosition, newArray, (Size - Count) / 2, FrontHead.MapPosition - RearHead.MapPosition + 1);
            FrontHead.MapPosition -= RearHead.MapPosition - ((Size - Count) / 2);
            RearHead.MapPosition -= RearHead.MapPosition - ((Size - Count) / 2);
        }
        QueueMap = newArray;
    }

    private void AllocateNewDataBlock(HeadPosition where)
    {
        Count++;
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
        if (QueueMap[FrontHead.MapPosition] == null)
            AllocateNewDataBlock(FrontHead);
        QueueMap[FrontHead.MapPosition][FrontHead.DataBlockOffset] = input;
        MoveFrontHead();
    }

    public virtual void AddBack(T input)
    {
        if (QueueMap[RearHead.MapPosition] == null)
            AllocateNewDataBlock(RearHead);
        QueueMap[RearHead.MapPosition][RearHead.DataBlockOffset] = input;
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
        return toReturn;

    }

    public int IndexOf(T item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, T item)
    {
        HeadPosition headPosition = new HeadPosition()
        {
            MapPosition = index / DATABLOCK_LENGTH,
            DataBlockOffset = (index % DATABLOCK_LENGTH)
        };
        
        T last = QueueMap[headPosition.MapPosition][headPosition.DataBlockOffset];
        QueueMap[headPosition.MapPosition][headPosition.DataBlockOffset] = item;
        for (int i = headPosition.MapPosition * DATABLOCK_LENGTH + headPosition.DataBlockOffset + 1; 
                i < (FrontHead.MapPosition * DATABLOCK_LENGTH + FrontHead.DataBlockOffset); 
                i++)
        {
            T temp = QueueMap[i / DATABLOCK_LENGTH][i % DATABLOCK_LENGTH];
            QueueMap[i / DATABLOCK_LENGTH][i % DATABLOCK_LENGTH] = last;
            last = temp;
        }
        AddFront(last);
    }

    public void RemoveAt(int index)
    {
        HeadPosition headPosition = new HeadPosition()
        {
            MapPosition = index / DATABLOCK_LENGTH,
            DataBlockOffset = (index % DATABLOCK_LENGTH)
        };
        for (int i = headPosition.MapPosition * DATABLOCK_LENGTH + headPosition.DataBlockOffset + 1;
                i < (FrontHead.MapPosition * DATABLOCK_LENGTH + FrontHead.DataBlockOffset);
                i++)
        {
            QueueMap[(i - 1) / DATABLOCK_LENGTH][(i - 1) % DATABLOCK_LENGTH] = QueueMap[i / DATABLOCK_LENGTH][i % DATABLOCK_LENGTH];
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
        QueueMap = new T[DEFAULT_MAP_SIZE][];
        FrontHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = (DATABLOCK_LENGTH / 2) };
        RearHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = (DATABLOCK_LENGTH / 2) - 1 };
    }

    public bool Contains(T item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}