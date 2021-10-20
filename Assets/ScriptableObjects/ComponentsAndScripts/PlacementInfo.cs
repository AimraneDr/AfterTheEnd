using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementInfo : MonoBehaviour
{
    private Vector2Int Origin;
    private PlacableObject.Direction Dir;
    private List<BuildNode> BookedUpPlaces;

    public void SetProperties(Vector2Int origin, PlacableObject.Direction dir, List<BuildNode> bookedUpPlaces)
    {
        Origin = origin;
        Dir = dir;
        BookedUpPlaces = bookedUpPlaces;

    }

    public void DestroySelf()
    {
        foreach (BuildNode item in BookedUpPlaces)
        {
            item.ClearHoldedObject();
        }
        Destroy(this.gameObject);
    }
}
