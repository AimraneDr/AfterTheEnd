using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class simpleMenuUIController : MonoBehaviour
{

    public GameObject UI_Handler;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ListenToInputs();
    }

    void ListenToInputs()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UI_Handler.SetActive(!UI_Handler.activeSelf);
            if(UI_Handler.activeSelf) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
    }

}
