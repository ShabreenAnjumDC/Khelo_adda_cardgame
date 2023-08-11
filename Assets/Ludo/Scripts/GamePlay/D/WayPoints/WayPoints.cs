using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    public static WayPoints instance;

    List<WayPoints> wayPointsToReturnOn;
    public WayPointPathPrent wayPointPathParent;
    public List<Players> playersList = new List<Players>();

    void Start()
    {
        instance = this;
        //wayPointPathParent = GetComponentInParent<WayPointPathPrent>();
        wayPointPathParent = FindObjectOfType<WayPointPathPrent>();
    }

    ////this function for add player that stay on which waypoint & kill player when different player are met.
    //public bool AddPlayerPiece(Players playerPiece)
    //{
    //    //Debug.Log("Add PlayerName: " + playerPiece.name);
    //    if(this.name == "CenterPoint")
    //    {
    //        PlayerCompleted(playerPiece);
    //    }

    //    if (this.name != "Spot" && this.name != "Spot (8)" && this.name != "Spot (13)" && this.name != "Spot (21)" && this.name != "Spot (26)" && this.name != "Spot (34)" && this.name != "Spot (39)" && this.name != "Spot (47)" && this.name != "CenterPoint")
    //    {
    //        if (playersList.Count == 1)
    //        {
    //            string previousPlayerName = playersList[0].name;
    //            string currentPlayerName = playerPiece.name;

    //            currentPlayerName = currentPlayerName.Substring(0, currentPlayerName.Length - 4);

    //            if (!previousPlayerName.Contains(currentPlayerName))
    //            {
    //                /*Debug.Log("currentPlayerName: " + currentPlayerName);
    //                Debug.Log("previousPlayerName: " + previousPlayerName);*/

    //                playersList[0].isReadyToMove = false;
    //                StartCoroutine(PlayersNotOnPath(playersList[0]));
    //                playersList[0].numberOfStepsAlreadyMove = 0;

    //                if(OptionPanel_Controller.instance.onClickQuickLudo) //change
    //                {
    //                    playersList[0].isReadyToMove = true;
    //                    playersList[0].numberOfStepsAlreadyMove = 1;
    //                }

    //                if (currentPlayerName.Contains("Green"))
    //                {
    //                    //Debug.Log("green Player Kill Others.");
    //                    GameManager.instance.isGreenPlayerKill = true;
    //                    if (GameManager.instance.greenPlayerKillCount != -1)
    //                        GameManager.instance.greenPlayerKillCount = 1;
    //                }
    //                else if (currentPlayerName.Contains("Yellow"))
    //                {
    //                    //Debug.Log("Yellow Player Kill Others.");
    //                    GameManager.instance.isYellowPlayerKill = true;
    //                    if (GameManager.instance.yellowPlayerKillCount != -1)
    //                        GameManager.instance.yellowPlayerKillCount = 1;
    //                }
    //                else if (currentPlayerName.Contains("Blue"))
    //                {
    //                    //Debug.Log("Blue Player Kill Others.");
    //                    GameManager.instance.isBluePlayerKill = true;
    //                    if (GameManager.instance.bluePlayerKillCount != -1)
    //                        GameManager.instance.bluePlayerKillCount = 1;
    //                }
    //                else if (currentPlayerName.Contains("Red"))
    //                {
    //                    //Debug.Log("Red Player Kill Others.");
    //                    GameManager.instance.isRedPlayerKill = true;
    //                    if (GameManager.instance.redPlayerKillCount != -1)
    //                        GameManager.instance.redPlayerKillCount = 1;
    //                }
    //                RemovePlayerPiece(playersList[0]);
    //                playersList.Add(playerPiece);
    //                return false;
    //            }
    //        }
    //    }
    //    AddPlayer(playerPiece);
    //    return true;
    //}


}