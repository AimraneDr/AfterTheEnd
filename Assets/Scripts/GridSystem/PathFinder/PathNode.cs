using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathNode : GridObject<PathNode>, IHeapItem<PathNode>
{
   
    public int G_Cost, H_Cost, F_Cost;
    public PathNode CameFromNode;
    public bool IsWalkable
    {
        get { return State == GridState.Free; }
    }

    private int heapIndex;
    public int HeapIndex
    {
        get => heapIndex;
        set => heapIndex = value;
    }

    public PathNode(GridXZ<PathNode> _grid,int _x,int _z)
    {
        grid = _grid;
        x = _x;
        z = _z;
        //Neighbors = grid.GetNeighbors(x, y);

    }
   
    public void CalculateFCost()
    {
        F_Cost = G_Cost + H_Cost;
    }

    public int CompareTo(PathNode nodeToCompare)
    {
        int compare = F_Cost.CompareTo(nodeToCompare.F_Cost);
        if (compare == 0)
        {
            compare = H_Cost.CompareTo(nodeToCompare.H_Cost);
        }
        return -compare;
    }

}

