using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_OptionPanel_Controller : MonoBehaviour
{
    public static T_OptionPanel_Controller instance;
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
            T_WayPointPathPrent.instance.greenWayPointPath.Add(greenPathParent.transform.GetChild(i).GetComponent<T_WayPoints>());
            T_WayPointPathPrent.instance.yellowWayPointPath.Add(yellowPathParent.transform.GetChild(i).GetComponent<T_WayPoints>());
            T_WayPointPathPrent.instance.blueWayPointPath.Add(bluePathParent.transform.GetChild(i).GetComponent<T_WayPoints>());
            T_WayPointPathPrent.instance.redWayPointPath.Add(redPathParent.transform.GetChild(i).GetComponent<T_WayPoints>());
        }
        T_WayPointPathPrent.instance.redWayPointPath.Remove(T_WayPointPathPrent.instance.wayPointsParent.GetChild(51).GetComponent<T_WayPoints>());
        T_WayPointPathPrent.instance.yellowWayPointPath.Remove(T_WayPointPathPrent.instance.wayPointsParent.GetChild(12).GetComponent<T_WayPoints>());
        T_WayPointPathPrent.instance.greenWayPointPath.Remove(T_WayPointPathPrent.instance.wayPointsParent.GetChild(25).GetComponent<T_WayPoints>());
        T_WayPointPathPrent.instance.blueWayPointPath.Remove(T_WayPointPathPrent.instance.wayPointsParent.GetChild(38).GetComponent<T_WayPoints>());
    }

    //this method use when player select classic ludo option.
    public void OnClickClassicLudo()
    {
        onClickClassicLudo = true;

        for (int i = 0; i < greenPathParent.transform.childCount; i++)
        {
            T_WayPointPathPrent.instance.greenWayPointPath.Add(greenPathParent.transform.GetChild(i).GetComponent<T_WayPoints>());
            T_WayPointPathPrent.instance.yellowWayPointPath.Add(yellowPathParent.transform.GetChild(i).GetComponent<T_WayPoints>());
            T_WayPointPathPrent.instance.blueWayPointPath.Add(bluePathParent.transform.GetChild(i).GetComponent<T_WayPoints>());
            T_WayPointPathPrent.instance.redWayPointPath.Add(redPathParent.transform.GetChild(i).GetComponent<T_WayPoints>());
        }
        T_WayPointPathPrent.instance.redWayPointPath.Remove(T_WayPointPathPrent.instance.wayPointsParent.GetChild(51).GetComponent<T_WayPoints>());
        T_WayPointPathPrent.instance.yellowWayPointPath.Remove(T_WayPointPathPrent.instance.wayPointsParent.GetChild(12).GetComponent<T_WayPoints>());
        T_WayPointPathPrent.instance.greenWayPointPath.Remove(T_WayPointPathPrent.instance.wayPointsParent.GetChild(25).GetComponent<T_WayPoints>());
        T_WayPointPathPrent.instance.blueWayPointPath.Remove(T_WayPointPathPrent.instance.wayPointsParent.GetChild(38).GetComponent<T_WayPoints>());
    }
}