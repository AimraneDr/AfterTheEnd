using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    MyCharacterController characterController;
    public LayerMask GraounLayerMask;


    // Start is called before the first frame update
    void Start()
    {

        characterController = GetComponent<MyCharacterController>();
        Transform target = GameObject.Find("citizen").transform;
        characterController.SetTarget(target, 1);
    }

    // Update is called once per frame
    void Update()
    {

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
