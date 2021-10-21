using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesigner
{
    public PlacableObject  DefaultObject;
    public GridXZ<BuildNode> grid;
    PlacableObject.Direction Dir = PlacableObject.Direction.Forward;


    public LevelDesigner(GridXZ<BuildNode> _grid, PlacableObject _DefaultObject)
    {
        grid = _grid;
        DefaultObject = _DefaultObject;

        bool switcher = true;
        for (int x = 1; x < grid.GetWidth(); x += 2)
        {

            if (switcher)
            {
                for (int z = 0; z < grid.GetHeight() - 1; z++)
                {

                    Vector2Int offset = new Vector2Int(x, z);
                    List<Vector2Int> grid_positions = DefaultObject.GetImaginaryBookedUpPlacesList(offset, Dir);
                    Vector2Int RotationOffset = DefaultObject.GetRotationOffset(PlacableObject.Direction.Forward);
                    Vector3 SpawnAtWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(RotationOffset.x, 0, RotationOffset.y) * grid.GetCellSize();
                    DefaultObject.CreateCopy(SpawnAtWorldPosition, offset, PlacableObject.Direction.Forward, out GameObject obj);
                    foreach (Vector2Int item in grid_positions)
                    {
                        grid.GetGridObject(item.x, item.y).SetHoldedObject(obj);
                    }
                }
                switcher = false;
            }
            else
            {
                for (int z = 1; z < grid.GetHeight(); z++)
                {
                    Vector2Int offset = new Vector2Int(x, z);
                    List<Vector2Int> grid_positions = DefaultObject.GetImaginaryBookedUpPlacesList(offset, Dir);
                    Vector2Int RotationOffset = DefaultObject.GetRotationOffset(PlacableObject.Direction.Forward);
                    Vector3 SpawnAtWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(RotationOffset.x, 0, RotationOffset.y) * grid.GetCellSize();
                    DefaultObject.CreateCopy(SpawnAtWorldPosition, offset, PlacableObject.Direction.Forward, out GameObject obj);
                    foreach (Vector2Int item in grid_positions)
                    {
                        grid.GetGridObject(item.x, item.y).SetHoldedObject(obj);
                    }
                }
                switcher = true;
            }
        }

    }

}
