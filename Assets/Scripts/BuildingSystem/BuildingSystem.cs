using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public LayerMask MoseColliderLayerMask;
    public List<PlacableObject> ObjectsList;
    
    private PlacableObject SelectedObject;

    private PlacableObject.Direction Dir;

    public int GridWidth = 100;
    public int GridHeight = 100;

    float CellSize = 1f;

    private GridXZ<GridObject> grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GridXZ<GridObject>(GridWidth, GridHeight, CellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));

        foreach(PlacableObject obj in ObjectsList)
        {
            obj.SetGridRef(grid);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Administration.Game.GameIsStoped())
        {
            ChangeSelectedObjectWithInpus();
            IputsHandler();
        }
    }

    void ChangeSelectedObjectWithInpus()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectedObject = ObjectsList[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectedObject = ObjectsList[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectedObject = ObjectsList[2];
        }
    }
    void IputsHandler()
    {
        float MaxHoldTime = 0.5f, HoldTime = 0;
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 spawn_position = GetMouseWorldPosition();
            if (spawn_position.x <= grid.GetWidth() && spawn_position.z <= grid.GetHeight())
            {
                grid.GetXZ(spawn_position, out int x, out int z);
                Vector2Int offset = new Vector2Int(x, z);
                List<Vector2Int> grid_positions = SelectedObject.GetImaginaryBookedUpPlacesList(offset, Dir);

                bool canBuild = true;
                foreach (Vector2Int grid_p in grid_positions)
                {
                    if (!grid.GetGridObject(grid_p.x, grid_p.y).CanBuild) { canBuild = false; break; }
                }

                if (canBuild)
                {
                    Vector2Int RotationOffset = SelectedObject.GetRotationOffset(Dir);
                    Vector3 SpawnAtWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(RotationOffset.x, 0, RotationOffset.y) * grid.GetCellSize();

                    //need to change the way i instantiate the objects
                    SelectedObject.CreateCopy(SpawnAtWorldPosition, offset, Dir, out GameObject obj);
                    foreach (Vector2Int item in grid_positions)
                    {
                        grid.GetGridObject(item.x, item.y).SetHoldedObject(obj);

                    }
                }


            }
        }
        if (Input.GetMouseButton(1))
        {
            HoldTime += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(1))
        {
            HoldTime = 0;
        }
    }


    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, MoseColliderLayerMask))
        {
            return hitInfo.point;
        }
        else return Vector3.zero;
    }


}
