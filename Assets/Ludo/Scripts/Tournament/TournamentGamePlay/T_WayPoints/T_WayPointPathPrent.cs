using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_WayPointPathPrent : MonoBehaviour
{
    public static T_WayPointPathPrent instance;

    public List<T_WayPoints> redWayPointPath, yellowWayPointPath, greenWayPointPath, blueWayPointPath, homePoints, homePointsQuick, commonWayPoint;

    public Transform wayPointsParent;

    void Awake()
    {
        instance = this;
    }
}
