using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Cam;


    public float NormalSpeed, FastSpeed;
    public float MovementTime;
    public float RotationAmmount;
    public Vector3 ZoomAmmount;
    public float MaxHeight, MinHeight;

    private float MovementSpped;
    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = Cam.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        MovementInputHandeler();
    }

    void MovementInputHandeler()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MovementSpped = FastSpeed;
        }
        else
        {
            MovementSpped = NormalSpeed;
        }


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * MovementSpped);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * MovementSpped);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -MovementSpped);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -MovementSpped);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * RotationAmmount);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -RotationAmmount);
        }

        if (Input.GetKey(KeyCode.R) )
        {
            float old_z_zoom = newZoom.z;
            newZoom += ZoomAmmount;
            newZoom.y = Mathf.Clamp(newZoom.y, MinHeight, MaxHeight);
            if(newZoom.y == MinHeight || newZoom.y == MaxHeight)
            {
                newZoom.z = old_z_zoom;
            }
        }

        if (Input.GetKey(KeyCode.F) )
        {
            float old_z_zoom = newZoom.z;
            newZoom -= ZoomAmmount;
            newZoom.y = Mathf.Clamp(newZoom.y, MinHeight, MaxHeight);
            if (newZoom.y == MinHeight || newZoom.y == MaxHeight)
            {
                newZoom.z = old_z_zoom;
            }
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, MovementTime * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, MovementTime * Time.deltaTime);
        Cam.localPosition = Vector3.Lerp(Cam.localPosition, newZoom, MovementTime * Time.deltaTime);
    }
}
