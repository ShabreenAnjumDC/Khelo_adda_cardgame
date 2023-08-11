using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;

public class P_InGameUiManager : MonoBehaviour
{
    public static P_InGameUiManager instance;

    [SerializeField]
    public P_InGameManager inGameManager;

    public GameObject[] screens; // All screens prefab
    public Transform[] screenLayers; // screen spawn parent
    private List<P_InGameActiveScreens> inGameActiveScreensList = new List<P_InGameActiveScreens>();

    public List<GameObject> TableImages = new List<GameObject>();

    public Text tableInfoText;
    public Text potAmountText;
    public Text gameStartInText;
    public Text tableText;
    public Text errorText;

    public string TempUserID;
    public int emojiContainerVal;

    public int emojiIndex;
    public int otherId;
    public string sentToEmojiValue;

    public string tableId;

    public string inviteCode, isPrivate;
    public int totalTablePlayers;
    public string joinSimilarTableButtonsTextTableType; //tableType // to solve incorrect gametype in buttons text

    private Canvas canvas;

    // from PanelController
    public Image[] timerImages;
    IEnumerator diceTimerCo;
    bool turnTimerBool;
    string turnTimerPlayerTurn = "";
    float turnTimerTime;
    float turnTimerMaxTime;


    [Space(10)]
    public GameObject buyInPopUp;
    public P_BuyinPopup p_BuyinPopup;
    public Text buyInErrorText;
    public bool isTopUp = false;
    public bool isCallFromMenu = false;


    void Awake()
    {
        instance = this;

        canvas = gameObject.GetComponent<Canvas>();
    }

    void OnEnable()
    {
        P_SocketController.instance.isCheckForInternet = true;
    }

    void Start()
    {
        SwitchTables();
    }

    void Update()
    {
        //if (turnTimerBool)
        //    StartTurnTimer();
    }

    public void OnClickOnButton(string eventName)
    {
        switch (eventName)
        {
            case "menu":
                if ((P_SocketController.instance.isViewer == true) || (P_SocketController.instance.isMyBalanceZero == true))
                {
                    ShowScreen(P_InGameScreens.MenuForViewer);

                    if (P_SocketController.instance.isMyBalanceZero == true)
                        P_TableMenu.instance.buttonContainer.Find("TopUp").gameObject.SetActive(true);
                }
                else
                {
                    ShowScreen(P_InGameScreens.Menu);
                }
                break;
            case "hand_history":
                ShowScreen(P_InGameScreens.HandHistory);
                P_HandHistory.instance.OnGetTableHandHistoryDetails(1);
                break;
            case "real_time_result":
                    ShowScreen(P_InGameScreens.RealTimeResult);
                break;
            case "chat":
                ShowScreen(P_InGameScreens.Chat);
                break;
            case "emoji_screen":
                    //if (inGameManager.AmISpectator)
                    //    return;
                    ShowScreen(P_InGameScreens.EmojiScreen);
                break;
            case "hand_ranking_open":
                //P_SocketController.instance.JoinSimilarButtonsDisable();
                //P_SocketController.instance.joinSimilarTblBtnContainer.gameObject.SetActive(false);
                break;
            case "hand_ranking_close":
                //P_SocketController.instance.JoinSimilarButtonsEnable();
                //P_SocketController.instance.joinSimilarTblBtnContainer.gameObject.SetActive(true);
                break;
        }
    }

