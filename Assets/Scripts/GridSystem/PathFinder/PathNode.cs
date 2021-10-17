using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathNode
{
    GridXZ<PathNode> grid;
    public int x, y;
    public int G_Cost, H_Cost, F_Cost;
    public PathNode CameFromNode;
    public bool IsWalkable;

    //neighbors
    private static PathNode 
        UpNeighbor = null, 
        UpRightNeighbor = null,
        RightNeighbor = null,
        RightDownNeighbor = null,
        DownNeighbor = null,
        LeftDownNeighbor = null,
        LeftNeighbor = null, 
        LeftUpNeighbor = null
        ;
    public List<PathNode> Neighbors = new List<PathNode>()
    {
        UpNeighbor,
        UpRightNeighbor,
        RightNeighbor,
        RightDownNeighbor,
        DownNeighbor,
        LeftDownNeighbor,
        LeftNeighbor,
        LeftUpNeighbor
    };

    public PathNode(GridXZ<PathNode> _grid,int _x,int _y)
    {
        grid = _grid;
        x = _x;
        y = _y;
        grid.GetNeighbors(
            x,
            y,
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

    public void CalculateFCost()
    {
        F_Cost = G_Cost + H_Cost;
    }

}

