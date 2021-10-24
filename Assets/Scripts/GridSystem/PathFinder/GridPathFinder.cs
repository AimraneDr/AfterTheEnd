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
    /// <summary>
    /// find the clean and the fastiest path to reach the destination
    /// </summary>
    /// <returns></returns>
    public List<PathNode> FindCleanPath(int start_x, int start_z, int target_x, int target_z)
    {
        //UnityEngine.Debug.Log("Start fint Path nodes");
        sw = new Stopwatch();
        sw.Start();
        PathNode startNode = grid.GetGridObject(start_x, start_z);
        PathNode endNode = grid.GetGridObject(target_x, target_z);
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

        }
        return null;
    }

    /// <summary>
    /// find the fastiest path to reach the destination without taking barriers into count
    /// </summary>
    /// <returns></returns>
    public List<PathNode> FindThroughBarriersPath(int start_x, int start_z, int target_x, int target_z)
    {
        //UnityEngine.Debug.Log("start finding Through Barriers Path");
        sw = new Stopwatch();
        sw.Start();
        PathNode startNode = grid.GetGridObject(start_x, start_z);
        PathNode endNode = grid.GetGridObject(target_x, target_z);
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

            CheckNeighbors(CurrentNode, endNode ,true);

        }
        return null;
    }

    public List<Vector3> FindPath(Vector3 StartWorldPosition,Vector3 TargetWorldPosition)
    {
        //UnityEngine.Debug.Log("Start fint Path positions");
        var sw = new Stopwatch();
        sw.Start();
        grid.GetXZ(StartWorldPosition, out int s_x, out int s_y);
        grid.GetXZ(TargetWorldPosition, out int x, out int y);
        List<PathNode> path = FindCleanPath(s_x, s_y, x, y);
        if (path == null)
        {
            path = FindThroughBarriersPath(s_x, s_y, x, y);

            if (path == null) { 
                sw.Stop();
                UnityEngine.Debug.Log($"path fount after : {sw.ElapsedMilliseconds} ms");
                return null; 
            }
            else 
            {
                var _ = new List<Vector3>(PathToWorldPostions(path.ToArray()));
                sw.Stop();
                UnityEngine.Debug.Log($"path fount after : {sw.ElapsedMilliseconds} ms");
                return _;
            }
        }
        else
        {
            var _ = new List<Vector3>(PathToWorldPostions(path.ToArray()));
            sw.Stop();
            UnityEngine.Debug.Log($"path fount after : {sw.ElapsedMilliseconds} ms");
            return _;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">an array of waypoints</param>
    /// <param name="keep_waypoints_of_index">represents wich waypoint of path[] needs to be keeped </param>
    /// <returns>simplified virsion of the input path empty of inligned waypoints</returns>
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

    private void CheckNeighbor(PathNode currentNode, PathNode neighbor, PathNode endNode,bool ConsiderBarriers = false)
    {
        if (!ClosedNodes.Contains(neighbor))
        {
            if (!neighbor.IsWalkable)
            {
                ClosedNodes.Add(neighbor);

                if (!ConsiderBarriers)
                {
                    ClosedNodes.Add(neighbor);
                }
                else
                {
                    Item.Spicialities HoldedObjectSpiciality = Item.Spicialities.None;
                    if (BuildGridLevel.GetGridObject(neighbor.x, neighbor.z).GetHoldedObject() != null)
                    {
                        HoldedObjectSpiciality = BuildGridLevel.GetGridObject(neighbor.x, neighbor.z).GetHoldedObject().GetComponent<ObjectInfo>().Spisiality;
                    }
                    UnityEngine.Debug.Log($"{HoldedObjectSpiciality}");
                    if (HoldedObjectSpiciality == Item.Spicialities.Deffance)
                    {
                        UnityEngine.Debug.Log("Grid Holds a deffance Object");
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
                    else
                    {
                        ClosedNodes.Add(neighbor);
                    }
                }
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
    private void CheckNeighbors(PathNode CurrentNode,PathNode endNode,bool ConsiderBarriers = false)
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
                    if (!ConsiderBarriers)
                    {
                        ClosedNodes.Add(CurrentNode.RightNeighbor);
                        RightIsValid = false;
                    }
                    else
                    {
                        Item.Spicialities HoldedObjectSpiciality = Item.Spicialities.None;
                        if(BuildGridLevel.GetGridObject(CurrentNode.RightNeighbor.x, CurrentNode.RightNeighbor.z).GetHoldedObject() != null)
                        {
                            HoldedObjectSpiciality = BuildGridLevel.GetGridObject(CurrentNode.RightNeighbor.x, CurrentNode.RightNeighbor.z).GetHoldedObject().GetComponent<ObjectInfo>().Spisiality;
                        }
                        UnityEngine.Debug.Log($"{HoldedObjectSpiciality}");
                        if (HoldedObjectSpiciality == Item.Spicialities.Deffance)
                        {
                            UnityEngine.Debug.Log("Grid Holds a deffance Object");
                            Check(CurrentNode.RightNeighbor);
                            RightIsValid = false;
                        }
                        else
                        {
                            ClosedNodes.Add(CurrentNode.RightNeighbor);
                            RightIsValid = false;
                        }
                    }
                   
                }
                else
                {
                    Check(CurrentNode.RightNeighbor);
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
                    if (!ConsiderBarriers)
                    {
                        ClosedNodes.Add(CurrentNode.LeftNeighbor);
                        LeftIsValid = false;
                    }
                    else
                    {
                        Item.Spicialities HoldedObjectSpiciality = Item.Spicialities.None;
                        if (BuildGridLevel.GetGridObject(CurrentNode.RightNeighbor.x, CurrentNode.RightNeighbor.z).GetHoldedObject() != null)
                        {
                            HoldedObjectSpiciality = BuildGridLevel.GetGridObject(CurrentNode.RightNeighbor.x, CurrentNode.RightNeighbor.z).GetHoldedObject().GetComponent<ObjectInfo>().Spisiality;
                        }
                        UnityEngine.Debug.Log($"{HoldedObjectSpiciality}");
                        if (HoldedObjectSpiciality == Item.Spicialities.Deffance)
                        {
                            UnityEngine.Debug.Log("Grid Holds a deffance Object");
                            Check(CurrentNode.LeftNeighbor);
                            LeftIsValid = false;
                        }
                        else
                        {
                            ClosedNodes.Add(CurrentNode.LeftNeighbor);
                            LeftIsValid = false;
                        }
                    }
                }
                else
                {
                    Check(CurrentNode.LeftNeighbor);
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
                    if (!ConsiderBarriers)
                    {
                        ClosedNodes.Add(CurrentNode.UpNeighbor);
                        UpIsValid = false;
                    }
                    else
                    {
                        Item.Spicialities HoldedObjectSpiciality = Item.Spicialities.None;
                        if (BuildGridLevel.GetGridObject(CurrentNode.RightNeighbor.x, CurrentNode.RightNeighbor.z).GetHoldedObject() != null)
                        {
                            HoldedObjectSpiciality = BuildGridLevel.GetGridObject(CurrentNode.RightNeighbor.x, CurrentNode.RightNeighbor.z).GetHoldedObject().GetComponent<ObjectInfo>().Spisiality;
                        }
                        UnityEngine.Debug.Log($"{HoldedObjectSpiciality}");
                        if (HoldedObjectSpiciality == Item.Spicialities.Deffance)
                        {
                            UnityEngine.Debug.Log("Grid Holds a deffance Object");
                            Check(CurrentNode.UpNeighbor);
                            UpIsValid = false;
                        }
                        else
                        {
                            ClosedNodes.Add(CurrentNode.UpNeighbor);
                            UpIsValid = false;
                        }
                    }
                }
                else
                {
                    Check(CurrentNode.UpNeighbor);
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
                    if (!ConsiderBarriers)
                    {
                        ClosedNodes.Add(CurrentNode.DownNeighbor);
                        DownIsValid = false;
                    }
                    else
                    {
                        Item.Spicialities HoldedObjectSpiciality = Item.Spicialities.None;
                        if (BuildGridLevel.GetGridObject(CurrentNode.RightNeighbor.x, CurrentNode.RightNeighbor.z).GetHoldedObject() != null)
                        {
                            HoldedObjectSpiciality = BuildGridLevel.GetGridObject(CurrentNode.RightNeighbor.x, CurrentNode.RightNeighbor.z).GetHoldedObject().GetComponent<ObjectInfo>().Spisiality;
                        }
                        UnityEngine.Debug.Log($"{HoldedObjectSpiciality}");
                        if (HoldedObjectSpiciality == Item.Spicialities.Deffance)
                        {
                            UnityEngine.Debug.Log("Grid Holds a deffance Object");
                            Check(CurrentNode.DownNeighbor);
                            DownIsValid = false;
                        }
                        else
                        {
                            ClosedNodes.Add(CurrentNode.DownNeighbor);
                            DownIsValid = false;
                        }
                    }
                }
                else
                {
                    Check(CurrentNode.DownNeighbor);
                    DownIsValid = true;
                }
            }
        }


        if (UpIsValid && RightIsValid)
        {
            if (CurrentNode.UpRightNeighbor != null) CheckNeighbor(CurrentNode, CurrentNode.UpRightNeighbor, endNode, ConsiderBarriers);
        }
        if (UpIsValid && LeftIsValid)
        {
            if (CurrentNode.LeftUpNeighbor != null) CheckNeighbor(CurrentNode, CurrentNode.LeftUpNeighbor, endNode, ConsiderBarriers);
        }
        if (DownIsValid && RightIsValid)
        {
            if (CurrentNode.RightDownNeighbor != null) CheckNeighbor(CurrentNode, CurrentNode.RightDownNeighbor, endNode, ConsiderBarriers);
        }
        if (DownIsValid && LeftIsValid)
        {
            if (CurrentNode.LeftDownNeighbor != null) CheckNeighbor(CurrentNode, CurrentNode.LeftDownNeighbor, endNode, ConsiderBarriers);
        }

        void Check(PathNode checkNode)
        {
            int tentativeGCost = CurrentNode.G_Cost + Calculat_distance_cost(CurrentNode, checkNode);
            if (tentativeGCost < checkNode.G_Cost)
            {
                checkNode.CameFromNode = CurrentNode;
                checkNode.G_Cost = tentativeGCost;
                checkNode.H_Cost = Calculat_distance_cost(checkNode, endNode);
                checkNode.CalculateFCost();

                if (!OpenNodes.Contains(checkNode)) OpenNodes.Add(checkNode);
            }
        }
        
    }
}