    public void ShowScreen(P_InGameScreens screenName, object[] parameter = null)
    {
        int layer = (int)GetScreenLayer(screenName);
        for (int i = layer + 1; i < screenLayers.Length; i++)
        {
            DestroyScreen((P_IGScreenLayer)i);
        }

        if (!IsScreenActive(screenName))
        {
            DestroyScreen(GetScreenLayer(screenName));

            P_InGameActiveScreens mainMenuScreen = new P_InGameActiveScreens();
            mainMenuScreen.screenName = screenName;
            mainMenuScreen.screenLayer = GetScreenLayer(screenName);
            if (P_GameConstant.enableLog)
                Debug.Log("screenName:" + screenName + " - screenLayer:" + mainMenuScreen.screenLayer);
            GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;
            inGameActiveScreensList.Add(mainMenuScreen);


            switch (screenName)
            {
                case P_InGameScreens.Menu:
                    //Canvas socketCanvas = P_SocketController.instance.transform.GetChild(0).GetComponent<Canvas>();
                    //socketCanvas.sortingOrder = 0;
                    //Canvas.ForceUpdateCanvases();
                    break;
                case P_InGameScreens.HandHistory:
                    
                    break;
                case P_InGameScreens.RealTimeResult:
                    
                    break;
                case P_InGameScreens.Chat:
                    
                    break;
                case P_InGameScreens.EmojiScreen:
                    //gm.GetComponent<P_EmojiUIScreenManager>().containerVal = emojiContainerVal;
                    //P_SocketController.instance.JoinSimilarButtonsDisable();
                    //P_SocketController.instance.JoinSimilarButtonsInteractable(false);
                    //P_SocketController.instance.joinSimilarTblBtnContainer.gameObject.SetActive(false);
                    break;

                //    case P_InGameScreens.TopUp:
                //        {
                //            Debug.Log("Init topUp screen");
                //            gm.GetComponent<TopUpScript>().Init((float)parameter[0]);
                //        }
                //        break;

                    //    case P_InGameScreens.RealTimeResult:
                    //        {
                    //            gm.GetComponent<RealTimeResultUiManager>().OnOpen(transform.parent.name);
                    //        }
                    //        break;
                    //    case P_InGameScreens.HandHistory:
                    //        {
                    //            gm.GetComponent<HandHistoryManager>().Init();
                    //        }
                    //        break;
                    //    case P_InGameScreens.PointEarnMsg:
                    //        {
                    //            //gm.GetComponent<PointEarnMsg>().OnOpen();
                    //        }
                    //        break;
                    //    default:
                    //        break;
            }

            //if (screenName != P_InGameScreens.PointEarnMsg)
            //{
            //    canvas.sortingOrder = 2;
            //}

        }
    }

    P_IGScreenLayer GetScreenLayer(P_InGameScreens screenName)
    {
        switch (screenName)
        {
            //case P_InGameScreens.Lobby:
            //    return P_IGScreenLayer.LAYER1;
            case P_InGameScreens.TableSettings:
            case P_InGameScreens.SwitchTable:
            case P_InGameScreens.RealTimeResult:
            case P_InGameScreens.HandHistory:
            case P_InGameScreens.Chat:
            case P_InGameScreens.EmojiScreen:
            case P_InGameScreens.Leaderboard:
                return P_IGScreenLayer.LAYER3;
            //case P_InGameScreens.:
                //return P_IGScreenLayer.LAYER4;
            case P_InGameScreens.Message:
            case P_InGameScreens.Loading:
                return P_IGScreenLayer.LAYER4;
            default:
                return P_IGScreenLayer.LAYER2;
        }
    }

    public bool IsScreenActive(P_InGameScreens screenName)
    {
        for (int i = 0; i < inGameActiveScreensList.Count; i++)
        {
            if (inGameActiveScreensList[i].screenName == screenName)
            {
                return true;
            }
        }

        return false;
    }

    public void DestroyScreen(P_InGameScreens screenName)
    {
        for (int i = 0; i < inGameActiveScreensList.Count; i++)
        {
            if (inGameActiveScreensList[i].screenName == screenName)
            {
                Destroy(inGameActiveScreensList[i].screenObject);
                inGameActiveScreensList.RemoveAt(i);
            }
        }
    }

    public void DestroyScreen(P_IGScreenLayer layerName)
    {
        for (int i = 0; i < inGameActiveScreensList.Count; i++)
        {
            if (inGameActiveScreensList[i].screenLayer == layerName)
            {
                Destroy(inGameActiveScreensList[i].screenObject);
                inGameActiveScreensList.RemoveAt(i);
            }
        }
    }

    public void SwitchTables(int counter = 0)
    {
        //Debug.LogError("counter is " + counter);
        counter = PlayerPrefs.GetInt("TableCount");

        foreach (GameObject g in TableImages)
        {
            g.SetActive(false);
        }
        TableImages[counter].SetActive(true);
        canvas.sortingOrder = 1;
    }

