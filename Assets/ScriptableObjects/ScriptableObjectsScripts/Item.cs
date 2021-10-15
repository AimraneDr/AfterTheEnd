using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Item : ScriptableObject
{
    public string Name;
    public Image image;
    public GameObject Graphics;
    public Type ItemType;
    public enum Type
    {
        PlacableOnly,
        PlacableAndPickable,
        PickableOnly,

    }
}
