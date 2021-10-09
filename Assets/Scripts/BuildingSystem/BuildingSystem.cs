using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{

    private int _GridWidth;
    public int GridWidth 
    {
        get => _GridWidth;
        set
        {
            _GridWidth = value;
        }
    }
    private int _GridHeight;
    public int GridHeight
    {
        get => _GridHeight;
        set
        {
            _GridHeight = value;
            
        }
    }

    float CellSize = 1f;

    private GridXZ<GridObject> grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GridXZ<GridObject>(GridWidth, GridHeight, CellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
