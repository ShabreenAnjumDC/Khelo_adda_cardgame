using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    public WayPointPathPrent wayPointPathParent;
    public List<Players> players = new List<Players>();

    // Start is called before the first frame update
    void Start()
    {
        wayPointPathParent = GetComponentInParent<WayPointPathPrent>();
    }
}
