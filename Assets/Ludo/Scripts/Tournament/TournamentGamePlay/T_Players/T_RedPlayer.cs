using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class T_RedPlayer : T_Players
{
    public static new T_RedPlayer instance;
    public T_DiceManager redRollingDice;
    public Transform redPathParent;

    private void Start()
    {
        instance = this;
    }

    // method use for click on red tokens
    public void OnClickRedPlayer()
    {
        if (T_SocketController.instance.myColor == "red")
        {
            if (T_SocketController.instance.playerTurnIndex == T_SocketController.instance.myPlayerIndex)
            {
                if (T_SocketController.instance.waitingForMove == true)
                {
                    if (T_SocketController.instance.myPreservedMoves.Count > 0)
                    {
                        if (distance != 0 && distance != 57)  // home points k bahar ki kukri k liye
                        {
                            Debug.Log("players red > 15");
                            if (T_SocketController.instance.myPreservedMoves.Count == 1) // 6 move ho gayi ho to
                            {
                                Debug.Log("players red> 15 -1");

                                if ((distance + T_SocketController.instance.myPreservedMoves[0]) <= 57)
                                {
                                    Debug.Log("players red> 15 -2");
                                    T_SocketController.instance.PlayerMove(currentWayPointInt, T_SocketController.instance.myPreservedMoves[0].ToString());
                                }
                            }
                            else if (T_SocketController.instance.myPreservedMoves.Count == 2 || T_SocketController.instance.myPreservedMoves.Count == 3)
                            {
                                T_GameManager.instance.HideOptionsRedPlayers();

                                tokenOptionForMove.SetActive(true);

                                for (int i = 0; i < T_GameManager.instance.redPlayers.Length; i++) //4
                                {
                                    int tempI = i;
                                    for (int j = 0; j < T_SocketController.instance.myPreservedMoves.Count; j++)  //2
                                    {
                                        int tempJ = j;
                                        if ((distance + T_SocketController.instance.myPreservedMoves[tempJ]) <= 57)
                                        {
                                            T_GameManager.instance.redPlayers[i].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(true);
                                            OptionMove(T_WayPointPathPrent.instance.redWayPointPath);
                                            T_GameManager.instance.redPlayers[i].DiceValue6Options1[tempJ].transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
                                            T_GameManager.instance.redPlayers[i].DiceValue6Options1[tempJ].text = T_SocketController.instance.myPreservedMoves[tempJ].ToString();
                                            T_GameManager.instance.redPlayers[i].DiceValue6Options1[tempJ].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
                                            {
                                                Debug.Log("players red move tempDice6i=" + tempJ);
                                                if (T_SocketController.instance.myPreservedMoves.Count == 1)
                                                {
                                                    T_SocketController.instance.PlayerMove(currentWayPointInt, T_SocketController.instance.myPreservedMoves[0].ToString());
                                                }
                                                else
                                                {
                                                    T_SocketController.instance.PlayerMove(currentWayPointInt, T_SocketController.instance.myPreservedMoves[tempJ].ToString());
                                                }

                                                T_GameManager.instance.redPlayers[tempI].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(false);
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
                            // home point ki kukri select kare to 6 move ho
                            if (T_SocketController.instance.myPreservedMoves.Contains(6) || T_SocketController.instance.myPreservedMoves.Contains(1))
                            {
                                Debug.Log("players red== 6");
                                T_SocketController.instance.PlayerMove(currentWayPointInt, T_SocketController.instance.myPreservedMoves[0].ToString());
                                T_GameManager.instance.HideOptionsRedPlayers();
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
                                Debug.Log("Players RED posiblePos==currentWaypoint");
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
