using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Route : MonoBehaviour
{
    // the waypoint in the route
    public List<Vector3> Locations { get; private set; }

    // the start of the route
    public Vector3 StartLocation
    {
        get
        {
            if (Locations != null && Locations.Count > 0)
            {
                return Locations[0];
            }
            return Vector3.zero;
        }
    }

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        // populating the route location according to the child objects
        if (Locations != null)
        {
            Locations.Clear();
        }
        else
        {
            Locations = new List<Vector3>();
        }

        foreach (Transform child in transform)
        {
            Locations.Add(child.position);
        }
    }
}
