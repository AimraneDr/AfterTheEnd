using System.Collections;
using UnityEngine;


public class MyCharacterController : MonoBehaviour
{
    GridPathFinder path_finder;
    public bool ShowPathGizmos,UseSimplifiedPath;
    public float Speed = 5f;
    public bool ChaseTarget = false;
    int currentWayPointIndex = 0;
    Transform Target;
    Vector3[] path,SimplifiedPath;
    Vector3 TargetPosition, LastTargetPosition;
    

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
        if (ChaseTarget)
        {
            if (Target != null) 
                if (LastTargetPosition != Target.position) 
                    FindPath(1);
        }
        Move();
        if (Target != null) LastTargetPosition = Target.position;
        
    }
    private void Move()
    {
        if (UseSimplifiedPath)
        {
            if (SimplifiedPath != null)
            {

                Vector3 target = SimplifiedPath[currentWayPointIndex];
                if (Vector3.Distance(transform.position, target) > 1f)
                {
                    Vector3 moveDir = (target - transform.position).normalized;
                    //transform.position = transform.position + moveDir * Speed * Time.deltaTime;
                    transform.position = Vector3.Lerp(transform.position, transform.position + moveDir, Speed * Time.deltaTime);
                }
                else
                {
                    currentWayPointIndex++;
                    if (currentWayPointIndex >= SimplifiedPath.Length)
                    {
                        transform.position = Vector3.Lerp(transform.position, target, Speed * Time.deltaTime);
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
                    //transform.position = transform.position + moveDir * Speed * Time.deltaTime;
                    transform.position = Vector3.Lerp(transform.position, target, Speed * Time.deltaTime);
                }
                else
                {
                    currentWayPointIndex++;
                    if (currentWayPointIndex >= path.Length)
                    {
                        transform.position = Vector3.Lerp(transform.position, target, Speed * Time.deltaTime);
                        StopMoving();
                    }
                }
            }
        }
            
    }
    void FindPath(int StopBefore = 0)
    {
        path = path_finder.FindPath(transform.position, TargetPosition)?.ToArray();
        path = RemoveLastWayPoints(path, StopBefore);
        SimplifiedPath = path_finder.SimplifyPath(path);
    }

    void FindPath(Transform target , int StopBefore = 0)
    {
        path = path_finder.FindPath(transform.position, target.position)?.ToArray();
        path = RemoveLastWayPoints(path, StopBefore);
        SimplifiedPath = path_finder.SimplifyPath(path);


    }
    Vector3[] RemoveLastWayPoints(Vector3[] waypointsArray,int waypointsNumber)
    {
        Vector3[] newPath = new Vector3[waypointsArray.Length - waypointsNumber];
        for(int i = 0; i < newPath.Length; i++)
        {
            newPath[i] = waypointsArray[i];
        }
        return newPath;
    }
    
    void DrawPathGizmos()
    {
        if (path != null && ShowPathGizmos)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.red, 10f);
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
                ShowPathGizmos = false;

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
                ShowPathGizmos = false;

            }
        }
    }

    public void MoveToward(Vector3 target, int StopBefore = 0)
    {
        TargetPosition = target;

        currentWayPointIndex = 0;
        StopMoving();
        FindPath(StopBefore);
        //Move();
    }

    public void SetTarget(Transform target, int StopBefore = 0)
    {
        Target = target;
        TargetPosition = target.position;

        currentWayPointIndex = 0;
        StopMoving();
        FindPath(Target, StopBefore);
        //Move();
    }
    public void StopMoving()
    {
        path = null;
        SimplifiedPath = null;
    }
}
