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
    public byte DataBlockOffset;
}

public class Deque<T> : IDeque<T>
{
    const int DATABLOCK_LENGTH = 256;
    const int DEFAULT_MAP_SIZE = 16;

    int Size { get; set; }
    HeadPosition FrontHead { get; set; }
    HeadPosition RearHead { get; set; }
    T[][] QueueMap { get; set; }

    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => false;

    public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Deque()
    {
        Size = DEFAULT_MAP_SIZE;
        QueueMap = new T[DEFAULT_MAP_SIZE][];
        FrontHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = DATABLOCK_LENGTH / 2 };
        RearHead = new HeadPosition() { MapPosition = (Size / 2), DataBlockOffset = (DATABLOCK_LENGTH / 2) + 1 };
    }

    public void AddFront(T input)
    {
        if (QueueMap[FrontHead.MapPosition] == null)
            QueueMap[FrontHead.MapPosition] = new T[DATABLOCK_LENGTH];
        QueueMap[FrontHead.MapPosition][FrontHead.DataBlockOffset] = input;
        if ((FrontHead.DataBlockOffset + 1) == DATABLOCK_LENGTH)
        {
            FrontHead.MapPosition++;
            FrontHead.DataBlockOffset = 0;
        }
        else
        {
            FrontHead.DataBlockOffset++;
        }
    }

    public void AddBack(T input)
    {
        if (QueueMap[RearHead.MapPosition] == null)
            QueueMap[RearHead.MapPosition] = new T[DATABLOCK_LENGTH];
        QueueMap[RearHead.MapPosition][RearHead.DataBlockOffset] = input;
        if ((RearHead.DataBlockOffset - 1) < 0)
        {
            RearHead.MapPosition--;
            RearHead.DataBlockOffset = DATABLOCK_LENGTH - 1;
        }
        else
        {
            RearHead.DataBlockOffset--;
        }
    }

    public T GetFront()
    {
        if ((FrontHead.DataBlockOffset - 1) < 0)
        {
            QueueMap[FrontHead.MapPosition] = null;
            FrontHead.MapPosition--;
            FrontHead.DataBlockOffset = DATABLOCK_LENGTH - 1;
        }
        else
        {
            FrontHead.DataBlockOffset--;
        }
        T toReturn = QueueMap[FrontHead.MapPosition][FrontHead.DataBlockOffset];
        QueueMap[FrontHead.MapPosition][FrontHead.DataBlockOffset] = default(T);
        return toReturn;
    }

    public T GetBack()
    {
        if ((RearHead.DataBlockOffset + 1) == DATABLOCK_LENGTH)
        {
            QueueMap[RearHead.MapPosition] = null;
            RearHead.MapPosition++;
            RearHead.DataBlockOffset = 0;
        }
        else
        {
            RearHead.DataBlockOffset++;
        }
        T toReturn = QueueMap[RearHead.MapPosition][RearHead.DataBlockOffset];
        QueueMap[RearHead.MapPosition][RearHead.DataBlockOffset] = default(T);
        return toReturn;

    }

    public int IndexOf(T item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
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

