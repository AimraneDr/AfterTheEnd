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
    private float old_z_zoom;
    private Vector3 DragStartPosition, DragCurrentPosition;
    private Vector3 RotationStartPostion, RotationCurrenetPostion;

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
        old_z_zoom = newZoom.z;
        if (!Administration.Game.GameIsStoped())
        {
            MouseInputsHaandler();
            KeyboardInputsHandeler();
        }
    }

    void KeyboardInputsHandeler()
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
            newZoom += ZoomAmmount;
        }

        if (Input.GetKey(KeyCode.F) )
        {
            newZoom -= ZoomAmmount;
        }
        newZoom.y = Mathf.Clamp(newZoom.y, MinHeight, MaxHeight);
        if (newZoom.y == MinHeight || newZoom.y == MaxHeight)
        {
            newZoom.z = old_z_zoom;
        }
        transform.position = Vector3.Lerp(transform.position, newPosition, MovementTime * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, MovementTime * Time.deltaTime);
        Cam.localPosition = Vector3.Lerp(Cam.localPosition, newZoom, MovementTime * Time.deltaTime);
    }

    void MouseInputsHaandler()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * ZoomAmmount;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Plane plan = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if(plan.Raycast(ray, out entry))
            {
                DragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(0))
        {
            Plane plan = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plan.Raycast(ray, out entry))
            {
                DragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + DragStartPosition - DragCurrentPosition;
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            RotationCurrenetPostion = Input.mousePosition;
            Vector3 difference = RotationStartPostion - RotationCurrenetPostion;
            RotationStartPostion = RotationCurrenetPostion;
            newRotation *= Quaternion.Euler(Vector3.up * (difference.x / 10f));
        }
    }
}
