using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridXZ<GridObject> grid;
    private int x, z;
    private Transform HoldedObject;
    public bool CanBuild { get { return HoldedObject == null; } }

    public GridObject(GridXZ<GridObject> _grid, int _x, int _z)
    {
        grid = _grid;
        x = _x;
        z = _z;
    }

    public void SetHoldedObject(Transform obj)
    {
        HoldedObject= obj;
    }
    public Transform GetHoldedObject()
    {
        return HoldedObject;
    }

    public void ClearHoldedObject()
    {
        HoldedObject = null;
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
