using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GridPathFinder path_finder;
    public LayerMask GraounLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        path_finder = new GridPathFinder(100, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Button Down");
            Vector3 pos = GetMouseWorldPosition();

            path_finder.GetGrid().GetXZ(transform.position, out int s_x, out int s_y);
            path_finder.GetGrid().GetXZ(pos, out int x, out int y);
            
            List<PathNode> path = path_finder.FindPath(s_x, s_y, x, y);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.Log($"{path[i].x} , {path[i].y} to {path[i + 1].x} , {path[i + 1].y}");
                    Debug.DrawLine(new Vector3(path[i].x, 0.5f, path[i].y) * 10f + Vector3.one * 0.5f, new Vector3(path[i + 1].x, 0.5f, path[i + 1].y) * 10f + Vector3.one * 0.5f,Color.red);
                }
            }
        }
    }



    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 2000f, GraounLayerMask))
        {
            return hitInfo.point;
        }
        else return Vector3.zero;
    }
}
