using System;
using System.Collections.Generic;
//TODO: struct na reprezentaciu citacich hlav s pouzitim With metod

interface IDeque<T> : IList<T>
{
    T[][] QueueMap { get; set; }
    void AddFront(T input);
    void AddBack(T input);
    T GetFront();
    T GetBack();
}

internal struct HeadPosition
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
}

