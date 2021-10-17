using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPathFinder
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;


    GridXZ<PathNode> grid;
    List<PathNode> OpenNodes, ClosedNodes;

    public GridPathFinder(int width, int height)
    {
        grid = new GridXZ<PathNode>(width, height, 1f, Vector3.zero, (GridXZ<PathNode> g, int x, int y) => new PathNode(g, x, y));

    }

    public List<PathNode> FindPath(int start_x, int start_y, int end_x, int end_y)
    {
        PathNode startNode = grid.GetGridObject(start_x, start_y);
        PathNode endNode = grid.GetGridObject(end_x, end_y);
        OpenNodes = new List<PathNode>() { startNode};
        ClosedNodes = new List<PathNode>();

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
            PathNode CurrentNode = GetLowestFcostNode(OpenNodes);
            if (CurrentNode == endNode)
            {
                Debug.Log("a Path had been found");
                return CalculatePath(endNode);
            }

            OpenNodes.Remove(CurrentNode);
            ClosedNodes.Add(CurrentNode);

            foreach (PathNode neighbor in CurrentNode.Neighbors)
            {
                Debug.Log($"neighbor is null = {neighbor == null}");
            }
            foreach (PathNode neighbor in CurrentNode.Neighbors)
            {
                if (ClosedNodes.Contains(neighbor)) continue;

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
        Debug.Log("No PathFound");
        return null;
    }

    private int Calculat_distance_cost(PathNode a, PathNode b)
    {
        int xDistance = Math.Abs(a.x - b.x);
        int yDistance = Math.Abs(a.y - b.y);
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
        return Path;
    }

    public GridXZ<PathNode> GetGrid() 
    {
        return grid;
    }
}