    public void ShowMessage(string messageToShow, Action callBackMethod = null, string okButtonText = "OK")
    {
        if (!IsScreenActive(P_InGameScreens.Message))
        {
            P_InGameActiveScreens mainMenuScreen = new P_InGameActiveScreens();
            mainMenuScreen.screenName = P_InGameScreens.Message;
            mainMenuScreen.screenLayer = GetScreenLayer(P_InGameScreens.Message);

            GameObject gm = Instantiate(screens[(int)P_InGameScreens.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;

            inGameActiveScreensList.Add(mainMenuScreen);

            gm.GetComponent<MessageScript>().ShowSingleButtonPopUp(messageToShow, callBackMethod, okButtonText);
        }
    }


    public void ShowMessage(string messageToShow, Action yesButtonCallBack, Action noButtonCallBack, string yesButtonText = "Yes", string noButtonText = "No")
    {
        if (!IsScreenActive(P_InGameScreens.Message))
        {
            P_InGameActiveScreens mainMenuScreen = new P_InGameActiveScreens();
            mainMenuScreen.screenName = P_InGameScreens.Message;
            mainMenuScreen.screenLayer = GetScreenLayer(P_InGameScreens.Message);

            GameObject gm = Instantiate(screens[(int)P_InGameScreens.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;

            inGameActiveScreensList.Add(mainMenuScreen);
            gm.GetComponent<MessageScript>().ShowDoubleButtonPopUp(messageToShow, yesButtonCallBack, noButtonCallBack, yesButtonText, noButtonText);
        }
    }

    public void OnClickEmoji(int val)
    {
        emojiContainerVal = val;
    }

    public void CallEmojiSocket(int index)
    {
        //emojiIndex = index;
        //Debug.Log("i am here------------ call emoji index " + index + "   " + emojiIndex + "    " + otherId);
        //SocketController.instance.SentEmoji(otherId, InGameUiManager.instance.emojiIndex);
    }









    #region HANDLE_SOCKET_RESPONSE

    public void OnGameCounterSet(string gameConter)
    {
        P_SocketController.instance.isGameCounterStart = true;
        gameStartInText.text = "Game starts in " + gameConter.ToString();
        if (!gameStartInText.gameObject.activeSelf)
        {
            gameStartInText.gameObject.SetActive(true);
        }
        //if (tableText.text != "")
        //    tableText.text = "";

        //HideBottomPanel();
        //HideHoleCards();
        ////P_InGameManager.instance.HidePlayersOnNewGame();  //player hide karva mate.
        //HidefoldSprites();
        //hideCommunityCard();

        //HideCardsAndMsg();

        if (gameConter == "0")
        {

        }
    }


    public void OnPotSet(string str)
    {
        JsonData data = JsonMapper.ToObject(str);

        if (data["pots"].Count > 0)
        {
            //HideAllPots();
            switch (data["pots"].Count)
            {
                case 1:
                    if (data["pots"][0] != null)
                    {
                        P_InGameManager.instance.allPots[4].SetActive(true);
                        P_InGameManager.instance.allPots[4].transform.GetChild(1).GetComponent<Text>().text = data["pots"][0]["amount"].ToString();
                    }
                    break;
                case 2:
                    if (data["pots"][0] != null && data["pots"][1] != null)
                    {
                        P_InGameManager.instance.allPots[7].SetActive(true);
                        P_InGameManager.instance.allPots[7].transform.GetChild(1).GetComponent<Text>().text = data["pots"][0]["amount"].ToString();
                        P_InGameManager.instance.allPots[8].SetActive(true);
                        P_InGameManager.instance.allPots[8].transform.GetChild(1).GetComponent<Text>().text = data["pots"][1]["amount"].ToString();
                    }
                    break;
                case 3:
                    if (data["pots"][0] != null && data["pots"][1] != null && data["pots"][2] != null)
                    {
                        P_InGameManager.instance.allPots[4].SetActive(true);
                        P_InGameManager.instance.allPots[4].transform.GetChild(1).GetComponent<Text>().text = data["pots"][0]["amount"].ToString();
                        P_InGameManager.instance.allPots[5].SetActive(true);
                        P_InGameManager.instance.allPots[5].transform.GetChild(1).GetComponent<Text>().text = data["pots"][1]["amount"].ToString();
                        P_InGameManager.instance.allPots[6].SetActive(true);
                        P_InGameManager.instance.allPots[6].transform.GetChild(1).GetComponent<Text>().text = data["pots"][2]["amount"].ToString();
                    }
                    break;
                case 4:
                    if (data["pots"][0] != null && data["pots"][1] != null && data["pots"][2] != null && data["pots"][3] != null)
                    {
                        P_InGameManager.instance.allPots[0].SetActive(true);
                        P_InGameManager.instance.allPots[0].transform.GetChild(1).GetComponent<Text>().text = data["pots"][0]["amount"].ToString();
                        P_InGameManager.instance.allPots[1].SetActive(true);
                        P_InGameManager.instance.allPots[1].transform.GetChild(1).GetComponent<Text>().text = data["pots"][1]["amount"].ToString();
                        P_InGameManager.instance.allPots[2].SetActive(true);
                        P_InGameManager.instance.allPots[2].transform.GetChild(1).GetComponent<Text>().text = data["pots"][2]["amount"].ToString();
                        P_InGameManager.instance.allPots[3].SetActive(true);
                        P_InGameManager.instance.allPots[3].transform.GetChild(1).GetComponent<Text>().text = data["pots"][3]["amount"].ToString();
                    }
                    break;
            }

            //for (int i = 0; i < data["pots"].Count; i++)
            //{
            //    if (data["pots"][i] != null)
            //    {
                    
            //    }
            //}
        }
        string potAmount = data["roundBets"].ToString();
        if (!potAmountText.gameObject.activeSelf)
            potAmountText.gameObject.SetActive(true);
        potAmountText.text = "Pot: " + potAmount;
        P_InGameManager.instance.potAmount = float.Parse(potAmount);

        bool isShowdown = bool.Parse(data["isShowdown"].ToString());
        if (isShowdown)
        {
            P_InGameManager.instance.PlayerTimerReset();
            P_InGameUiManager.instance.StopIdleTimerFunc();
        }
    }


    public void OnRoundChangeSet(string str)
    {
        GameObject[] activeBetAmount;

        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            P_Players pl = P_InGameManager.instance.playersScript[i];
            // timer
            pl.timerImage.fillAmount = 0f;
            pl.fx_holder.gameObject.SetActive(false);
            StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(1f, () => {
                pl.betAmount.SetActive(false);
            }));
            if (diceTimerCo != null)
            {
                StopCoroutine(diceTimerCo);
            }
            if (pl.betAmount.activeSelf)
            {
                
            }
        }
        //HideAllPots();
    }

    #endregion










    // from PanelController
    public void IdleTimerFunc(string playerTurn)
    {
        // update way
        //turnTimerBool = true;
        //turnTimerPlayerTurn = playerTurn;

        //// update way & also used in coroutine way
        //turnTimerMaxTime = float.Parse(P_SocketController.instance.idleTimeout);
        //turnTimerTime = turnTimerMaxTime;

        // coroutine way
        diceTimerCo = IdleTimer(playerTurn, P_SocketController.instance.idleTimeout);
        StartCoroutine(diceTimerCo);
    }

    //public Image timerRing = null;
    // coroutine for player turn timer & panel highlight animation on/off
    public IEnumerator IdleTimer(string playerTurn, string idleTimerSeconds)
    {
        Image timerRing = null;   /* = timerImages[0]*/   //null
        Transform fx_holder = null;

        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            P_Players pl = P_InGameManager.instance.playersScript[i];
            if (pl.GetPlayerData().userId == playerTurn.ToString())
            {
                timerRing = pl.timerImage;
                //timerRing.fillAmount = 0f;
                fx_holder = pl.fx_holder;
            }
        }

        if (timerRing == null)
        {
            if (P_GameConstant.enableLog)
                Debug.Log("Timer Ring is null.");
            yield return null;
        }
        else
        {
            // working from server second 10, 9, 8....1
            //float localTurnTimerStr = 0f;
            //if (float.TryParse(P_SocketController.instance.turnTimerStr, out localTurnTimerStr))
            //{
            //    float divideAmt = (float.Parse(P_SocketController.instance.idleTimeout) - localTurnTimerStr) / 10;
            //    divideAmt += 0.1f; //10 to 1 is coming from backend
            //    timerRing.fillAmount = divideAmt;
            //    yield return null;
            //}


            // working
            //int timerInt = Int32.Parse(idleTimerSeconds);
            //float divideAmt = (1f / (float)timerInt);
            //while (timerInt >= 0)
            //{
            //    timerRing.fillAmount = divideAmt + timerRing.fillAmount;
            //    yield return new WaitForSeconds(1f);
            //    timerInt--;
            //}


            // working smoothly
            float timerInt = float.Parse(idleTimerSeconds);
            float divideAmt = (1f / timerInt);
            fx_holder.gameObject.SetActive(true);

            while (timerInt >= 0.0f)
            {
                timerRing.fillAmount += (Time.deltaTime * divideAmt);

                fx_holder.rotation = Quaternion.Euler(new Vector3(0, 0, -(timerRing.fillAmount) * 360));

                yield return null;
                timerInt -= Time.deltaTime;
            }



            // try from smooth with server second 10, 9, 8....1
            //            float localTurnTimerStr = 0f;
            //            if (float.TryParse(P_SocketController.instance.turnTimerStr, out localTurnTimerStr))
            //            {
            //                float t = 0;
            //                fx_holder.gameObject.SetActive(true);
            //                while (t <= localTurnTimerStr)
            //                {
            //                    t += Time.deltaTime;
            //                    timerRing.fillAmount = t / localTurnTimerStr;
            //                    fx_holder.rotation = Quaternion.Euler(new Vector3(0, 0, -(timerRing.fillAmount) * 360));
            //                    if (timerRing.fillAmount.ToString("F2") == "0.50") //isSound && 
            //                    {
            //                        //Handheld.Vibrate();
            //#if UNITY_ANDROID && !UNITY_EDITOR
            //        Vibration.Vibrate(400);
            //#endif
            //                    }
            //                    yield return null;
            //                }
            //            }
        }
    }

    // using update()
    public void StartTurnTimer()
    {
        Image timerRing = null;

        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            P_Players pl = P_InGameManager.instance.playersScript[i];
            if (pl.userName.text == turnTimerPlayerTurn)
            {
                timerRing = pl.timerImage;
            }
        }

        if (timerRing == null)
        {
            if (P_GameConstant.enableLog)
                Debug.Log("Timer Ring is null.");
        }
        else
        {
            turnTimerTime += Time.deltaTime;
            timerRing.fillAmount = turnTimerTime / turnTimerMaxTime;
            if (turnTimerTime > 10)
                turnTimerTime = 0;

            //turnTimerTime -= Time.deltaTime;
            //timerRing.fillAmount = turnTimerTime / turnTimerMaxTime; //10f;
            //if (turnTimerTime < 0)
            //    turnTimerTime = 0;
        }
    }

