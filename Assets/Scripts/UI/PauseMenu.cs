using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public GameObject UI_Handler,SlotsHolder;
    



    // Start is called before the first frame update
    void Start()
    {
        RenderInventory();
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
            if(UI_Handler.activeSelf) Administration.Game.Stop();
            else Administration.Game.Continue();
        }
    }


    public void RenderInventory()
    {
        ItemsDataBase DB = GameObject.Find("DataBase").GetComponent<ItemsDataBase>();
        PlacableObject[] list = DB.PlacableObjects;
        for (int i = 0; i < list.Length; i++)
        {
            GameObject obj = GameObject.Instantiate(
                DB?.InvetorySlot,
                SlotsHolder.transform,
                false
                );
            SlotInfo info = obj.GetComponent<SlotInfo>();
            info.NameMesh.text = list[i]?.Name;
            info.image = list[i]?.image;
        }
    }

}
