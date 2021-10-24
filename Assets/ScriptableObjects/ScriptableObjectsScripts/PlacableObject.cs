using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmptyTemplate", menuName = "Buildable Objects/EmptyTemplate")]
public class PlacableObject : Item ,IDamageable
{
    public int Health;
    public int Width, Length, Height;


    public int Cost;

    private Vector2Int Origin;
    private GridXZ<BuildNode> GridReference;


    public enum Direction { Forward, right, Back, Left, }

    public PlacableObject()
    {
        ItemType = Type.PickableOnly;
    }
    public void SetGridRef(GridXZ<BuildNode> GridRef)
    {
        GridReference = GridRef;
    }

    public void CreateCopy(Vector3 SpawnAtWorldPosition, Vector2Int offset, Direction dir, out GameObject FinalObj)
    {
        Origin = offset;
        GameObject obj = Instantiate(
            Graphics,
            SpawnAtWorldPosition,
            Quaternion.Euler(0, GetRotationAngle(dir), 0)

            );
        //obj.layer = Layer;
        ObjectInfo component = obj.gameObject.AddComponent<ObjectInfo>() as ObjectInfo;
        component.SetProperties(Origin, dir, BookUpGridPlaces(Origin, dir));
        component.Spisiality = Spisiality;
        FinalObj = obj;
    }

    public List<BuildNode> BookUpGridPlaces(Vector2Int offset, Direction dir)
    {
        List<BuildNode> positions = new List<BuildNode>();
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

    public void Damage(int damage)
    {
        Health -= damage;
    }

}
