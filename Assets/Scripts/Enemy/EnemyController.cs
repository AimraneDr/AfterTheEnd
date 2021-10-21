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
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 targetPos = GetMouseWorldPosition();
            characterController.SetTarget(targetPos);
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