    // method use for stop player turn timer
    public void StopIdleTimerFunc()
    {
        // coroutine way
        if (diceTimerCo != null)
        {
            StopCoroutine(diceTimerCo);

            for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
            {
                P_InGameManager.instance.playersScript[i].timerImage.fillAmount = 0f;
                //inGameManager.instance.players[i].GetComponent<Players>().timerImage.gameObject.SetActive(false);
                //timerTexts[i].text = "00s";
                P_InGameManager.instance.playersScript[i].fx_holder.gameObject.SetActive(false);
            }
        }

        // update way
        //turnTimerBool = false;
        //turnTimerPlayerTurn = "";
        //for (int i = 0; i < P_InGameManager.instance.players.Length; i++)
        //{
        //    P_InGameManager.instance.players[i].GetComponent<P_Players>().timerImage.fillAmount = 0f;
        //}
    }

    public void HideCardsAndMsg()
    {
        if (tableText.text != "")
            tableText.text = "";

        HideBottomPanel();
        HideHoleCards();
        //P_InGameManager.instance.HidePlayersOnNewGame();  //player hide karva mate.
        //HidefoldSprites(); //winner me bich me player join ho uska unhide ho raha tha isi liye comment kiya
        hideCommunityCard();
    }

    public void FoldLoginPlayers(string id)
    {
        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            P_Players pl = P_InGameManager.instance.playersScript[i];
            if (pl.playerData.userId == id)
            {
                pl.foldImage.SetActive(true);
                //Debug.Log("foldImage TRUE FoldLoginPlayers");

                //if (pl.playerData.twoCards[0].transform.parent.gameObject.activeSelf)
                //{
                //    pl.fold2CardsImage.SetActive(true);
                //}
                //else if (pl.playerData.fourCards[0].transform.parent.gameObject.activeSelf)
                //{
                //    pl.fold4CardsImage.SetActive(true);
                //}
                if (pl.playerData.sixCards[0].transform.parent.gameObject.activeSelf)
                {
                    if (P_InGameManager.instance.holeCardCount == 2)
                        pl.fold2CardsImage.SetActive(true);
                    else if (P_InGameManager.instance.holeCardCount == 4)
                        pl.fold4CardsImage.SetActive(true);
                    else if (P_InGameManager.instance.holeCardCount == 5)
                        pl.fold5CardsImage.SetActive(true);
                    else if (P_InGameManager.instance.holeCardCount == 6)
                        pl.fold6CardsImage.SetActive(true);
                }
            }
        }
    }

    public void DealerIconAllFalse()
    {
        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
            P_InGameManager.instance.playersScript[i].dealer.SetActive(false);
    }

    public void AllPlayerPosPlusOn()
    {
        for (int i = 0; i < P_InGameManager.instance.allPlayerPos.Length; i++)
        {
            P_InGameManager.instance.allPlayerPos[i].GetChild(0).gameObject.SetActive(false);
            P_InGameManager.instance.allPlayerPos[i].GetChild(1).gameObject.SetActive(true);
            int tempI = i;
            P_InGameManager.instance.allPlayerPos[i].GetChild(1).GetComponent<Button>().onClick.AddListener(() => {
                isTopUp = false;
                buyInPopUp.GetComponent<P_BuyinPopup>().ShowBuyInPopup(true); // ShowBuyInPopup(true);
            });
        }
    }

    public void AllPlayerPosPlusOff(bool isActive)
    {
        for (int i = 0; i < P_InGameManager.instance.allPlayerPos.Length; i++)
        {
            P_InGameManager.instance.allPlayerPos[i].GetChild(0).gameObject.SetActive(isActive);
            P_InGameManager.instance.allPlayerPos[i].GetChild(1).gameObject.SetActive(!isActive);
        }
    }

    /*
     * timer reset
     * dealer icon false
     * best hand realtimeresult false
     * fold boolean false
     * fold 
     */
    public void ResetPlayersUI()
    {
        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            P_Players pl = P_InGameManager.instance.playersScript[i];

            // timer
            if (diceTimerCo != null)
            {
                StopCoroutine(diceTimerCo);
                pl.timerImage.fillAmount = 0f;
                pl.fx_holder.gameObject.SetActive(false);
            }

            // dealer icon
            pl.dealer.SetActive(false);

            // best hand text
            if (pl.realTimeResult.gameObject.activeSelf)
            {
                pl.realTimeResult.DOFade(0f, 1f).OnComplete(() => {
                    pl.realTimeResult.gameObject.SetActive(false);
                    pl.realTimeResult.text = "";
                    pl.realTimeResult.color = new Color(255f, 255f, 255f, 255f);
                });
            }

            //pl.UpdateLastAction("");

            //winner me bich me player join ho uska unhide ho raha tha isi liye comment kiya
            //pl.playerData.isFold = false;

            //if (pl.foldImage != null) pl.foldImage.SetActive(false);
            //if (pl.fold2CardsImage != null) pl.fold2CardsImage.SetActive(false);
            //if (pl.fold4CardsImage != null) pl.fold4CardsImage.SetActive(false);
        }
    }

    public void ResetPlayerAllData()
    {
        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            P_Players pl = P_InGameManager.instance.playersScript[i];

            pl.ResetData();

            pl.gameObject.SetActive(false);
        }
    }

    public void ResetLastAction()
    {
        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            P_InGameManager.instance.playersScript[i].UpdateLastAction("");
            P_InGameManager.instance.playersScript[i].betAmount.SetActive(false);
        }
    }

    public void hideCommunityCard()
    {
        for (int i = 0; i < P_InGameManager.instance.communityCards.Length; i++)
        {
            if (P_InGameManager.instance.communityCards[i].gameObject.activeSelf)
            {
                int tempI = i;
                P_InGameManager.instance.communityCards[i].DOFade(0f, 0.5f).OnComplete(() => {
                    P_InGameManager.instance.communityCards[tempI].color = new Color32(255, 255, 255, 255);
                    P_InGameManager.instance.communityCards[tempI].sprite = null;
                    P_InGameManager.instance.communityCards[tempI].gameObject.SetActive(false);
                });
            }
        }
    }

    public void HideHoleCards()
    {
        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            int tempi = i;
            P_Players pl = P_InGameManager.instance.playersScript[i];
            //for (int j = 0; j < pl.playerData.twoCards.Length; j++)
            //{
            //    pl.playerData.twoCards[j].sprite = P_CardsManager.instance.cardBackSprite;
            //}
            //for (int j = 0; j < pl.playerData.fourCards.Length; j++)
            //{
            //    pl.playerData.fourCards[j].sprite = P_CardsManager.instance.cardBackSprite;
            //}
            //pl.playerData.fourCards[0].transform.parent.gameObject.SetActive(false);
            //pl.playerData.twoCards[0].transform.parent.gameObject.SetActive(false);
            //pl.playerData.sixCards[0].transform.parent.gameObject.SetActive(false);

            //pl.playerData.twoCards[0].transform.parent.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            //pl.playerData.twoCards[0].transform.parent.DOScale(new Vector3(1f, 1f, 1f), GameConstants.CARD_ANIMATION_DURATION).OnComplete(() =>
            //{
            //    pl.playerData.twoCards[0].transform.parent.gameObject.SetActive(false);
            //    for (int j = 0; j < pl.playerData.twoCards.Length; j++)
            //    {
            //        pl.playerData.twoCards[j].sprite = P_CardsManager.instance.cardBackSprite;
            //    }
            //    for (int j = 0; j < pl.playerData.fourCards.Length; j++)
            //    {
            //        pl.playerData.fourCards[j].sprite = P_CardsManager.instance.cardBackSprite;
            //    }
            //});

            for (int j = 0; j < pl.playerData.sixCards.Length; j++)
            {
                // two card
                //int tempj = j;
                //StartCoroutine(Fade(pl.playerData.twoCards[tempj], 0.5f, false));
                ////pl.playerData.twoCards[tempj].DOKill();
                ////pl.playerData.twoCards[tempj].DOFade(0f, 0.5f);

                //StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.5f, () => {
                //    for (int k = 0; k < pl.playerData.twoCards.Length; k++)
                //    {
                //        pl.playerData.twoCards[k].color = new Color(255, 255, 255, 255);
                //        if (pl.playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
                //        {
                //            pl.playerData.twoCards[k].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                //        }
                //    }

                //    for (int s = 0; s < pl.playerData.twoCards.Length; s++)
                //    {
                //        pl.playerData.twoCards[s].sprite = P_CardsManager.instance.cardBackSprite;
                //    }
                //    pl.playerData.twoCards[0].transform.parent.localScale = new Vector3(1f, 1f, 1f);
                //    pl.playerData.twoCards[0].transform.parent.gameObject.SetActive(false);
                //    if (tempi > 0)
                //    {
                //        pl.playerData.twoCards[0].transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(30f, 5f, 0f);
                //    }
                //}));


                // five card
                int tempj = j;
                StartCoroutine(Fade(pl.playerData.sixCards[tempj], 0.5f, false));
                
                StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.5f, () => {
                    for (int k = 0; k < pl.playerData.sixCards.Length; k++)
                    {
                        pl.playerData.sixCards[k].gameObject.SetActive(false);
                        pl.playerData.sixCards[k].sprite = P_CardsManager.instance.cardBackSprite;
                        pl.playerData.sixCards[k].color = new Color(255, 255, 255, 255);

                        if (pl.playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            pl.playerData.sixCards[k].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        }
                    }
                }));

                //pl.playerData.twoCards[tempj].DOFade(0f, 0.5f).OnComplete(() => //GameConstants.LOCAL_BET_ANIMATION_DURATION
                //{
                //    Debug.Log("DoColor 2");
                //    for (int k = 0; k < pl.playerData.twoCards.Length; k++)
                //    {
                //        Debug.Log("DoColor 3");
                //        pl.playerData.twoCards[k].color = new Color(255, 255, 255, 255);
                //    }

                //    for (int s = 0; s < pl.playerData.twoCards.Length; s++)
                //    {
                //        pl.playerData.twoCards[s].sprite = P_CardsManager.instance.cardBackSprite;
                //    }
                //    pl.playerData.twoCards[0].transform.parent.localScale = new Vector3(1f, 1f, 1f);

                //    Debug.Log("DoColor false 0");
                //    pl.playerData.twoCards[0].transform.parent.gameObject.SetActive(false);
                //    Debug.Log("DoColor false 1");
                //});
            }
            

            if (pl.fold2CardsImage.activeSelf)
                StartCoroutine(Fade(pl.fold2CardsImage.GetComponent<Image>(), 0.5f, false));
            if (pl.fold4CardsImage.activeSelf)
                StartCoroutine(Fade(pl.fold4CardsImage.GetComponent<Image>(), 0.5f, false));
            if (pl.fold5CardsImage.activeSelf)
                StartCoroutine(Fade(pl.fold5CardsImage.GetComponent<Image>(), 0.5f, false));
            if (pl.fold6CardsImage.activeSelf)
                StartCoroutine(Fade(pl.fold6CardsImage.GetComponent<Image>(), 0.5f, false));
            StartCoroutine(P_MainSceneManager.instance.RunAfterDelay(0.5f, () => {
                pl.fold2CardsImage.SetActive(false);
                pl.fold4CardsImage.SetActive(false);
                pl.fold5CardsImage.SetActive(false);
                pl.fold6CardsImage.SetActive(false);
                Color color = Color.black;
                color.a = 0.58f;
                pl.fold2CardsImage.GetComponent<Image>().color = color;
                pl.fold4CardsImage.GetComponent<Image>().color = color;
                pl.fold5CardsImage.GetComponent<Image>().color = color;
                pl.fold6CardsImage.GetComponent<Image>().color = color;


                if (pl.playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
                {
                    pl.playerData.sixCards[0].transform.parent.DOScale(new Vector3(1f, 1f, 1f), GameConstants.CARD_ANIMATION_DURATION).SetEase(Ease.InOutBack);
                }
                if (tempi > 0)
                {
                    pl.playerData.sixCards[0].transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(15f, 5f, 0f);
                }
                pl.playerData.sixCards[0].transform.parent.localScale = new Vector3(1f, 1f, 1f);
                pl.playerData.sixCards[0].transform.parent.gameObject.SetActive(false);
            }));
        }
    }

    IEnumerator Fade(Image image, float fadeTime, bool fadeIn)
    {
        float elapsedTime = 0.0f;
        Color c = image.color;
        while (elapsedTime < fadeTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if (fadeIn)
            {
                c.a = Mathf.Clamp01(elapsedTime / fadeTime);
            }
            else
            {
                c.a = 1f - Mathf.Clamp01(elapsedTime / fadeTime);
            }

            image.color = c;
        }
    }

    public void HideBottomPanel()
    {
        P_InGameManager.instance.actionBtnParent.SetActive(false);

        for (int i = 0; i < P_InGameManager.instance.actionButtons.Length; i++)
        {
            P_InGameManager.instance.actionButtons[i].SetActive(false);
        }
    }

    public void HidefoldSprites()
    {
        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            P_Players pl = P_InGameManager.instance.playersScript[i];
            

            if (pl.playerData.userId == P_SocketController.instance.gamePlayerId)
            {
                if (pl.fold2CardsImage != null && pl.fold2CardsImage.activeSelf) //(pl.playerData.twoCards[0].transform.parent.gameObject.activeSelf)
                {
                    pl.fold2CardsImage.SetActive(false);
                }
                else if (pl.fold4CardsImage != null && pl.fold4CardsImage.activeSelf) //(pl.playerData.fourCards[0].transform.parent.gameObject.activeSelf)
                {
                    pl.fold4CardsImage.GetComponent<Image>().DOFade(0f, 0.5f).From().SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        pl.fold4CardsImage.SetActive(false);
                    });
                }
                else if (pl.fold5CardsImage != null && pl.fold5CardsImage.activeSelf) //(pl.playerData.fourCards[0].transform.parent.gameObject.activeSelf)
                {
                    pl.fold5CardsImage.GetComponent<Image>().DOFade(0f, 0.5f).From().SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        pl.fold5CardsImage.SetActive(false);
                    });
                }
            }
            //if (pl.foldImage.activeInHierarchy)
            //{
            //    pl.foldImage.GetComponent<Image>().DOFade(0f, 0.5f).From().SetEase(Ease.OutQuad).OnComplete(() =>
            //    {
            //        pl.foldImage.SetActive(false);
            //    });
            //}
        }
    }

    public void HidePlayersOnNewGame()
    {
        for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
        {
            P_InGameManager.instance.playersScript[i].gameObject.SetActive(false);
        }
    }

    public void HideAllPots()
    {
        for (int i = 0; i < P_InGameManager.instance.allPots.Length; i++)
        {
            P_InGameManager.instance.allPots[i].SetActive(false);
        }
    }






    void OnDestroy()
    {
        P_SocketController.instance.isCheckForInternet = false;
    }

}

