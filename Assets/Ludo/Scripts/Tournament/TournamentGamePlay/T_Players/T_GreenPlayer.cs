using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class T_GreenPlayer : T_Players
{
    public static new T_GreenPlayer instance;
    public T_DiceManager greenRollingDice;
    public Transform greenPathParent;

    private void Start()
    {
        instance = this;
    }

    // method use for click on green tokens
    public void OnClickGreenPlayer()
    {
        if (T_SocketController.instance.myColor == "green")
        {
            if (T_SocketController.instance.playerTurnIndex == T_SocketController.instance.myPlayerIndex)
            {
                if (T_SocketController.instance.waitingForMove == true)
                {
                    if (T_SocketController.instance.myPreservedMoves.Count > 0)
                    {
                        if (distance != 0 && distance != 57)
                        {
                            if (T_SocketController.instance.myPreservedMoves.Count == 1)
                            {
                                if ((distance + T_SocketController.instance.myPreservedMoves[0]) <= 57)
                                {
                                    T_SocketController.instance.PlayerMove(currentWayPointInt, T_SocketController.instance.myPreservedMoves[0].ToString());
                                }
                            }
                            else if (T_SocketController.instance.myPreservedMoves.Count == 2 || T_SocketController.instance.myPreservedMoves.Count == 3)
                            {
                                T_GameManager.instance.HideOptionsGreenPlayers();

                                tokenOptionForMove.SetActive(true);

                                for (int i = 0; i < T_GameManager.instance.greenPlayers.Length; i++) //4
                                {
                                    int tempI = i;
                                    for (int j = 0; j < T_SocketController.instance.myPreservedMoves.Count; j++)  //2
                                    {
                                        int tempJ = j;
                                        if ((distance + T_SocketController.instance.myPreservedMoves[tempJ]) <= 57)
                                        {
                                            T_GameManager.instance.greenPlayers[i].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(true);
                                            OptionMove(T_WayPointPathPrent.instance.greenWayPointPath);
                                            T_GameManager.instance.greenPlayers[i].DiceValue6Options1[tempJ].transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
                                            T_GameManager.instance.greenPlayers[i].DiceValue6Options1[tempJ].text = T_SocketController.instance.myPreservedMoves[tempJ].ToString();
                                            T_GameManager.instance.greenPlayers[i].DiceValue6Options1[tempJ].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
                                            {
                                                if (T_SocketController.instance.myPreservedMoves.Count == 1)
                                                {
                                                    T_SocketController.instance.PlayerMove(currentWayPointInt, T_SocketController.instance.myPreservedMoves[0].ToString());
                                                }
                                                else
                                                {
                                                    T_SocketController.instance.PlayerMove(currentWayPointInt, T_SocketController.instance.myPreservedMoves[tempJ].ToString());
                                                }

                                                T_GameManager.instance.greenPlayers[tempI].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(false);
                                                T_GameManager.instance.greenPlayers[tempI].DiceValue6Options1[tempJ].text = "";
                                                tokenOptionForMove.SetActive(false);
                                                T_SocketController.instance.btnClickRecordWhenDiceValue6[tempJ] = 1;
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        else if (distance == 0 && distance != 57)
                        {
                            if (T_SocketController.instance.myPreservedMoves.Contains(6) || T_SocketController.instance.myPreservedMoves.Contains(1))
                            {
                                T_SocketController.instance.PlayerMove(currentWayPointInt, T_SocketController.instance.myPreservedMoves[0].ToString());
                                T_GameManager.instance.HideOptionsGreenPlayers();
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < T_SocketController.instance.posiblePos.Length; i++)
                        {
                            if (T_SocketController.instance.posiblePos[i] == currentWayPointInt)
                            {
                                //AudioSource.Play();
                                T_SocketController.instance.PlayerMove(currentWayPointInt);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}