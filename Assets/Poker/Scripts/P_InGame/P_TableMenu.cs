using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_TableMenu : MonoBehaviour
{
    public static P_TableMenu instance;

    public Text userNameText;
    public Image profilePic, frameImage;
    public Transform buttonContainer;

    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        
    }

    void Start()
    {
        userNameText.text = PlayerManager.instance.GetPlayerGameData().userName;
    }

    public void OnClickOnButton(string eventName)
    {
        switch (eventName)
        {
            case "close":
                {
                    if ((P_SocketController.instance.isViewer == true) || (P_SocketController.instance.isMyBalanceZero))
                        P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.MenuForViewer);
                    else
                        P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Menu);
                    
                    //Canvas socketCanvas = P_SocketController.instance.transform.GetChild(0).GetComponent<Canvas>();
                    //socketCanvas.sortingOrder = 1;
                    //Canvas.ForceUpdateCanvases();
                }
                break;

            case "backtolobby":
                {
                    P_MainSceneManager.instance.LoadScene(P_MainScenes.LobbyScene);
                }
                break;

            case "leave":
                {
                    if (P_SocketController.instance != null)
                    {
                        if (!P_SocketController.instance.isViewer)
                        {
                            if (P_SocketController.instance.isJoinSended)
                            {
                                P_SocketController.instance.SendLeaveSeat();
                            }
                        }
                        else
                        {
                            P_SocketController.instance.SendLeaveViewer();
                        }
                    }
                    OnClickOnButton("close");
                }
                break;

            case "topup":
                P_InGameUiManager.instance.isCallFromMenu = true;
                P_InGameUiManager.instance.p_BuyinPopup.ShowBuyInPopup(true); //P_InGameUiManager.instance.ShowBuyInPopup(true);
                if (P_InGameUiManager.instance.IsScreenActive(P_InGameScreens.Menu))
                    P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Menu);
                if (P_InGameUiManager.instance.IsScreenActive(P_InGameScreens.MenuForViewer))
                    P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.MenuForViewer);
                break;

            case "leaderboard":
                {
                    if (P_InGameUiManager.instance != null)
                        P_InGameUiManager.instance.ShowScreen(P_InGameScreens.Leaderboard);
                }
                break;

            case "table_settings":
                {
                    if (P_InGameUiManager.instance != null)
                        P_InGameUiManager.instance.ShowScreen(P_InGameScreens.TableSettings);
                }
                break;

            case "table_themes":
                {
                    if (P_InGameUiManager.instance != null)
                        P_InGameUiManager.instance.ShowScreen(P_InGameScreens.SwitchTable);
                }
                break;
            case "exit":
                {
                    if (P_SocketController.instance != null)
                    {
                        if ((!P_SocketController.instance.isViewer) && (!P_SocketController.instance.isMyBalanceZero))
                        {
                            if (P_SocketController.instance.isJoinSended)
                            {
                                P_SocketController.instance.SendLeave();
                            }
                        }
                        else
                        {
                            P_SocketController.instance.SendLeaveViewer();
                        }
                        //StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.4f, () =>
                        //{
                        //    try
                        //    {
                        //        //P_SocketController.instance.SocketClose();
                                
                        //    }
                        //    catch(System.Exception e)
                        //    {
                        //        if (P_GameConstant.enableLog)
                        //            Debug.Log("Socket close: " + e.Message);
                        //    }
                        //}));
                        P_SocketController.instance.isJoinSended = false;
                    }

                    P_MainSceneManager.instance.LoadScene(P_MainScenes.LobbyScene);

                    StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.4f, () =>
                    {
                        if (P_Lobby.instance != null)
                        {
                            for (int i = 0; i < P_Lobby.instance.gameTypeStr.Length; i++)
                            {
                                int tempI = i;
                                if (P_Lobby.instance.gameTypeStr[i] == P_SocketController.instance.lobbySelectedGameType)
                                {
                                    P_Lobby.instance.OnGameTypeButtonClick(tempI, P_SocketController.instance.lobbySelectedGameType);
                                }
                            }
                        }
                        P_SocketController.instance.lobbySelectedGameType = "";
                    }));
                }
                break;
        }
    }
}
