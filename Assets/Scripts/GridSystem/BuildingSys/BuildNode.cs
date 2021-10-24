using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildNode : GridObject<BuildNode>
{
    int FilledAmmount = 0;

    private GameObject _HoldedObject;
    private GameObject HoldedObject
    {
        get => _HoldedObject;
        set
        {
            _HoldedObject = value;
            if (value == null)
            {
                State = GridState.Free;
                if (grid.BookedUpGrids.Contains(this))
                {
                    grid.BookedUpGrids.Remove(this);
                    grid.RizeOnBookedUpGridsRemove(grid, this);
                    grid.RizeOnBookedUpGridsChange(grid);
                }
            }
            else
            {
                State = GridState.BookedUp;
                if (!grid.BookedUpGrids.Contains(this))
                {
                    grid.BookedUpGrids.Add(this);
                    grid.RizeOnBookedUpGridsAdd(grid, this);
                    grid.RizeOnBookedUpGridsChange(grid);
                }
            }
        }
    }

    public BuildNode(GridXZ<BuildNode> _grid, int _x, int _z)
    {
        grid = _grid;
        x = _x;
        z = _z;
    }

    public void SetHoldedObject(GameObject obj)
    {
        HoldedObject= obj;
    }
    public GameObject GetHoldedObject()
    {
        return HoldedObject;
    }

    public void ClearHoldedObject()
    {
        HoldedObject = null;
    }
   

    
}
