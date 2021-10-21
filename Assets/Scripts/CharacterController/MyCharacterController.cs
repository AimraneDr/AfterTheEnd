using System.Collections;
using UnityEngine;


public class MyCharacterController : MonoBehaviour
{
    GridPathFinder path_finder;
    public bool ShowPathGizmos,UseSimplifiedPath;
    public float Speed = 5f;
    int currentWayPointIndex = 0;
    Vector3[] path,SimplifiedPath;
    Vector3 TargetPosition;


    private void Awake()
    {
        path_finder = new GridPathFinder(100, 100);
        path_finder.BuildGridLevel = BuildingSystem.Instance.grid;
        path_finder.PassEvents();
        path_finder.SetBookedUpNodes();
    }

    private void Update()
    {
        DrawPathGizmos();
        MovementHandler();

    }
    private void MovementHandler()
    {
        if (UseSimplifiedPath)
        {
            if (SimplifiedPath != null)
            {

                Vector3 target = SimplifiedPath[currentWayPointIndex];
                if (Vector3.Distance(transform.position, target) > .5f)
                {
                    Vector3 moveDir = (target - transform.position).normalized;
                    transform.position = Vector3.Lerp(transform.position, transform.position + moveDir, Speed * Time.deltaTime);
                }
                else
                {
                    currentWayPointIndex++;
                    if (currentWayPointIndex >= SimplifiedPath.Length)
                    {
                        StopMoving();
                    }
                }
            }
        }
        else
        {
            if (path != null)
            {

                Vector3 target = path[currentWayPointIndex];
                if (Vector3.Distance(transform.position, target) > 1f)
                {
                    Vector3 moveDir = (target - transform.position).normalized;
                    transform.position = Vector3.Lerp(transform.position, transform.position + moveDir, Speed * Time.deltaTime);
                }
                else
                {
                    currentWayPointIndex++;
                    if (currentWayPointIndex >= path.Length)
                    {
                        StopMoving();
                    }
                }
            }
        }
            
    }
    void FindPath()
    {
        path = path_finder.FindPath(transform.position, TargetPosition)?.ToArray();
        SimplifiedPath = path_finder.SimplifyPath(path);
         
    }
    void DrawPathGizmos()
    {
        if (path != null && ShowPathGizmos)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.red, 100f);
            }
        }

        if (UseSimplifiedPath)
        {
            if (SimplifiedPath != null && ShowPathGizmos)
            {
                for (int i = 0; i < SimplifiedPath.Length; i++)
                {
                    GameObject ob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    ob.transform.parent = GameObject.Find("DegugObj").transform;
                    ob.transform.localScale = Vector3.one * .5f;
                    ob.GetComponent<Renderer>().material.color = Color.blue;
                    Instantiate(
                        ob,
                        SimplifiedPath[i],
                        Quaternion.identity
                        );
                }
            }

        }
        else
        {
            if (path != null && ShowPathGizmos)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    GameObject ob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    ob.transform.parent = GameObject.Find("DegugObj").transform;
                    ob.transform.localScale = Vector3.one * .5f;
                    ob.GetComponent<Renderer>().material.color = Color.blue;
                    Instantiate(
                        ob,
                        path[i],
                        Quaternion.identity
                        );
                }
            }
        }
    }

    public void SetTarget(Vector3 target)
    {
        TargetPosition = target;

        currentWayPointIndex = 0;
        StopMoving();
        FindPath();
    }
    public void StopMoving()
    {
        path = null;
        SimplifiedPath = null;
    }
}
