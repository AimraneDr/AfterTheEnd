using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject<T> 
{
    public GridXZ<T> grid;
    public int x, z;
    public List<T> Neighbors;
    public GridState State = GridState.Free;
   

    public enum GridState
    {
        Free,
        BookedUp
    }
    public void SetNeighbors()
    {
        Neighbors = grid.GetNeighbors(x, z);
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