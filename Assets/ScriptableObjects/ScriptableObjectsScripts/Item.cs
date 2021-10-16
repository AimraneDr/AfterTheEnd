using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Item : ScriptableObject
{
    public int ID;
    public string Name;
    public Sprite Icon;
    public GameObject Graphics;
    public Type ItemType;
    public enum Type
    {
        PlacableOnly,
        PlacableAndPickable,
        PickableOnly,

    }
}
