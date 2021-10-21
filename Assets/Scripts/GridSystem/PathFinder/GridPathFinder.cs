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
        //UnityEngine.Debug.Log("Start fint Path nodes");
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

            CheckNeighbors(CurrentNode, endNode);
            //foreach (PathNode neighbor in CurrentNode.Neighbors)
            //{
            //    if (ClosedNodes.Contains(neighbor)) continue;
            //    if (!neighbor.IsWalkable)
            //    {
            //        ClosedNodes.Add(neighbor);
            //        continue;
            //    }

            //    int tentativeGCost = CurrentNode.G_Cost + Calculat_distance_cost(CurrentNode, neighbor);
            //    if (tentativeGCost < neighbor.G_Cost)
            //    {
            //        neighbor.CameFromNode = CurrentNode;
            //        neighbor.G_Cost = tentativeGCost;
            //        neighbor.H_Cost = Calculat_distance_cost(neighbor, endNode);
            //        neighbor.CalculateFCost();

            //        if (!OpenNodes.Contains(neighbor)) OpenNodes.Add(neighbor);
            //    }
            //}

        }
        return null;
    }

    public List<Vector3> FindPath(Vector3 StartWorldPosition,Vector3 TargetWorldPosition)
    {
        //UnityEngine.Debug.Log("Start fint Path positions");
        grid.GetXZ(StartWorldPosition, out int s_x, out int s_y);
        grid.GetXZ(TargetWorldPosition, out int x, out int y);
        List<PathNode> path = FindPath(s_x, s_y, x, y);
        if (path == null)
        {
            //UnityEngine.Debug.Log("Path Not found");
            return null;
        }
        else
        {
            //UnityEngine.Debug.Log("Path positions had been found");
            return new List<Vector3>(PathToWorldPostions(path.ToArray()));
        }
           
    }
    private int Calculat_distance_cost(PathNode a, PathNode b)
    {
        int xDistance = Math.Abs(a.x - b.x);
        int yDistance = Math.Abs(a.z - b.z);
        int remaining = Math.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Math.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
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
        //UnityEngine.Debug.Log($"path found in : ( {sw.ElapsedMilliseconds} )");
        return Path;
    }
    public PathNode[] SimplifyPath(List<PathNode> path)
    {
        List<PathNode> simplifiedPath = new List<PathNode>();
        Vector2 OldPoint = Vector2.zero;

        for(int i = 1; i < path.Count; i++)
        {
            Vector2 direction = new Vector2(path[i - 1].x - path[i].x, path[i - 1].z - path[i].z);
            if (direction != OldPoint) simplifiedPath.Add(path[i]);

            OldPoint = direction;
        }
        return simplifiedPath.ToArray();
    }
    public Vector3[] SimplifyPath(Vector3[] path)
    {
        if (path != null)
        {
            List<Vector3> simplifiedPath = new List<Vector3>();
            Vector2 OldPoint = Vector2.zero;

            for (int i = 1; i < path.Length; i++)
            {
                Vector2 direction = new Vector2(path[i - 1].x - path[i].x, path[i - 1].z - path[i].z);
                if (direction != OldPoint) simplifiedPath.Add(path[i - 1]);

                OldPoint = direction;
            }
            simplifiedPath.Add(path[path.Length - 1]);
            return simplifiedPath.ToArray();
        }
        else return null;
    }

    /// <summary>
    /// Convert the path from a PathNode array to a Vector3 array
    /// </summary>
    /// <returns> a Vector3 array with a constante y = 0 ;</returns>
    public Vector3[] PathToWorldPostions(PathNode[] path)
    {
        //UnityEngine.Debug.Log("Convert Path nodes to path Postions");
        Vector3[] path_vectors = new Vector3[path.Length];
        for(int i=0; i < path.Length; i++)
        {
            path_vectors[i] = new Vector3(path[i].GetPosition().x, 0, path[i].GetPosition().y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
        }
        return path_vectors;
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

    private void CheckNeighbor(PathNode currentNode, PathNode neighbor, PathNode endNode)
    {
        if (!ClosedNodes.Contains(neighbor))
        {
            if (!neighbor.IsWalkable)
            {
                ClosedNodes.Add(neighbor);

            }
            else
            {
                int tentativeGCost = currentNode.G_Cost + Calculat_distance_cost(currentNode, neighbor);
                if (tentativeGCost < neighbor.G_Cost)
                {
                    neighbor.CameFromNode = currentNode;
                    neighbor.G_Cost = tentativeGCost;
                    neighbor.H_Cost = Calculat_distance_cost(neighbor, endNode);
                    neighbor.CalculateFCost();

                    if (!OpenNodes.Contains(neighbor)) OpenNodes.Add(neighbor);
                }
            }
        }
    }
    private void CheckNeighbors(PathNode CurrentNode,PathNode endNode)
    {
        bool UpIsValid = false,
            RightIsValid = false,
            DownIsValid = false,
            LeftIsValid = false;

        if (CurrentNode.RightNeighbor != null)
        {
            if (!ClosedNodes.Contains(CurrentNode.RightNeighbor))
            {
                if (!CurrentNode.RightNeighbor.IsWalkable)
                {
                    ClosedNodes.Add(CurrentNode.RightNeighbor);
                    RightIsValid = false;
                }
                else
                {
                    int tentativeGCost = CurrentNode.G_Cost + Calculat_distance_cost(CurrentNode, CurrentNode.RightNeighbor);
                    if (tentativeGCost < CurrentNode.RightNeighbor.G_Cost)
                    {
                        CurrentNode.RightNeighbor.CameFromNode = CurrentNode;
                        CurrentNode.RightNeighbor.G_Cost = tentativeGCost;
                        CurrentNode.RightNeighbor.H_Cost = Calculat_distance_cost(CurrentNode.RightNeighbor, endNode);
                        CurrentNode.RightNeighbor.CalculateFCost();

                        if (!OpenNodes.Contains(CurrentNode.RightNeighbor)) OpenNodes.Add(CurrentNode.RightNeighbor);
                    }
                    RightIsValid = true;
                }
            }
        }
        if (CurrentNode.LeftNeighbor != null)
        {
            if (!ClosedNodes.Contains(CurrentNode.LeftNeighbor))
            {
                if (!CurrentNode.LeftNeighbor.IsWalkable)
                {
                    ClosedNodes.Add(CurrentNode.LeftNeighbor);
                    LeftIsValid = false;
                }
                else
                {
                    int tentativeGCost = CurrentNode.G_Cost + Calculat_distance_cost(CurrentNode, CurrentNode.LeftNeighbor);
                    if (tentativeGCost < CurrentNode.LeftNeighbor.G_Cost)
                    {
                        CurrentNode.LeftNeighbor.CameFromNode = CurrentNode;
                        CurrentNode.LeftNeighbor.G_Cost = tentativeGCost;
                        CurrentNode.LeftNeighbor.H_Cost = Calculat_distance_cost(CurrentNode.LeftNeighbor, endNode);
                        CurrentNode.LeftNeighbor.CalculateFCost();

                        if (!OpenNodes.Contains(CurrentNode.LeftNeighbor)) OpenNodes.Add(CurrentNode.LeftNeighbor);
                    }
                    LeftIsValid = true;
                }
            }
        }
        if (CurrentNode.UpNeighbor != null)
        {
            if (!ClosedNodes.Contains(CurrentNode.UpNeighbor))
            {
                if (!CurrentNode.UpNeighbor.IsWalkable)
                {
                    ClosedNodes.Add(CurrentNode.UpNeighbor);
                    UpIsValid = false;
                }
                else
                {
                    int tentativeGCost = CurrentNode.G_Cost + Calculat_distance_cost(CurrentNode, CurrentNode.UpNeighbor);
                    if (tentativeGCost < CurrentNode.UpNeighbor.G_Cost)
                    {
                        CurrentNode.UpNeighbor.CameFromNode = CurrentNode;
                        CurrentNode.UpNeighbor.G_Cost = tentativeGCost;
                        CurrentNode.UpNeighbor.H_Cost = Calculat_distance_cost(CurrentNode.UpNeighbor, endNode);
                        CurrentNode.UpNeighbor.CalculateFCost();

                        if (!OpenNodes.Contains(CurrentNode.UpNeighbor)) OpenNodes.Add(CurrentNode.UpNeighbor);
                    }
                    UpIsValid = true;
                }
            }
        }
        if (CurrentNode.DownNeighbor != null)
        {
            if (!ClosedNodes.Contains(CurrentNode.DownNeighbor))
            {
                if (!CurrentNode.DownNeighbor.IsWalkable)
                {
                    ClosedNodes.Add(CurrentNode.DownNeighbor);

                    DownIsValid = false;
                }
                else
                {
                    int tentativeGCost = CurrentNode.G_Cost + Calculat_distance_cost(CurrentNode, CurrentNode.DownNeighbor);
                    if (tentativeGCost < CurrentNode.DownNeighbor.G_Cost)
                    {
                        CurrentNode.DownNeighbor.CameFromNode = CurrentNode;
                        CurrentNode.DownNeighbor.G_Cost = tentativeGCost;
                        CurrentNode.DownNeighbor.H_Cost = Calculat_distance_cost(CurrentNode.DownNeighbor, endNode);
                        CurrentNode.DownNeighbor.CalculateFCost();

                        if (!OpenNodes.Contains(CurrentNode.DownNeighbor)) OpenNodes.Add(CurrentNode.DownNeighbor);
                    }
                    DownIsValid = true;
                }
            }
        }


        if (UpIsValid && RightIsValid)
        {
            if (CurrentNode.UpRightNeighbor != null) CheckNeighbor(CurrentNode, CurrentNode.UpRightNeighbor, endNode);
        }
        if (UpIsValid && LeftIsValid)
        {
            if (CurrentNode.LeftUpNeighbor != null) CheckNeighbor(CurrentNode, CurrentNode.LeftUpNeighbor, endNode);
        }
        if (DownIsValid && RightIsValid)
        {
            if (CurrentNode.RightDownNeighbor != null) CheckNeighbor(CurrentNode, CurrentNode.RightDownNeighbor, endNode);
        }
        if (DownIsValid && LeftIsValid)
        {
            if (CurrentNode.LeftDownNeighbor != null) CheckNeighbor(CurrentNode, CurrentNode.LeftDownNeighbor, endNode);
        }


        
    }
}
