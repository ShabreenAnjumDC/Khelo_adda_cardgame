using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanel_Controller : MonoBehaviour
{
    public static OptionPanel_Controller instance;
    public bool onClickQuickLudo = false, onClickClassicLudo = false;
    public Transform redPathParent, yellowPathParent, greenPathParent, bluePathParent;

    void Awake()
    {
        instance = this;
    }

    //this method use when player select quick ludo option.
    public void OnClickQuickLudo()
    {
        onClickQuickLudo = true;
        for (int i = 0; i < greenPathParent.transform.childCount; i++)
        {
            WayPointPathPrent.instance.greenWayPointPath.Add(greenPathParent.transform.GetChild(i).GetComponent<WayPoints>());
            WayPointPathPrent.instance.yellowWayPointPath.Add(yellowPathParent.transform.GetChild(i).GetComponent<WayPoints>());
            WayPointPathPrent.instance.blueWayPointPath.Add(bluePathParent.transform.GetChild(i).GetComponent<WayPoints>());
            WayPointPathPrent.instance.redWayPointPath.Add(redPathParent.transform.GetChild(i).GetComponent<WayPoints>());
        }
        WayPointPathPrent.instance.redWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(51).GetComponent<WayPoints>());
        WayPointPathPrent.instance.yellowWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(12).GetComponent<WayPoints>());
        WayPointPathPrent.instance.greenWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(25).GetComponent<WayPoints>());
        WayPointPathPrent.instance.blueWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(38).GetComponent<WayPoints>());
    }

    //this method use when player select classic ludo option.
    public void OnClickClassicLudo()
    {
        onClickClassicLudo = true;

        for (int i = 0; i < greenPathParent.transform.childCount; i++)
        {
            WayPointPathPrent.instance.greenWayPointPath.Add(greenPathParent.transform.GetChild(i).GetComponent<WayPoints>());
            WayPointPathPrent.instance.yellowWayPointPath.Add(yellowPathParent.transform.GetChild(i).GetComponent<WayPoints>());
            WayPointPathPrent.instance.blueWayPointPath.Add(bluePathParent.transform.GetChild(i).GetComponent<WayPoints>());
            WayPointPathPrent.instance.redWayPointPath.Add(redPathParent.transform.GetChild(i).GetComponent<WayPoints>());
        }
        WayPointPathPrent.instance.redWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(51).GetComponent<WayPoints>());
        WayPointPathPrent.instance.yellowWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(12).GetComponent<WayPoints>());
        WayPointPathPrent.instance.greenWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(25).GetComponent<WayPoints>());
        WayPointPathPrent.instance.blueWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(38).GetComponent<WayPoints>());
    }
}