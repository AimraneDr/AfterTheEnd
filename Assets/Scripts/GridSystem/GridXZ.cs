using System;
using System.Collections.Generic;
using UnityEngine;

public class GridXZ<TGridObject>
{
    private bool debugIsVisible;
    public bool DebugIsVisible
    {
        get => debugIsVisible;
        set
        {
            debugIsVisible = value;
            SwitchDebugVisibility();
        }
    }


    private int Width;
    private int Height;
    private float CellSize;
    private Vector3 OriginPosition;
    public TGridObject[,] GridArray;
    public List<TGridObject> BookedUpGrids;

    public delegate void GridXZEventsHandler(GridXZ<TGridObject> sender,TGridObject item = default(TGridObject));
    public event GridXZEventsHandler OnBookedUpGridsChange, OnBookedUpGridsAdd, OnBookedUpGridsRemove;
    public void RizeOnBookedUpGridsChange(GridXZ<TGridObject> grid)
    {
        /*Debug.Log("BookedUpGrids list changed");*/

        if (OnBookedUpGridsChange != null) OnBookedUpGridsChange(grid);
    }
    public void RizeOnBookedUpGridsAdd(GridXZ<TGridObject> grid,TGridObject AddedItem)
    {
        //Debug.Log("BookedUpGrids list changed type of Add");

        if (OnBookedUpGridsAdd != null) OnBookedUpGridsAdd(grid, AddedItem);
    }
    public void RizeOnBookedUpGridsRemove(GridXZ<TGridObject> grid,TGridObject RemovedItem)
    {
       //Debug.Log("BookedUpGrids list changed type of Remove");

        if (OnBookedUpGridsRemove != null) OnBookedUpGridsRemove(grid, RemovedItem);
    }


    public GridXZ(int width, int height, float cell_size, Vector3 originPosition, Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject)
    {
        Width = width;
        Height = height;
        CellSize = cell_size;
        OriginPosition = originPosition;
        GridArray = new TGridObject[width, height];
        BookedUpGrids = new List<TGridObject>();

        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int z = 0; z < GridArray.GetLength(1); z++)
            {
                GridArray[x, z] = createGridObject(this, x, z);
            }
        }

        debugIsVisible = false;

        foreach (TGridObject obj in GridArray)
        {
            try
            {
                PathNode node = obj as PathNode;
                BuildNode GObj = obj as BuildNode;
                if (node != null)
                {
                    node.SetNeighbors();
                    //Debug.Log(node.Neighbors.Count);
                }

                if (GObj != null)
                {
                    GObj.SetNeighbors();
                }
            }
            catch (Exception ex) { Debug.Log(ex.Message); }
        }
    }

    private void SwitchDebugVisibility()
    {
        if (debugIsVisible)
        {
            for (int x = 0; x < GridArray.GetLength(0); x++)
            {
                for (int z = 0; z < GridArray.GetLength(1); z++)
                {
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.blue, 1000f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.blue, 1000f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, Height), GetWorldPosition(Width, Height), Color.blue, 1000f);
            Debug.DrawLine(GetWorldPosition(Width, 0), GetWorldPosition(Width, Height), Color.blue, 1000f);
        }
    }

    public int GetWidth()
    {
        return Width;
    }

    public int GetHeight()
    {
        return Height;
    }

    public float GetCellSize()
    {
        return CellSize;
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * CellSize + OriginPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - OriginPosition).x / CellSize);
        z = Mathf.FloorToInt((worldPosition - OriginPosition).z / CellSize);
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        if (x >= 0 && z >= 0 && x < Width && z < Height)
        {
            GridArray[x, z] = value;

        }
    }


    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < Width && z < Height)
        {
            return GridArray[x, z];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    public List<TGridObject> GetNeighbors(int x, int z)
    {
        List<TGridObject> neighbors = new List<TGridObject>();

        if (x - 1 >= 0)
        {
            //Left
            neighbors.Add(GetGridObject(x - 1, z));
            //LeftDown
            if (z - 1 >= 0) neighbors.Add(GetGridObject(x - 1, z - 1));
            //LeftUp
            if (z + 1 < Height) neighbors.Add(GetGridObject(x - 1, z + 1));
        }
        if (x + 1 < Width)
        {
            //right
            neighbors.Add(GetGridObject(x + 1, z));
            //right down
            if (z - 1 >= 0) neighbors.Add(GetGridObject(x + 1, z - 1));
            //right up
            if (z + 1 < Height) neighbors.Add(GetGridObject(x + 1, z + 1));
        }
        if (z + 1 < Height) neighbors.Add(GetGridObject(x, z + 1));
        if (z - 1 >= 0) neighbors.Add(GetGridObject(x, z - 1));

        return neighbors;
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        return new Vector2Int(
            Mathf.Clamp(gridPosition.x, 0, Width - 1),
            Mathf.Clamp(gridPosition.y, 0, Height - 1)
        );
    }

}
