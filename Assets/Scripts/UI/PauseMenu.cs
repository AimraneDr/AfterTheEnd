using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public GameObject BuildingMenu,SlotsHolder;
    



    // Start is called before the first frame update
    void Start()
    {
        RenderInventory();
        BuildingMenu.SetActive(false);
        Administration.Game.Continue();
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
            BuildingMenu.SetActive(!BuildingMenu.activeSelf);
            if(BuildingMenu.activeSelf) Administration.Game.Stop();
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
                DB?.InventorySlot,
                SlotsHolder.transform,
                false
                );
            SlotInfo info = obj.GetComponent<SlotInfo>();
            info.Ref = list[i];
            info.NameMesh.text = list[i]?.Name;
            info.image.sprite = list[i]?.Icon;
            info.OnSlotClick += (sender) =>
              {
                  GameObject.Find("BuildingSystemObject").GetComponent<BuildingSystem>().SelectedObject = info.Ref;
                  BuildingMenu.SetActive(false);
                  Administration.Game.Continue();
              };
        }
    }

}
