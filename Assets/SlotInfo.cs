using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotInfo : MonoBehaviour
{
    public PlacableObject Ref;
    public TextMeshProUGUI NameMesh;
    public Image image;

    public delegate void SlotEventsHandler(SlotInfo sender);

    public event SlotEventsHandler OnSlotClick;

    public void Slot_Clicked()
    {
        OnSlotClick(this);
       
    }
}
