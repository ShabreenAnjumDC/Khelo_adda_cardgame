using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YellowPlayer : Players
{
    public static new YellowPlayer instance;
    public DiceManager yellowRollingDice;
    public Transform yellowPathParent;

    private void Start()
    {
        instance = this;
    }

    // method use for click on yellow tokens
    public void OnClickYellowPlayer()
    {
        if (L_SocketController.instance.myColor == "yellow")
        {
            if (L_SocketController.instance.playerTurnIndex == L_SocketController.instance.myPlayerIndex)
            {
                if (L_SocketController.instance.waitingForMove == true)
                {
                    if (L_SocketController.instance.myPreservedMoves.Count > 0)
                    {
                        if (distance != 0 && distance != 57)
                        {
                            if (L_SocketController.instance.myPreservedMoves.Count == 1)
                            {
                                if ((distance + L_SocketController.instance.myPreservedMoves[0]) <= 57)
                                {
                                    L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[0].ToString());
                                }
                            }
                            else if (L_SocketController.instance.myPreservedMoves.Count == 2 || L_SocketController.instance.myPreservedMoves.Count == 3)
                            {
                                GameManager.instance.HideOptionsYellowPlayers();

                                tokenOptionForMove.SetActive(true);

                                for (int i = 0; i < GameManager.instance.yellowPlayers.Length; i++) //4
                                {
                                    int tempI = i;
                                    for (int j = 0; j < L_SocketController.instance.myPreservedMoves.Count; j++)  //2
                                    {
                                        int tempJ = j;
                                        if ((distance + L_SocketController.instance.myPreservedMoves[tempJ]) <= 57)
                                        {
                                            GameManager.instance.yellowPlayers[i].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(true);
                                            OptionMove(WayPointPathPrent.instance.yellowWayPointPath);
                                            GameManager.instance.yellowPlayers[i].DiceValue6Options1[tempJ].transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
                                            GameManager.instance.yellowPlayers[i].DiceValue6Options1[tempJ].text = L_SocketController.instance.myPreservedMoves[tempJ].ToString();
                                            GameManager.instance.yellowPlayers[i].DiceValue6Options1[tempJ].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
                                            {
                                                if (L_SocketController.instance.myPreservedMoves.Count == 1)
                                                {
                                                    L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[0].ToString());
                                                }
                                                else
                                                {
                                                    L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[tempJ].ToString());
                                                }

                                                GameManager.instance.yellowPlayers[tempI].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(false);
                                                GameManager.instance.yellowPlayers[tempI].DiceValue6Options1[tempJ].text = "";
                                                tokenOptionForMove.SetActive(false);
                                                L_SocketController.instance.btnClickRecordWhenDiceValue6[tempJ] = 1;
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        else if (distance == 0 && distance != 57)
                        {
                            if (L_SocketController.instance.myPreservedMoves.Contains(6) || L_SocketController.instance.myPreservedMoves.Contains(1))
                            {
                                L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[0].ToString());
                                GameManager.instance.HideOptionsYellowPlayers();
                            }
                        }
                        //}
                    }
                    else
                    {
                        for (int i = 0; i < L_SocketController.instance.posiblePos.Length; i++)
                        {
                            if (L_SocketController.instance.posiblePos[i] == currentWayPointInt)
                            {
                                //AudioSource.Play();
                                L_SocketController.instance.PlayerMove(currentWayPointInt);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}