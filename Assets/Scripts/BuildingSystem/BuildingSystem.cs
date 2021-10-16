using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public LayerMask MoseColliderLayerMask;
    public List<PlacableObject> ObjectsList;
    
    public PlacableObject SelectedObject;

    private PlacableObject.Direction Dir;

    public int GridWidth = 100;
    public int GridHeight = 100;

    float CellSize = 1f;

    private GridXZ<GridObject> grid;

    public Accounter AccounterObj;

    public Material CanBuildMaterial, CanNotBuildMaterial;
    bool temporraryObjIsCreated = false;
    GameObject temporraryObj = null;
    int temporraryId = 0;

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
            TemporraryVisual();
            if (SelectedObject != null)
            {
                IputsHandler();
            }
                
        }
    }

    void TemporraryVisual()
    {
        if (SelectedObject != null)
        {
            Vector3 temporrary_position = GetMouseWorldPosition();
            if (temporrary_position.x <= grid.GetWidth() && temporrary_position.z <= grid.GetHeight())
            {
                grid.GetXZ(temporrary_position, out int x, out int z);
                Vector2Int offset = new Vector2Int(x, z);
                List<Vector2Int> grid_positions = SelectedObject.GetImaginaryBookedUpPlacesList(offset, Dir);

                bool canBuild = true;
                foreach (Vector2Int grid_p in grid_positions)
                {
                    if (!grid.GetGridObject(grid_p.x, grid_p.y).CanBuild) { canBuild = false; break; }
                }

                Vector2Int RotationOffset = SelectedObject.GetRotationOffset(Dir);
                Vector3 SpawnAtWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(RotationOffset.x, 0.5f, RotationOffset.y) * grid.GetCellSize();
                if (!temporraryObjIsCreated || temporraryId != SelectedObject.ID) 
                {
                    temporraryId = SelectedObject.ID;
                    Destroy(temporraryObj);
                    SelectedObject.CreateCopy(SpawnAtWorldPosition, offset, Dir, out temporraryObj);
                    temporraryObjIsCreated = true;
                }
                else
                {
                    temporraryObj.transform.position = SpawnAtWorldPosition;
                }

                if (canBuild)
                {
                    temporraryObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = CanBuildMaterial;
                }
                else
                {
                    temporraryObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = CanNotBuildMaterial;
                }
            }
        }
        else
        {
            temporraryObj = null;
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

                    if (AccounterObj.GetCurrentMonnyAmmount() >= SelectedObject.Cost)
                    {
                        SelectedObject.CreateCopy(SpawnAtWorldPosition, offset, Dir, out GameObject obj);
                        foreach (Vector2Int item in grid_positions)
                        {
                            grid.GetGridObject(item.x, item.y).SetHoldedObject(obj);
                        }
                        AccounterObj.PayBill(SelectedObject.Cost);
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
