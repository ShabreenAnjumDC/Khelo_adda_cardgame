using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_WayPoints : MonoBehaviour
{
    public static T_WayPoints instance;

    List<T_WayPoints> wayPointsToReturnOn;
    public T_WayPointPathPrent wayPointPathParent;
    //public List<Players> playersList = new List<Players>();

    void Start()
    {
        instance = this;
        wayPointPathParent = FindObjectOfType<T_WayPointPathPrent>();
    }
}