public enum P_IGScreenLayer
{
    LAYER1,
    LAYER2,
    LAYER3,
    LAYER4,
    LAYER5
}

public class P_InGameActiveScreens
{
    public GameObject screenObject;
    public P_InGameScreens screenName;
    public P_IGScreenLayer screenLayer;
}

public enum P_InGameScreens
{
    Message,
    Loading,
    Reconnecting,
    TopUp,
    Menu,
    MenuForViewer,
    TableSettings,
    SwitchTable,
    RealTimeResult,
    HandHistory,
    Chat,
    EmojiScreen,
    Leaderboard,
    //InGameShop,
    //HandRanking,
    //Missions,
    //PointEarnMsg,
    //Tips,
    //SpinWheelScreen,
    //DealerImageScreen,
    //RoomLobby,
    //TableList,
    //PrivateTableList,
    //CreatePrivateTable,
    //Profile,
    //EditProfile,
    //MaineMenuScreen
}

public enum P_PlayerAction
{
    Call,
    Raise,
    Fold,
    Check,
    AllIn,
    Bet,
}

public enum P_SuggestionActions
{
    Call,
    Call_Any,
    Fold,
    Check,
    Null
}

public enum P_Emoji
{
    BeerCheers,
    Bluffing,
    Cherees,
    DangerBomb,
    Donkey,
    Dung,
    Fish,
    Gun,
    Kiss,
    Murgi,
    Oscar,
    Rocket,
    ThumbsUp,
    YouRaPro,
    Rose,
    Perfume,
    Ring,
    Car
}

