using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject<T> 
{
    public GridXZ<T> grid;
    public int x, z;
    public List<T> Neighbors;
    public GridState State = GridState.Free;

    public T
        UpNeighbor,
        UpRightNeighbor,
        RightNeighbor,
        RightDownNeighbor,
        DownNeighbor,
        LeftDownNeighbor,
        LeftNeighbor,
        LeftUpNeighbor
        ;
    public enum GridState
    {
        Free,
        BookedUp
    }
    public void SetNeighbors()
    {
        Neighbors = grid.GetNeighbors(x, z,
            out UpNeighbor,
            out UpRightNeighbor,
            out RightNeighbor,
            out RightDownNeighbor,
            out DownNeighbor,
            out LeftDownNeighbor,
            out LeftNeighbor,
            out LeftUpNeighbor
            );
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(x, z);
    }

    public override string ToString()
    {
        return $"{x} , {z}";
    }

}