using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class T_Players : MonoBehaviour
{
    public static T_Players instance;

    public int leftWayPointsToMoveOn;
    public int LeftWayPointsToMoveOn;

    public int previousWayPointInt;
    public int currentWayPointInt;
    public int distance;
    public int inAtTurn;
    public int counter = 0;

    public GameObject tokenOptionForMove;
    public TMP_Text[] DiceValue6Options1;

    public T_WayPointPathPrent wayPointPathPrent;

    public AudioSource AudioSource;


    public void Awake()
    {
        leftWayPointsToMoveOn = 52;
        instance = this;
        wayPointPathPrent = FindObjectOfType<T_WayPointPathPrent>();
    }


    // for classic ludo token movement
    public IEnumerator MoveStepsNew(int posiblePos, List<T_WayPoints> wayPointsToMoveOn)
    {
        string namePosiblePos = posiblePos.ToString();

        // for kill, click & set position to home as per backend chips position
        bool moved = false;
        for (int i = 0; i < wayPointPathPrent.homePoints.Count; i++)
        {
            if (i == posiblePos)
            {
                if (previousWayPointInt > 15)
                {
                    Debug.Log("KILL");
                    yield return new WaitForSeconds(1f);
                    counter = 0;
                    transform.parent = wayPointPathPrent.homePoints[i].transform;
                    transform.position = wayPointPathPrent.homePoints[i].transform.position;
                    moved = true;

                    if (L_SoundManager.instance.isSound)
                        L_SoundManager.instance.PlaySound(L_SoundType.GotiCutSound, T_SocketController.instance.transform);

                    break;
                }
            }
        }
        if (moved)
        {
            for (int i = 0; i < wayPointsToMoveOn.Count; i++)
            {
                ReScaleAndRepositionAllPlayerPiece(wayPointsToMoveOn[i].transform);
            }
        }

        bool isMovement = false;

        if (!moved)
        {
            for (int i = 0; i < wayPointsToMoveOn.Count; i++)
            {
                if (wayPointsToMoveOn[i].gameObject.name == "Spot " + namePosiblePos)
                {
                    if (previousWayPointInt > 15) //pehli bar kukri bahar nikal gyi, uske bad step by step aage move ho. uske liye if condition hain
                    {
                        //Labell1:
                        //if (i > counter)
                        while (i > counter)
                        {
                            counter++;
                            transform.position = wayPointsToMoveOn[counter].transform.position;
                            ReScaleAndRepositionAllPlayerPiece(wayPointsToMoveOn[i].transform);
                            transform.parent = wayPointsToMoveOn[counter].transform;
                            transform.localPosition = Vector3.zero;
                            if (L_SoundManager.instance.isSound)
                                AudioSource.Play();
                            isMovement = true;
                            yield return new WaitForSeconds(0.15f);
                            //goto Labell1;
                        }
                    }
                    else  // pehli bar kukri bahar nikale, tab wo first position pe ani chahiye. uske liye else hain
                    {
                        transform.position = wayPointsToMoveOn[i].transform.position;
                        transform.parent = wayPointsToMoveOn[i].transform;
                        transform.localPosition = Vector3.zero;
                        if (L_SoundManager.instance.isSound)
                            AudioSource.Play();
                        isMovement = true;
                        yield return new WaitForSeconds(0f);
                    }
                }
            }

            for (int i = 0; i < wayPointsToMoveOn.Count; i++)
            {
                ReScaleAndRepositionAllPlayerPiece(wayPointsToMoveOn[i].transform);
            }
        }

        PlayHomeAndSafezoneSound(isMovement, distance);
    }

    // for quick ludo token movement
    public IEnumerator MoveStepsNewQuickLudo(int chipsPosInt, int distanceInt, int knockoutsInt, int chipsLostInt, string playerColor, List<T_WayPoints> wayPointsToMoveOn)
    {
        string chipsPosStr = chipsPosInt.ToString();
        bool moved = false;

        // for kill, click & set position to home as per backend chips position
        if (counter > 0)
        {
            if (chipsLostInt >= 1)
            {
                for (int i = 0; i < wayPointPathPrent.homePointsQuick.Count; i++)
                {
                    if (wayPointPathPrent.homePointsQuick[0].gameObject.name == "Spot " + chipsPosStr && playerColor == "red")
                        moved = true;
                    else if (wayPointPathPrent.homePointsQuick[1].gameObject.name == "Spot " + chipsPosStr && playerColor == "yellow")
                        moved = true;
                    else if (wayPointPathPrent.homePointsQuick[2].gameObject.name == "Spot " + chipsPosStr && playerColor == "green")
                        moved = true;
                    else if (wayPointPathPrent.homePointsQuick[3].gameObject.name == "Spot " + chipsPosStr && playerColor == "blue")
                        moved = true;
                }
            }
        }

        for (int i = 0; i < wayPointPathPrent.homePointsQuick.Count; i++)
        {
            if (wayPointPathPrent.homePointsQuick[i].gameObject.name == "Spot " + chipsPosStr)
            {
                if (chipsLostInt >= 1 && moved == true)
                {
                    moved = false;
                    Debug.Log("KILL: " + chipsPosStr);
                    yield return new WaitForSeconds(1f);
                    counter = 0;
                    transform.parent = wayPointPathPrent.homePointsQuick[i].transform;
                    transform.position = wayPointPathPrent.homePointsQuick[i].transform.position;
                    transform.localPosition = Vector3.zero;
                    for (int j = 0; j < wayPointsToMoveOn.Count; j++)
                    {
                        ReScaleAndRepositionAllPlayerPiece(wayPointsToMoveOn[j].transform);
                    }

                    if (L_SoundManager.instance.isSound)
                        L_SoundManager.instance.PlaySound(L_SoundType.GotiCutSound, T_SocketController.instance.transform);

                    break;
                }
            }
        }

        if (moved)
        {
            for (int i = 0; i < wayPointsToMoveOn.Count; i++)
            {
                ReScaleAndRepositionAllPlayerPiece(wayPointsToMoveOn[i].transform);
            }
        }

        bool isMovement = false;

        if (!moved)
        {
            for (int i = 0; i < wayPointsToMoveOn.Count; i++)
            {
                if (wayPointsToMoveOn[i].gameObject.name == "Spot " + chipsPosStr)
                {
                    if (previousWayPointInt > 15)
                    {
                        while (i > counter)
                        {
                            counter++;
                            transform.position = wayPointsToMoveOn[counter].transform.position;
                            ReScaleAndRepositionAllPlayerPiece(wayPointsToMoveOn[i].transform);
                            transform.parent = wayPointsToMoveOn[counter].transform;
                            transform.localPosition = Vector3.zero;
                            if (L_SoundManager.instance.isSound)
                                AudioSource.Play();
                            isMovement = true;
                            yield return new WaitForSeconds(0.15f);
                        }
                    }
                    else
                    {
                        transform.position = wayPointsToMoveOn[i].transform.position;
                        transform.parent = wayPointsToMoveOn[i].transform;
                        transform.localPosition = Vector3.zero;
                        if (L_SoundManager.instance.isSound)
                            AudioSource.Play();
                        isMovement = true;
                        yield return new WaitForSeconds(0f);
                    }
                }
            }

            for (int i = 0; i < wayPointsToMoveOn.Count; i++)
            {
                ReScaleAndRepositionAllPlayerPiece(wayPointsToMoveOn[i].transform);
            }
        }

        PlayHomeAndSafezoneSound(isMovement, distance);

        
    }



    // rescale token according count on one waypoint
    public void ReScaleAndRepositionAllPlayerPiece(Transform go)
    {
        if (go.transform.childCount == 1)
            go.GetComponent<GridLayoutGroup>().cellSize = new Vector2(25f, 34f);
        else if (go.transform.childCount == 2)
            go.GetComponent<GridLayoutGroup>().cellSize = new Vector2(17f, 25f);
        else if (go.transform.childCount == 3)
            go.GetComponent<GridLayoutGroup>().cellSize = new Vector2(15f, 17f);
        else if (go.transform.childCount == 4)
            go.GetComponent<GridLayoutGroup>().cellSize = new Vector2(15f, 17f);
        else if (go.transform.childCount == 5)
            go.GetComponent<GridLayoutGroup>().cellSize = new Vector2(15f, 11f);
        else if (go.transform.childCount == 6)
            go.GetComponent<GridLayoutGroup>().cellSize = new Vector2(15f, 11f);

        LayoutRebuilder.ForceRebuildLayoutImmediate(go.GetComponent<GridLayoutGroup>().GetComponent<RectTransform>());
    }


    void PlayHomeAndSafezoneSound(bool isMovementFunc, int distanceFunc)
    {
        if (isMovementFunc)
        {
            if (distanceFunc == 57)
            {
                if (L_SoundManager.instance.isSound)
                    L_SoundManager.instance.PlaySound(L_SoundType.GotiInHomeSound, T_SocketController.instance.transform);
            }
            else if (distanceFunc == 1 || distanceFunc == 9 || distanceFunc == 14 || distanceFunc == 22 || distanceFunc == 27 || distanceFunc == 35 || distanceFunc == 40 || distanceFunc == 48)
            {
                if (L_SoundManager.instance.isSound)
                    L_SoundManager.instance.PlaySound(L_SoundType.SafeZoneSound, T_SocketController.instance.transform);
            }
        }
    }




    public void OptionMove(List<T_WayPoints> wayPointsToMoveOn)
    {
        for (int i = 0; i < wayPointsToMoveOn.Count; i++)
        {
            //if (Panel_Controller.instance.rotationDegrees == 270)
            //{
            if (T_SocketController.instance.myColor == "red")
            {
                // server thi red niche aave to
                //if (wayPointsToMoveOn[i].gameObject.name == "Spot 26" || wayPointsToMoveOn[i].gameObject.name == "Spot 27" || wayPointsToMoveOn[i].gameObject.name == "Spot 28")
                //{
                //    if (wayPointsToMoveOn[i].transform.childCount != 0)
                //    {
                //        wayPointsToMoveOn[i].transform.GetChild(0).GetChild(0).transform.localPosition = new Vector3(30f, 1.1f, 0f);
                //        Debug.Log("--- " + wayPointsToMoveOn[i].transform.GetChild(0).GetChild(0), wayPointsToMoveOn[i].transform.GetChild(0).GetChild(0));
                //    }
                //}
                //else if (wayPointsToMoveOn[i].gameObject.name == "Spot 52" || wayPointsToMoveOn[i].gameObject.name == "Spot 53" || wayPointsToMoveOn[i].gameObject.name == "Spot 54")
                //{
                //    if (wayPointsToMoveOn[i].transform.childCount != 0)
                //    {
                //        wayPointsToMoveOn[i].transform.GetChild(0).GetChild(0).transform.localPosition = new Vector3(-52f, 1.1f, 0f);
                //        Debug.Log("--- " + wayPointsToMoveOn[i].transform.GetChild(0).GetChild(0), wayPointsToMoveOn[i].transform.GetChild(0).GetChild(0));
                //    }
                //}
                //else
                //{
                //    if (wayPointsToMoveOn[i].transform.childCount != 0)
                //    {
                //        wayPointsToMoveOn[i].transform.GetChild(0).GetChild(0).transform.localPosition = new Vector3(0, 1.1f, 0f);
                //        Debug.Log("--- " + wayPointsToMoveOn[i].transform.GetChild(0).GetChild(0), wayPointsToMoveOn[i].transform.GetChild(0).GetChild(0));
                //    }
                //}
                OptionMoveX(wayPointsToMoveOn, i, "Spot 26", "Spot 27", "Spot 28", "Spot 52", "Spot 53", "Spot 54");
            }

            // yellow nichwe aave to
            //39, 40, 41, 65, 66, 67
            else if (T_SocketController.instance.myColor == "yellow")
            {
                OptionMoveX(wayPointsToMoveOn, i, "Spot 39", "Spot 40", "Spot 41", "Spot 65", "Spot 66", "Spot 67");
            }

            // green hoi to
            // 52 53, 54, 26, 27, 28
            else if (T_SocketController.instance.myColor == "green")
            {
                OptionMoveX(wayPointsToMoveOn, i, "Spot 52", "Spot 53", "Spot 54", "Spot 26", "Spot 27", "Spot 28");
            }

            // blue hoi to
            // 65, 66 ,67, 39, 40, 41
            else if (T_SocketController.instance.myColor == "blue")
            {
                OptionMoveX(wayPointsToMoveOn, i, "Spot 65", "Spot 66", "Spot 67", "Spot 39", "Spot 40", "Spot 41");
            }
        }
    }


    public void OptionMoveX(List<T_WayPoints> wayPointsToMoveOn, int index, string leftA, string leftB, string leftC, string rightA, string rightB, string rightC)
    {
        if (wayPointsToMoveOn[index].gameObject.name == leftA || wayPointsToMoveOn[index].gameObject.name == leftB || wayPointsToMoveOn[index].gameObject.name == leftC)
        {
            for (int i = 0; i < wayPointsToMoveOn[index].transform.childCount; i++)
            {
                wayPointsToMoveOn[index].transform.GetChild(i).GetChild(0).transform.localPosition = new Vector3(50f, 1.1f, 0f);
                //Debug.Log("--- 1 " + wayPointsToMoveOn[index].transform.GetChild(i).GetChild(0), wayPointsToMoveOn[index].transform.GetChild(i).GetChild(0));
                //Canvas.ForceUpdateCanvases();
            }
        }
        else if (wayPointsToMoveOn[index].gameObject.name == rightA || wayPointsToMoveOn[index].gameObject.name == rightB || wayPointsToMoveOn[index].gameObject.name == rightC)
        {
            for (int i = 0; i < wayPointsToMoveOn[index].transform.childCount; i++)
            {
                wayPointsToMoveOn[index].transform.GetChild(i).GetChild(0).transform.localPosition = new Vector3(-50f, 1.1f, 0f);
                //Debug.Log("--- 2 " + wayPointsToMoveOn[index].transform.GetChild(i).GetChild(0), wayPointsToMoveOn[index].transform.GetChild(i).GetChild(0));
            }
        }
        else
        {
            if (wayPointsToMoveOn[index].transform.childCount != 0)
            {
                wayPointsToMoveOn[index].transform.GetChild(0).GetChild(0).transform.localPosition = new Vector3(0, 1.1f, 0f);
                //Debug.Log("--- 3 " + wayPointsToMoveOn[index].transform.GetChild(0).GetChild(0), wayPointsToMoveOn[index].transform.GetChild(0).GetChild(0));
            }
        }
    }
}
