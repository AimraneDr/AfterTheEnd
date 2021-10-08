using System;
using System.Collections;
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
    private TGridObject[,] GridArray;

    public GridXZ(int width, int height, float cell_size, Vector3 originPosition, Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject)
    {
        Width = width;
        Height = height;    
        CellSize = cell_size;
        OriginPosition = originPosition;
        GridArray = new TGridObject[width, height];

        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int z = 0; z < GridArray.GetLength(1); z++)
            {
                GridArray[x, z] = createGridObject(this, x, z);
            }
        }

        debugIsVisible = false;
       
    }

    private void SwitchDebugVisibility()
    {
        if (debugIsVisible)
        {
            for (int x = 0; x < GridArray.GetLength(0); x++)
            {
                for (int z = 0; z < GridArray.GetLength(1); z++)
                {
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.red, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.red, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, Height), GetWorldPosition(Width, Height), Color.red, 100f);
            Debug.DrawLine(GetWorldPosition(Width, 0), GetWorldPosition(Width, Height), Color.red, 100f);
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
            //TriggerGridObjectChanged(x, z);
        }
    }

    //public void TriggerGridObjectChanged(int x, int z)
    //{
    //    OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
    //}

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

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        return new Vector2Int(
            Mathf.Clamp(gridPosition.x, 0, Width - 1),
            Mathf.Clamp(gridPosition.y, 0, Height - 1)
        );
    }

}
