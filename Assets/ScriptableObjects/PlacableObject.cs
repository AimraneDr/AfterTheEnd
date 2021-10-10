using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableObject : ScriptableObject
{
    public string Name;
    public Transform Prephab;
    public int Width, Length, Height;

    private Vector2Int Origin;
    private Direction Dir;
    private List<GridObject> BookedUpPlaces;
    private GridXZ<GridObject> GridReference;


    public enum Direction { Forward, right, Back, Left, }
    public void SetGridRef(GridXZ<GridObject> GridRef)
    {
        GridReference = GridRef;
    }

    public void Create(Vector3 SpawnAtWorldPosition, Vector2Int offset, Direction dir, out Transform FinalObj)
    {
        Origin = offset;
        Transform obj = Instantiate(
            Prephab,
            SpawnAtWorldPosition,
            Quaternion.Euler(0, GetRotationAngle(dir), 0)

            );
        //BuildableObject component = obj.gameObject.AddComponent<BuildableObject>() as BuildableObject;
        //component.SetProperties(Origin, dir, BookUpGridPlaces(Origin, dir));

        //BookUpPlaces(Origin, dir);

        FinalObj = obj;
    }
    public void DestroySelf()
    {
        //Debug.Log($"Check Holded Grid Objects , lenth of ({BookedUpPlaces.Count})");
        foreach (GridObject item in BookedUpPlaces)
        {
            item.ClearHoldedObject();
            //Debug.Log("Object had been destroyed");
        }
        Destroy(this);
    }

    //public List<Vector2Int> BookUpPlaces(Vector2Int offset, Direction dir)
    //{
    //    List<Vector2Int> positions = new List<Vector2Int>();
    //    switch (dir)
    //    {
    //        case Direction.Forward:
    //        case Direction.Back:
    //            for (int x = 0; x < Width; x++)
    //            {
    //                for (int z = 0; z < Length; z++)
    //                {
    //                    positions.Add(offset + new Vector2Int(x, z));
    //                }
    //            }
    //            break;
    //        case Direction.right:
    //        case Direction.Left:
    //            for (int x = 0; x < Length; x++)
    //            {
    //                for (int z = 0; z < Width; z++)
    //                {
    //                    positions.Add(offset + new Vector2Int(x, z));
    //                }
    //            }
    //            break;
    //    }

    //    return positions;
    //}
    public List<GridObject> BookUpGridPlaces(Vector2Int offset, Direction dir)
    {
        List<GridObject> positions = new List<GridObject>();
        switch (dir)
        {
            case Direction.Forward:
            case Direction.Back:
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < Length; z++)
                    {
                        Vector2Int GridPos = offset + new Vector2Int(x, z);
                        positions.Add(GridReference.GetGridObject(GridPos.x, GridPos.y));
                    }
                }
                break;
            case Direction.right:
            case Direction.Left:
                for (int x = 0; x < Length; x++)
                {
                    for (int z = 0; z < Width; z++)
                    {
                        Vector2Int GridPos = offset + new Vector2Int(x, z);
                        positions.Add(GridReference.GetGridObject(GridPos.x, GridPos.y));
                    }
                }
                break;
        }

        return positions;
    }
    public List<GridObject> GetBookedUpPlacesList()
    {
        return BookedUpPlaces;
    }

    public List<Vector2Int> GetImaginaryBookedUpPlacesList(Vector2Int offset, Direction dir)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        switch (dir)
        {
            case Direction.Forward:
            case Direction.Back:
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < Length; z++)
                    {
                        positions.Add(offset + new Vector2Int(x, z));
                    }
                }
                break;
            case Direction.right:
            case Direction.Left:
                for (int x = 0; x < Length; x++)
                {
                    for (int z = 0; z < Width; z++)
                    {
                        positions.Add(offset + new Vector2Int(x, z));
                    }
                }
                break;
        }

        return positions;
    }

    public Direction GetNextDirection(Direction current_direction)
    {
        switch (current_direction)
        {
            case Direction.Forward: return Direction.Left;
            case Direction.Left: return Direction.Back;
            case Direction.Back: return Direction.right;
            case Direction.right: return Direction.Forward;
        }
        return current_direction;
    }
    public int GetRotationAngle(Direction current_direction)
    {
        switch (current_direction)
        {
            case Direction.Forward: return 0;
            case Direction.Left: return 90;
            case Direction.Back: return 180;
            case Direction.right: return 270;
        }
        return 0;
    }
    public Vector2Int GetRotationOffset(Direction current_direction)
    {
        switch (current_direction)
        {
            case Direction.Forward: return new Vector2Int(0, 0);
            case Direction.Left: return new Vector2Int(0, Width);
            case Direction.Back: return new Vector2Int(Width, Height);
            case Direction.right: return new Vector2Int(Height, 0);
        }
        return new Vector2Int(0, 0);
    }

}
