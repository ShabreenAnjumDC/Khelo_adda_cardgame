using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedPlayer : Players
{
    public static new RedPlayer instance;
    public DiceManager redRollingDice;
    public Transform redPathParent;

    private void Start()
    {
        instance = this;
        //redRollingDice = GetComponentInParent<RedHome>().rollingDice;
    }

    // method use for click on red tokens
    public void OnClickRedPlayer()
    {
        if (L_SocketController.instance.myColor == "red")
        {
            if (L_SocketController.instance.playerTurnIndex == L_SocketController.instance.myPlayerIndex)
            {
                if (L_SocketController.instance.waitingForMove == true)
                {
                    if (L_SocketController.instance.myPreservedMoves.Count > 0) // && L_SocketController.instance.throwsLeft == 0)
                    {
                        //if (L_SocketController.instance.isClassicLudo)
                        //{
                        if (distance != 0 && distance != 57) //currentWayPointInt > 15  // home points k bahar ki kukri k liye
                        {
                            Debug.Log("players red > 15");
                            if (L_SocketController.instance.myPreservedMoves.Count == 1) // 6 move ho gayi ho to
                            {
                                Debug.Log("players red> 15 -1");
                                //if (L_SocketController.instance.myPreservedMoves[0] != 6)
                                if ((distance + L_SocketController.instance.myPreservedMoves[0]) <= 57)
                                {
                                    Debug.Log("players red> 15 -2");
                                    L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[0].ToString());
                                }
                            }
                            else if (L_SocketController.instance.myPreservedMoves.Count == 2 || L_SocketController.instance.myPreservedMoves.Count == 3)
                            {
                                GameManager.instance.HideOptionsRedPlayers();

                                tokenOptionForMove.SetActive(true);
                                //DiceValue6Options1[2].gameObject.SetActive(false);

                                for (int i = 0; i < GameManager.instance.redPlayers.Length; i++) //4
                                {
                                    int tempI = i;
                                    for (int j = 0; j < L_SocketController.instance.myPreservedMoves.Count; j++)  //2
                                    {
                                        int tempJ = j;
                                        if ((distance + L_SocketController.instance.myPreservedMoves[tempJ]) <= 57)
                                        {
                                            GameManager.instance.redPlayers[i].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(true);
                                            OptionMove(WayPointPathPrent.instance.redWayPointPath);
                                            GameManager.instance.redPlayers[i].DiceValue6Options1[tempJ].transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
                                            GameManager.instance.redPlayers[i].DiceValue6Options1[tempJ].text = L_SocketController.instance.myPreservedMoves[tempJ].ToString();
                                            GameManager.instance.redPlayers[i].DiceValue6Options1[tempJ].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
                                            {
                                                Debug.Log("players red move tempDice6i=" + tempJ);
                                                if (L_SocketController.instance.myPreservedMoves.Count == 1)
                                                {
                                                    //if ((distance + L_SocketController.instance.myPreservedMoves[0]) <= 57)
                                                    //{
                                                    L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[0].ToString());
                                                    //}
                                                    //Panel_Controller.instance.HideMyDiceOf6(L_SocketController.instance.myPreservedMoves[0].ToString());
                                                }
                                                else
                                                {
                                                    //if ((distance + L_SocketController.instance.myPreservedMoves[tempJ]) <= 57)
                                                    //{
                                                    L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[tempJ].ToString());
                                                    //}
                                                    //Panel_Controller.instance.HideMyDiceOf6(L_SocketController.instance.myPreservedMoves[tempJ].ToString());
                                                }
                                                //if (L_SocketController.instance.myPreservedMoves.Count == 1)
                                                //{
                                                //    L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[0].ToString());
                                                //    Panel_Controller.instance.HideMyDiceOf6(L_SocketController.instance.myPreservedMoves[0].ToString());
                                                //}
                                                //else
                                                //{
                                                //    L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[tempJ].ToString());
                                                //    Panel_Controller.instance.HideMyDiceOf6(L_SocketController.instance.myPreservedMoves[tempJ].ToString());
                                                //}

                                                GameManager.instance.redPlayers[tempI].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(false);
                                                tokenOptionForMove.SetActive(false);
                                                L_SocketController.instance.btnClickRecordWhenDiceValue6[tempJ] = 1;
                                            });
                                        }
                                    }
                                }
                            }
                            //if (L_SocketController.instance.diceValueListWhen6.Count == 2)
                            //{
                            //    tokenOptionForMove.SetActive(true);
                            //    DiceValue6Options1[2].gameObject.SetActive(false);
                            //}
                            //else if (L_SocketController.instance.diceValueListWhen6.Count == 3)
                            //{
                            //    tokenOptionForMove.SetActive(true);
                            //    DiceValue6Options1[2].gameObject.SetActive(true);
                            //}
                        }
                        else if (distance == 0 && distance != 57)
                        {
                            // home point ki kukri select kare to 6 move ho
                            if (L_SocketController.instance.myPreservedMoves.Contains(6) || L_SocketController.instance.myPreservedMoves.Contains(1))
                            {
                                Debug.Log("players red== 6");
                                L_SocketController.instance.PlayerMove(currentWayPointInt, L_SocketController.instance.myPreservedMoves[0].ToString());
                                //Panel_Controller.instance.HideMyDiceOf6(L_SocketController.instance.myPreservedMoves[0].ToString());
                                GameManager.instance.HideOptionsRedPlayers();
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
                                Debug.Log("Players RED posiblePos==currentWaypoint");
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
