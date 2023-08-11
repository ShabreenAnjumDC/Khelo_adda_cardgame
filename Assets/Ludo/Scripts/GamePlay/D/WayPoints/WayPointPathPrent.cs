using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPathPrent : MonoBehaviour
{
    public static WayPointPathPrent instance;

    public List<WayPoints> redWayPointPath, yellowWayPointPath, greenWayPointPath, blueWayPointPath, homePoints, homePointsQuick, commonWayPoint;

    public Transform wayPointsParent;

    void Awake()
    {
        instance = this;
    }
}
