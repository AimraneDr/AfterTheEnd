using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class GridPathFinder
{
    Stopwatch sw;
    public GridXZ<BuildNode> BuildGridLevel;
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private int MaxSize
    {
        get { return grid.GetWidth() * grid.GetHeight(); }
    }

    GridXZ<PathNode> grid;
    Heap<PathNode> OpenNodes, ClosedNodes;

    public GridPathFinder(int width, int height)
    {
        grid = new GridXZ<PathNode>(
            width, 
            height, 
            1f, 
            Vector3.zero, 
            (GridXZ<PathNode> g, int x, int y) => new PathNode(g, x, y)
            );
        //grid.DebugIsVisible = true;
        
    }

    public void PassEvents()
    {
        BuildGridLevel.OnBookedUpGridsAdd += BookedUpPlacesIncreased;
        BuildGridLevel.OnBookedUpGridsRemove += BookedUpPlacesDencreased;
    }

    public void SetBookedUpNodes()
    {
        foreach(BuildNode BookedNode in BuildGridLevel.BookedUpGrids)
        {
            PathNode node = grid.GetGridObject(BookedNode.x, BookedNode.z);
            node.State = GridObject<PathNode>.GridState.BookedUp;
            grid.BookedUpGrids.Add(node);
        }
    }
    public List<PathNode> FindPath(int start_x, int start_y, int end_x, int end_y)
    {
        sw = new Stopwatch();
        sw.Start();
        PathNode startNode = grid.GetGridObject(start_x, start_y);
        PathNode endNode = grid.GetGridObject(end_x, end_y);
        OpenNodes = new Heap<PathNode>(MaxSize);
        ClosedNodes = new Heap<PathNode>(MaxSize);
        OpenNodes.Add(startNode);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode node = grid.GetGridObject(x, y);
                node.G_Cost = int.MaxValue;
                node.CalculateFCost();
                node.CameFromNode = null;
            }
        }

        startNode.G_Cost = 0;
        startNode.H_Cost = Calculat_distance_cost(startNode, endNode);
        startNode.CalculateFCost();

        while (OpenNodes.Count > 0)
        {
            PathNode CurrentNode = OpenNodes.RemoveFirst();
            if (CurrentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            ClosedNodes.Add(CurrentNode);

            foreach (PathNode neighbor in CurrentNode.Neighbors)
            {
                if (ClosedNodes.Contains(neighbor)) continue;
                if (!neighbor.IsWalkable)
                {
                    ClosedNodes.Add(neighbor);
                    continue;
                }

                int tentativeGCost = CurrentNode.G_Cost + Calculat_distance_cost(CurrentNode, neighbor);
                if (tentativeGCost < neighbor.G_Cost)
                {
                    neighbor.CameFromNode = CurrentNode;
                    neighbor.G_Cost = tentativeGCost;
                    neighbor.H_Cost = Calculat_distance_cost(neighbor, endNode);
                    neighbor.CalculateFCost();

                    if (!OpenNodes.Contains(neighbor)) OpenNodes.Add(neighbor);
                }
            }

        }
        return null;
    }

    private int Calculat_distance_cost(PathNode a, PathNode b)
    {
        int xDistance = Math.Abs(a.x - b.x);
        int yDistance = Math.Abs(a.z - b.z);
        int remaining = Math.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Math.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFcostNode(List<PathNode> NodesList)
    {
        PathNode LowestFCostNode = NodesList[0];
        for (int i = 1; i < NodesList.Count; i++)
        {
            if (LowestFCostNode.F_Cost > NodesList[i].F_Cost) LowestFCostNode = NodesList[i];
        }
        return LowestFCostNode;
    }

    private List<PathNode> CalculatePath(PathNode end_node)
    {
        List<PathNode> Path = new List<PathNode>();
        Path.Add(end_node);
        PathNode current_node = end_node;
        while (current_node.CameFromNode != null)
        {
            Path.Add(current_node.CameFromNode);
            current_node = current_node.CameFromNode;
        }
        Path.Reverse();
        sw.Stop();
        UnityEngine.Debug.Log($"path found in : ( {sw.ElapsedMilliseconds} )");
        return Path;
    }

    public GridXZ<PathNode> GetGrid() 
    {
        return grid;
    }

    private void BookedUpPlacesIncreased(GridXZ<BuildNode> gr,BuildNode BookedNode)
    {
        PathNode node = grid.GetGridObject(BookedNode.x, BookedNode.z);
        node.State = GridObject<PathNode>.GridState.BookedUp;
        grid.BookedUpGrids.Add(node);
    }
    private void BookedUpPlacesDencreased(GridXZ<BuildNode> gr, BuildNode LiberatedNode)
    {
        PathNode node = grid.GetGridObject(LiberatedNode.x, LiberatedNode.z);
        node.State = GridObject<PathNode>.GridState.Free;

       if (grid.BookedUpGrids.Contains(node))
            grid.BookedUpGrids.Remove(node);
    }
}
