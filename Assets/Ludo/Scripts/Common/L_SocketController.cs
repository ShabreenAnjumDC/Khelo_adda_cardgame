using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP.SocketIO;
using System.Collections;
using BestHTTP.JSON;
using LitJson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class L_SocketController : MonoBehaviour
{
    public static L_SocketController instance;

    private const float RESPONSE_READ_DELAY = 0.2f, REQUEST_SEND_DELAY = 0.1f;
    private SocketManager socketManager;

    private List<L_SocketResponse> L_SocketResponse = new List<L_SocketResponse>();
    private List<L_SocketRequest> L_SocketRequest = new List<L_SocketRequest>();

    [SerializeField]
    private L_SocketState L_SocketState;

    public string socketUrl = L_GameConstant.SOCKET_URL + "/socket.io/";

    [SerializeField]
    private string TABLE_ID = "";

    public bool isClassicLudo, isQuickLudo;
    public bool isRegisterSend;

    public string gameId;
    public string gamePlayerId; // user id
    public string gamePlayerToken;
    public string myColor;
    public int swapIndex;
    int isGameObjectFirstTime = 0;

    int killerRed = 0;
    int killerYellow = 0;
    int killerGreen = 0;
    int killerBlue = 0;

    public bool waitingForMove;
    public int[] posiblePos;
    public int[] posiblePosDest;

    public string gameTypeId;
    public string gameVarientId;
    public string gameAmount;
    public string selectedGotiColor;
    public float winAmount;

    public string idleTimeout;
    public string gameTime;
    public string scoreTotalDistance;
    public string timeLeftTurn;

    public string diceValue;
    public int playerTurnIndex = 0, lastPlayerTurnIndex = -1, oneTokenOpenPos, totalPlayersInGame = 0, myPlayerIndex = -1;
    //myPlayerIndex set when first time gameObject received

    public int throwsLeft = 0;
    public List<int> myPreservedMoves = new List<int>();
    //public List<int> diceValueListWhen6 = new List<int>();

    public int[] btnClickRecordWhenDiceValue6 = { 0, 0, 0 }; //set default to 0, when button clicked set to 1 of that button index

    public Dictionary<int, int[]> playerTokensPosiblePos;
    bool firstDiceAnimate = false;
    [HideInInspector]
    public string winnerName;
    public Sprite diceLogo;
    float timeRemaining, userServerTimer;
    bool isStartTime = false;
    [HideInInspector]
    public bool needToFetchLobby = false;
    int turnValue = 0;
    //public List<Token> AllTokens = new List<Token>();
    public JsonData ReturnToStartPoint = null;
    [HideInInspector]
    //public BoardHighlighter lightHighLighter;

    Coroutine applicationQuitCo;
    bool isClose = false;



    void Awake()
    {
        Debug.Log("L socket Awake()");
        instance = this;
        //if (Application.isEditor)
            Application.runInBackground = true;
        SetSocketState(L_SocketState.NULL);
        turnValue = 0;

        Application.wantsToQuit += OnQuit;
    }

    void Start()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 30;

        //Connect();
        //StartCoroutine(WaitAndCheckInternetConnection());
    }

    private bool OnQuit()
    {
        return isClose;
    }

    private void OnEnable()
    {
        Debug.Log("ONENABLE SOCKET");
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Connect();
        StartCoroutine(WaitAndCheckInternetConnection());
    }


    private void Update()
    {
        //if (L_InGameUiManager.instance != null && isStartTime)
        //    StartTimer();
    }


    void StartTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int m = Mathf.FloorToInt(timeRemaining / 60.0f);
            int s = Mathf.FloorToInt(timeRemaining - m * 60f);
            //L_InGameUiManager.instance.timerText.text = string.Format("{0:00}:{1:00}", m, s);
        }
        else
        {
            //L_InGameUiManager.instance.timerText.text = string.Format("{0:00}:{1:00}", 0, 0);
            isStartTime = false;
        }
    }


    private void OnDestroy()
    {
        //    SendLeaveMatchRequest();
        Debug.Log("socket controller OnDestroy");

    }



    bool isPaused = false;

    void OnGUI()
    {
        if (isPaused)
        {
            GUI.Label(new Rect(100, 100, 50, 30), "Game paused");
        }
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            //ResumeApplication();
            Debug.Log("111111 FOCUS: TRUE");
            if (applicationQuitCo != null)
            {
                Debug.Log("111111 FOCUS: TRUE StopCoroutine WaitAfterFocusLost()");
                StopCoroutine(applicationQuitCo);
                SendRejoin();
            }
        }
        else
        {
            //LeaveApplication();
            Debug.Log("111111 FOCUS: FALSE APP QUIT");
            applicationQuitCo = StartCoroutine(WaitAfterFocusLost());
        }
    }
#endif

#if UNITY_EDITOR || UNITY_IOS
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            //LeaveApplication();
            Debug.Log("111111 PAUSE: TRUE");

            //RemovePlayerFromGame();
            applicationQuitCo = StartCoroutine(WaitAfterFocusLost());
        }
        else
        {
            //ResumeApplication();
            Debug.Log("111111 PAUSE: FALSE");
            if (applicationQuitCo != null)
            {
                Debug.Log("111111 PAUSE: FALSE StopCoroutine WaitAfterFocusLost()");
                StopCoroutine(applicationQuitCo);
                SendRejoin();
            }
        }
    }
#endif

    IEnumerator WaitAfterFocusLost()
    {
        Debug.Log("WaitAfterFocusLost() ENTER WaitForSecondsRealtime");
        yield return new WaitForSecondsRealtime(60f);
        Debug.Log("WaitAfterFocusLost() after 60 seconds WaitForSecondsRealtime");
        RemovePlayerFromGame();
    }

    // for reference
    //    void OnApplicationFocus(bool hasFocus)
    //    {
    //        //#if !UNITY_EDITOR
    //        //        isPaused = !hasFocus;
    //        //        if (!isPaused)
    //        //        {
    //        //            MaximizeAppEvent();
    //        //        }
    //        //        if (isPaused)
    //        //        {
    //        //            MinimizeAppEvent();
    //        //        }
    //        //#endif
    //    }

    //    void OnApplicationPause(bool pauseStatus)
    //    {
    ////#if !UNITY_EDITOR
    ////        isPaused = pauseStatus;
    ////        if (isPaused)
    ////        {
    ////            MinimizeAppEvent();
    ////        }
    ////        else
    ////        {
    ////            MaximizeAppEvent();
    ////        }
    ////#endif
    //    }

    void OnApplicationQuit()
    {
        Debug.Log("111111 OnApplicationQuit: TRUE");
        OnClickYes();
    }

    void OnClickYes()
    {
        Application.wantsToQuit -= OnQuit;
        isClose = true;
        Application.wantsToQuit += OnQuit;
        Application.Quit();
    }





    public void Connect(bool isReconnecting = false)
    {
        ResetConnection(isReconnecting);
        SetSocketState(L_SocketState.Connecting);


        if (!IsInvoking("HandleSocketResponse"))
        {
            InvokeRepeating("HandleSocketResponse", RESPONSE_READ_DELAY, RESPONSE_READ_DELAY);
        }

        if (!IsInvoking("SendSocketRequest"))
        {
            InvokeRepeating("SendSocketRequest", REQUEST_SEND_DELAY, REQUEST_SEND_DELAY);
        }

        ReConnect();
    }


    private void ReConnect()
    {
        socketManager = null;
        SocketOptions socketOptions = new SocketOptions();
        socketOptions.Timeout = new TimeSpan(0, 0, 4);
        socketOptions.Reconnection = false;
        socketOptions.AutoConnect = false;
        socketOptions.ReconnectionDelayMax = new TimeSpan(0, 0, 4);

        socketManager = new SocketManager(new Uri(L_GameConstant.SOCKET_URL + "/socket.io/"), socketOptions);

        Debug.Log("uri: " + socketManager.Uri);

        socketManager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
        socketManager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
        socketManager.Socket.On(SocketIOEventTypes.Error, OnError);

        //Default Events
        socketManager.Socket.On("reconnect", OnReconnect);
        socketManager.Socket.On("reconnecting", OnReconnecting);
        socketManager.Socket.On("reconnect_attempt", OnReconnectAttempt);
        socketManager.Socket.On("reconnect_failed", OnReconnectFailed);
        

        //Ludo
        socketManager.Socket.On("gameStart", GameStart);
        socketManager.Socket.On("gamestop", GameStop);
        socketManager.Socket.On("move", Move);
        socketManager.Socket.On("playerAdded", PlayerAdded);
        socketManager.Socket.On("gameObject", GameObjectData);
        socketManager.Socket.On("animateDice", AnimateDice);
        socketManager.Socket.On("removePlayer", RemovePlayer);
        socketManager.Socket.On("gameError", GameError);
        socketManager.Socket.On("GAME_TIME_LEFT", OnGameTimeLeft);
        socketManager.Socket.On("INVALID", OnInvalid);

        socketManager.Socket.On("rejoin", OnRejoin);
        socketManager.Socket.On("register", Register);

        socketManager.Open();
    }

    private void HandleSocketResponse()
    {
        if (L_SocketResponse.Count > 0)
        {
            L_SocketResponse responseObject = L_SocketResponse[0];
            L_SocketResponse.RemoveAt(0);

#if DEBUG

#if UNITY_EDITOR

#else
            Debug.LogError("Handling ServerResponse = " + responseObject.eventType + "  L_SocketState = " + L_SocketState + "   data = " + responseObject.data);
#endif

#endif
            Debug.Log(gameObject.transform.parent.name + " <color=yellow> " + responseObject.eventType + ",</color> " + responseObject.data);
            switch (responseObject.eventType)
            {
                case L_SocketEvetns.CONNECT:
                    {
                        //SendPlayerDetails();

                        switch (GetSocketState())
                        {
                            case L_SocketState.Connecting:
                                //SetSocketState(L_SocketState.WaitingForOpponent);
                                //Debug.Log(responseObject.eventType + "<color=yellow> IsJoiningPreviousGame " + L_GlobalGameManager.IsJoiningPreviousGame + "</color>");
                                //CreateRoom();
                                SetSocketState(L_SocketState.Connected);
                                break;

                            case L_SocketState.ReConnecting:
                                //SendRejoin();
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case L_SocketEvetns.DISCONNECT:
                    StartReconnectProcedure();
                    break;


                case L_SocketEvetns.RECONNECT_ATTEMPT:
                    break;

                default:
                    Debug.LogError("UnHandlled EventType Found in response eventType = " + responseObject.eventType + "   responseStructure = " + responseObject.data);
                    break;
            }

        }
    }


    private void SendSocketRequest()
    {
        if (L_SocketRequest.Count > 0)
        {
            L_SocketRequest request = L_SocketRequest[0];
            L_SocketRequest.RemoveAt(0);

            if (request.plainDataToBeSend != null)
            {
                socketManager.Socket.Emit(request.emitEvent, request.plainDataToBeSend);

                Debug.Log("sending event = <color=yellow>" + request.emitEvent + "</color>        Plain request = " + request.requestDataStructure + "     Time = " + System.DateTime.Now);
            }
            else if (request.jsonDataToBeSend != null)
            {
                socketManager.Socket.Emit(request.emitEvent, request.jsonDataToBeSend);

                Debug.Log("sending event = <color=yellow>" + request.emitEvent + "</color>        Request = " + request.requestDataStructure + "     Time = " + System.DateTime.Now);
            }
        }
    }

    




    // LISTNER_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------




    #region LISTNER_METHODS

    private void GameStart(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>gameStart</color>: " + responseText);
        JsonData data = JsonMapper.ToObject(responseText);
        gameId = data[0].ToString();
        if (PlayerFinding.instance != null)
        {
            PlayerFinding.instance.OnGameStart(gameId);
        }
    }

    private void GameStop(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>gamestop</color>: " + responseText);
    }

    private void GameError(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=red>gameError</color>: " + responseText);

        if (QuickLudoSelection.instance != null)
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(QuickLudoSelection.instance.errorTMP, "Error in connecting game server", "red", 2f));
        else if (responseText == "INVALID")
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(Panel_Controller.instance.selfPanelMsgText, "Invalid Move", "red", 2f));

    }

    private void OnGameTimeLeft(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        //Debug.Log("Socket => <color=yellow>GAME_TIME_LEFT</color>: " + responseText);
        if (Panel_Controller.instance != null)
        {
            string trimedStr = responseText.Remove(0, 2); //["10:00"]   10:00"]
            trimedStr = trimedStr.Remove((trimedStr.Length - 2), 2); //10:00"]   10:00
            Panel_Controller.instance.gameTimerText.text = trimedStr;
        }
        if (L_SoundManager.instance.isSound)
        {
            if (responseText.Equals("[\"00:10\"]") || responseText.Equals("[\"00:05\"]"))
            {
                L_SoundManager.instance.PlaySound(L_SoundType.TimeOutSound, transform);
            }
        }
    }
    
    private void OnInvalid(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>INVALID</color>: " + responseText);
        if (Panel_Controller.instance != null)
        {
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(Panel_Controller.instance.selfPanelMsgText, "Invalid Move", "red", 2f));

            if (L_SoundManager.instance.isSound)
                GameManager.instance.invalidMoveAudio.Play();
        }
    }

    void OnRejoin(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>rejoin</yellow> = " + responseText + "  Time = " + System.DateTime.Now);
    }

    private void Register(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>register</color>: " + responseText);
    }

    private void Move(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>move</color>: " + responseText);
    }


    private void RemovePlayer(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>removePlayer</color>: " + responseText);

        if (Panel_Controller.instance != null)
        {
            Panel_Controller.instance.hidePanelWhenBack.SetActive(false);
            Panel_Controller.instance.backButton.interactable = true;
        }

        SocketClose();

        if (L_MainMenuController.instance.IsScreenActive(MainMenuScreens.Winner))
        {
            L_MainMenuController.instance.ShowScreen(MainMenuScreens.QuickLudoSelection);
            if (isClassicLudo)
            {
                QuickLudoSelection.instance.isClassicLudo = true;
                QuickLudoSelection.instance.isQuickLudo = false;
            }
            else
            {
                QuickLudoSelection.instance.isQuickLudo = true;
                QuickLudoSelection.instance.isClassicLudo = false;
            }
            QuickLudoSelection.instance.gameTypeId = gameTypeId;
        }

        if (L_MainMenuController.instance.IsScreenActive(MainMenuScreens.PlayerFinding) || L_MainMenuController.instance.IsScreenActive(MainMenuScreens.ClassicLudoGamePlay))
        {
            L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
        }
    }
   

    private void PlayerAdded(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>playerAdded</color>: " + responseText);
        JsonData data = JsonMapper.ToObject(responseText);
        Debug.Log("Socket => PlayerAdded: " + data[0]["playerId"].ToString());
        gamePlayerId = data[0]["playerId"].ToString();
        
        
        
        gamePlayerToken = data[0]["token"].ToString();
        if (QuickLudoSelection.instance != null)
        {
            QuickLudoSelection.instance.OnPlayerAddedReceived();
        }
        
    }


    

    

    private void AnimateDice(Socket socket, Packet packet, object[] args)
    {
        firstDiceAnimate = true;
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>animateDice</color>: " + responseText);
        JsonData data = JsonMapper.ToObject(responseText);
        //Debug.Log("AnimateDice value=" + data[0]);
        diceValue = data[0].ToString();

        int diceValueInt = Int32.Parse(data[0].ToString());

        if (GameManager.instance.manageRollingDice[0].gameObject.activeInHierarchy) //red
        {
            StartCoroutine(GameManager.instance.manageRollingDice[0].DiceTime(diceValueInt)); //Int32.Parse(data[0].ToString())
        }
        else if (GameManager.instance.manageRollingDice[1].gameObject.activeInHierarchy) //yellow
        {
            StartCoroutine(GameManager.instance.manageRollingDice[1].DiceTime(diceValueInt)); //Int32.Parse(data[0].ToString())
        }
        else if (GameManager.instance.manageRollingDice[2].gameObject.activeInHierarchy) //green
        {
            StartCoroutine(GameManager.instance.manageRollingDice[2].DiceTime(diceValueInt)); //Int32.Parse(data[0].ToString())
        }
        else if (GameManager.instance.manageRollingDice[3].gameObject.activeInHierarchy) //blue
        {
            StartCoroutine(GameManager.instance.manageRollingDice[3].DiceTime(diceValueInt, (int diceValueIntOfAction) => {
                //

                //if (playerTurnIndex == myPlayerIndex) // for current login user
                //{
                //    if (lastPlayerTurnIndex > -1)
                //    {
                //int tempDiceValue = diceValueIntOfAction - 1;

                //if (diceValueListWhen6.Count == 0)
                //{
                //    if (diceValueIntOfAction == 6)
                //    {
                //        Debug.Log("AnimateDice() Value 6");
                //        diceValueListWhen6.Add(diceValueIntOfAction);
                //        Panel_Controller.instance.blueUserDiceWhen6[0].sprite = DiceManager.instance.Dice[5];
                //        Panel_Controller.instance.blueUserDiceWhen6[0].gameObject.SetActive(true);

                //        //Panel_Controller.instance.blueUserOptBtnTxtWhen6[0].text = diceValue;
                //        //Panel_Controller.instance.blueUserOptBtnTxtWhen6[0].transform.parent.gameObject.SetActive(true);
                //    }
                //}


                //else if (diceValueListWhen6.Count == 1)
                //{
                //    if (diceValueListWhen6[0] == 6)
                //    {
                //        Debug.Log("AnimateDice() after 6");
                //        diceValueListWhen6.Add(diceValueIntOfAction);
                //        Panel_Controller.instance.blueUserDiceWhen6[1].sprite = DiceManager.instance.Dice[tempDiceValue];
                //        Panel_Controller.instance.blueUserDiceWhen6[1].gameObject.SetActive(true);

                //        //Panel_Controller.instance.blueUserOptBtnTxtWhen6[1].text = diceValue;
                //        //Panel_Controller.instance.blueUserOptBtnTxtWhen6[1].transform.parent.gameObject.SetActive(true);
                //    }
                //}

                //else if (diceValueListWhen6.Count == 2)
                //{
                //    if (diceValueListWhen6[1] == 6)
                //    {
                //        Debug.Log("AnimateDice() after 6");
                //        diceValueListWhen6.Add(diceValueIntOfAction);
                //        Panel_Controller.instance.blueUserDiceWhen6[2].sprite = DiceManager.instance.Dice[tempDiceValue];
                //        Panel_Controller.instance.blueUserDiceWhen6[2].gameObject.SetActive(true);

                //        //Panel_Controller.instance.blueUserOptBtnTxtWhen6[2].text = diceValue;
                //        //Panel_Controller.instance.blueUserOptBtnTxtWhen6[2].transform.parent.gameObject.SetActive(true);
                //    }
                //}
                //    }
                //}
                //
            })); //Int32.Parse(data[0].ToString())
        }



        //L_GamePlay.instance.players[playerTurnIndex].transform.GetChild(3).GetChild(0).GetComponent<Dice>().rolling = false;
        //L_GamePlay.instance.players[playerTurnIndex].transform.GetChild(4).GetChild(0).GetComponent<Dice>().rolling = false;

        //if (L_GamePlay.instance.players[playerTurnIndex].transform.GetChild(3).gameObject.activeSelf)
        //    L_GamePlay.instance.players[playerTurnIndex].transform.GetChild(3).GetChild(0).GetComponent<Dice>().OnClickOnDice(int.Parse(data[0].ToString()));
        //if (L_GamePlay.instance.players[playerTurnIndex].transform.GetChild(4).gameObject.activeSelf)
        //    L_GamePlay.instance.players[playerTurnIndex].transform.GetChild(4).GetChild(0).GetComponent<Dice>().OnClickOnDice(int.Parse(data[0].ToString()));
    }


    private void GameObjectData(Socket socket, Packet packet, object[] args)
    {
        totalPlayersInGame = 0;
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>gameObject</color>: " + responseText);
        JsonData data = JsonMapper.ToObject(responseText);

        bool isFirstTime = false;
        if (isGameObjectFirstTime == 0)
        {
            isFirstTime = true;
            isGameObjectFirstTime = 1;
            //PlayerFinding.instance.OnGameObjectFirstTime(responseText);
            L_MainMenuController.instance.ShowScreen(MainMenuScreens.ClassicLudoGamePlay);
            if (string.IsNullOrEmpty(gameId)) { gameId = data[0]["gameId"].ToString(); }
            Panel_Controller.instance.roomCodeText.text = gameId;

            if (L_SoundManager.instance.isSound)
                L_SoundManager.instance.PlayLoopSound(L_SoundType.BoardGamePlaySound, transform);

            if (isClassicLudo)
            {
                OptionPanel_Controller.instance.OnClickClassicLudo();
                Panel_Controller.instance.gameTypeText.text = "CLASSIC";
            }
            else
            {
                OptionPanel_Controller.instance.OnClickQuickLudo();
                Panel_Controller.instance.gameTypeText.text = "QUICK";
            }


            //int amountInt = 0;
            //if (int.TryParse(gameAmount, out amountInt))
            //{
            //    winAmount = (amountInt + amountInt);
            //    Panel_Controller.instance.winAmountTMP.text = "$" + winAmount.ToString();
            //}

            try
            {
                IDictionary iData = data[0] as IDictionary;
                if (iData.Contains("counter"))
                {
                    gameTime = data[0]["counter"].ToString();

                    Panel_Controller.instance.IdleGameTimerFunc();
                }
            }
            catch (Exception e)
            {
                Debug.Log("counter catch: " + e.Message);
            }

            try
            {
                IDictionary iData = data[0] as IDictionary;
                if (iData.Contains("info"))
                {
                    IDictionary iDataInfo = data[0]["info"] as IDictionary;
                    if (iDataInfo.Contains("winnigAmount"))
                    {
                        Debug.Log("winnigAmount: " + data[0]["info"]["winnigAmount"]);
                        Panel_Controller.instance.winAmountTMP.text = "₹" + data[0]["info"]["winnigAmount"].ToString();
                        float amountFloat = 0;
                        if (float.TryParse(data[0]["info"]["winnigAmount"].ToString(), out amountFloat))
                        {
                            winAmount = amountFloat;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("winning amount catch: " + e.Message);
            }
        }

        int tempPlayerTurnIndex = playerTurnIndex;
        if (int.TryParse(data[0]["playerTurn"].ToString(), out playerTurnIndex))
            lastPlayerTurnIndex = tempPlayerTurnIndex;

        waitingForMove = (bool)data[0]["waitingForMove"];

        posiblePos = new int[data[0]["posiblePos"].Count];
        for (int pp = 0; pp < data[0]["posiblePos"].Count; pp++)
            posiblePos[pp] = Int32.Parse(data[0]["posiblePos"][pp].ToString());

        posiblePosDest = new int[data[0]["posiblePosDest"].Count];
        for (int pd = 0; pd < data[0]["posiblePosDest"].Count; pd++)
            posiblePosDest[pd] = Int32.Parse(data[0]["posiblePosDest"][pd].ToString());

        //// for self turn skip
        //if (playerTurnIndex == myPlayerIndex)
        //    Panel_Controller.instance.selfTurnSkipBtn.interactable = true;
        //else
        //    Panel_Controller.instance.selfTurnSkipBtn.interactable = false;

        // for dice value 6
        if (int.TryParse(data[0]["throwsLeft"].ToString(), out throwsLeft)) { }

        if (!isFirstTime) // && myPreservedMoves.Count > 0
        {
            if (playerTurnIndex != myPlayerIndex)  //turn changed    //clear list & hide self dice when 6 objects
            {
                myPreservedMoves.Clear();
                HideDice6OptionsSelfPanel();
            }
        }

        if (data[0]["winners"].Count > 0)
        {
                StartCoroutine(ShowWinLoseScreen(data[0]["winners"]));
        }

        for (int i = 0; i < data[0]["players"].Count; i++)
        {
            if (data[0]["players"][i] != null)
            {
                if (data[0]["players"][i].Count > 0)
                {
                    if (data[0]["players"][i]["status"].ToString() == "1")
                    {
                        int tempIwinner = i;
                        Panel_Controller.instance.allUsersPanel[IndexSwapForTableRotation(myColor, tempIwinner)].SetActive(false);

                        if (data[0]["players"][i]["color"].ToString() == "red")
                        {
                            Panel_Controller.instance.HidePlayers(GameManager.instance.redPlayers);

                            for (int ipl = 0; ipl < GameManager.instance.redPlayers.Length; ipl++)
                            {
                                GameManager.instance.redPlayers[ipl].gameObject.transform.parent = WayPointPathPrent.instance.homePoints[ipl].transform;
                                GameManager.instance.redPlayers[ipl].gameObject.transform.position = WayPointPathPrent.instance.homePoints[ipl].transform.position;
                            }
                        }
                        else if (data[0]["players"][i]["color"].ToString() == "yellow")
                        {
                            Panel_Controller.instance.HidePlayers(GameManager.instance.yellowPlayers);

                            for (int ipl = 0; ipl < GameManager.instance.yellowPlayers.Length; ipl++)
                            {
                                int iplHome = (ipl + 4);
                                GameManager.instance.yellowPlayers[ipl].gameObject.transform.parent = WayPointPathPrent.instance.homePoints[iplHome].transform;
                                GameManager.instance.yellowPlayers[ipl].gameObject.transform.position = WayPointPathPrent.instance.homePoints[iplHome].transform.position;
                            }
                        }
                        else if (data[0]["players"][i]["color"].ToString() == "green")
                        {
                            Panel_Controller.instance.HidePlayers(GameManager.instance.greenPlayers);

                            for (int ipl = 0; ipl < GameManager.instance.greenPlayers.Length; ipl++)
                            {
                                int iplHome = (ipl + 8);
                                GameManager.instance.greenPlayers[ipl].gameObject.transform.parent = WayPointPathPrent.instance.homePoints[iplHome].transform;
                                GameManager.instance.greenPlayers[ipl].gameObject.transform.position = WayPointPathPrent.instance.homePoints[iplHome].transform.position;
                            }
                        }
                        else if (data[0]["players"][i]["color"].ToString() == "blue")
                        {
                            Panel_Controller.instance.HidePlayers(GameManager.instance.bluePlayers);

                            for (int ipl = 0; ipl < GameManager.instance.bluePlayers.Length; ipl++)
                            {
                                int iplHome = (ipl + 12);
                                GameManager.instance.bluePlayers[ipl].gameObject.transform.parent = WayPointPathPrent.instance.homePoints[iplHome].transform;
                                GameManager.instance.bluePlayers[ipl].gameObject.transform.position = WayPointPathPrent.instance.homePoints[iplHome].transform.position;
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < data[0]["players"].Count; i++)
        {
            if (data[0]["players"][i] == null)
            {
                Debug.Log("gameObject player " + i + " is null");
            }
            else if (data[0]["players"][i] != null)
            {
                if (data[0]["players"][i].Count == 0)
                {
                    Debug.Log("gameObject player " + i + " count is zero");
                }
                else
                {
                    IDictionary iPlayers = data[0]["players"][i] as IDictionary;

                    string playerColor = data[0]["players"][i]["color"].ToString();
                    int knockoutsInt = Int32.Parse(data[0]["players"][i]["stats"]["knockouts"].ToString());
                    int chipsLostInt = Int32.Parse(data[0]["players"][i]["stats"]["chipsLost"].ToString());

                    for (int j = 0; j < data[0]["players"][i]["chips"].Count; j++)
                    {
                        string chipsPosStr = data[0]["players"][i]["chips"][j]["pos"].ToString();
                        int chipsPosInt = Int32.Parse(chipsPosStr);
                        int distanceInt = Int32.Parse(data[0]["players"][i]["chips"][j]["distance"].ToString());

                        if (isFirstTime)
                        {
                            if (isClassicLudo == true)  //change here.
                            {
                                if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "0")
                                {
                                    // red user panel on, name dispaly
                                    Panel_Controller.instance.ShowPlayers(GameManager.instance.redPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        Panel_Controller.instance.tempInfoTMP.text = "Your color is Red";
                                        myColor = "red";
                                        myPlayerIndex = 0;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "4")
                                {
                                    // yellow user panel on, name dispaly
                                    Panel_Controller.instance.ShowPlayers(GameManager.instance.yellowPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        Panel_Controller.instance.tempInfoTMP.text = "Your color is yellow";
                                        myColor = "yellow";
                                        myPlayerIndex = 1;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "8")
                                {
                                    // green user panel on, name dispaly
                                    Panel_Controller.instance.ShowPlayers(GameManager.instance.greenPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        Panel_Controller.instance.tempInfoTMP.text = "Your color is green";
                                        myColor = "green";
                                        myPlayerIndex = 2;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "12")
                                {
                                    // blue user panel on, name dispaly
                                    Panel_Controller.instance.ShowPlayers(GameManager.instance.bluePlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        Panel_Controller.instance.tempInfoTMP.text = "Your color is blue";
                                        myColor = "blue";
                                        myPlayerIndex = 3;
                                    }
                                }
                            }
                            else if (isQuickLudo == true) //change here.
                            {
                                //for quick ludo gameplay.
                                if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "16")
                                {
                                    Panel_Controller.instance.ShowPlayers(GameManager.instance.redPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        Panel_Controller.instance.tempInfoTMP.text = "Your color is Red";
                                        myColor = "red";
                                        myPlayerIndex = 0;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "29")
                                {
                                    Panel_Controller.instance.ShowPlayers(GameManager.instance.yellowPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        Panel_Controller.instance.tempInfoTMP.text = "Your color is yellow";
                                        myColor = "yellow";
                                        myPlayerIndex = 1;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "42")
                                {
                                    Panel_Controller.instance.ShowPlayers(GameManager.instance.greenPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        Panel_Controller.instance.tempInfoTMP.text = "Your color is green";
                                        myColor = "green";
                                        myPlayerIndex = 2;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "55")
                                {
                                    Panel_Controller.instance.ShowPlayers(GameManager.instance.bluePlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        Panel_Controller.instance.tempInfoTMP.text = "Your color is blue";
                                        myColor = "blue";
                                        myPlayerIndex = 3;
                                    }
                                }
                            }

                            if (playerColor == "red")
                                GameManager.instance.redPlayers[j].GetComponent<RedPlayer>().currentWayPointInt = chipsPosInt;
                            else if (playerColor == "yellow")
                                GameManager.instance.yellowPlayers[j].GetComponent<YellowPlayer>().currentWayPointInt = chipsPosInt;
                            else if (playerColor == "green")
                                GameManager.instance.greenPlayers[j].GetComponent<GreenPlayer>().currentWayPointInt = chipsPosInt;
                            else if (playerColor == "blue")
                                GameManager.instance.bluePlayers[j].GetComponent<BluePlayer>().currentWayPointInt = chipsPosInt;

                        }
                        else
                        {
                            if (playerColor == "red")
                            {
                                GameManager.instance.redPlayers[j].GetComponent<RedPlayer>().previousWayPointInt = GameManager.instance.redPlayers[j].GetComponent<RedPlayer>().currentWayPointInt;
                                GameManager.instance.redPlayers[j].GetComponent<RedPlayer>().currentWayPointInt = chipsPosInt;
                                GameManager.instance.redPlayers[j].GetComponent<RedPlayer>().distance = distanceInt;

                                if (isClassicLudo)
                                    StartCoroutine(GameManager.instance.redPlayers[j].GetComponent<RedPlayer>().MoveStepsNew(chipsPosInt, WayPointPathPrent.instance.redWayPointPath));
                                else if (isQuickLudo == true)
                                {
                                    //if (knockoutsInt == 1 && killerRed == 0) // killer
                                    //{
                                    //    WayPointPathPrent.instance.redWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(51).GetComponent<WayPoints>());
                                    //    for (int ri = 0; ri < OptionPanel_Controller.instance.redPathParent.childCount; ri++)
                                    //        WayPointPathPrent.instance.redWayPointPath.Add(OptionPanel_Controller.instance.redPathParent.GetChild(ri).GetComponent<WayPoints>());

                                    //    killerRed = 1;
                                    //}

                                    StartCoroutine(GameManager.instance.redPlayers[j].GetComponent<RedPlayer>().MoveStepsNewQuickLudo(chipsPosInt, distanceInt, knockoutsInt, chipsLostInt, playerColor, WayPointPathPrent.instance.redWayPointPath));
                                }
                            }
                            else if (playerColor == "yellow")
                            {
                                GameManager.instance.yellowPlayers[j].GetComponent<YellowPlayer>().previousWayPointInt = GameManager.instance.yellowPlayers[j].GetComponent<YellowPlayer>().currentWayPointInt;
                                GameManager.instance.yellowPlayers[j].GetComponent<YellowPlayer>().currentWayPointInt = chipsPosInt;
                                GameManager.instance.yellowPlayers[j].GetComponent<YellowPlayer>().distance = distanceInt;

                                if (isClassicLudo)
                                    StartCoroutine(GameManager.instance.yellowPlayers[j].GetComponent<YellowPlayer>().MoveStepsNew(chipsPosInt, WayPointPathPrent.instance.yellowWayPointPath));
                                else if (isQuickLudo == true)
                                {
                                    //if (knockoutsInt == 1 && killerYellow == 0) // killer
                                    //{
                                    //    WayPointPathPrent.instance.yellowWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(12).GetComponent<WayPoints>());
                                    //    for (int ri = 0; ri < OptionPanel_Controller.instance.yellowPathParent.childCount; ri++)
                                    //        WayPointPathPrent.instance.yellowWayPointPath.Add(OptionPanel_Controller.instance.yellowPathParent.GetChild(ri).GetComponent<WayPoints>());

                                    //    killerYellow = 1;
                                    //}

                                    StartCoroutine(GameManager.instance.yellowPlayers[j].GetComponent<YellowPlayer>().MoveStepsNewQuickLudo(chipsPosInt, distanceInt, knockoutsInt, chipsLostInt, playerColor, WayPointPathPrent.instance.yellowWayPointPath));
                                }
                            }
                            else if (playerColor == "green")
                            {
                                GameManager.instance.greenPlayers[j].GetComponent<GreenPlayer>().previousWayPointInt = GameManager.instance.greenPlayers[j].GetComponent<GreenPlayer>().currentWayPointInt;
                                GameManager.instance.greenPlayers[j].GetComponent<GreenPlayer>().currentWayPointInt = chipsPosInt;
                                GameManager.instance.greenPlayers[j].GetComponent<GreenPlayer>().distance = distanceInt;

                                if (isClassicLudo)
                                    StartCoroutine(GameManager.instance.greenPlayers[j].GetComponent<GreenPlayer>().MoveStepsNew(chipsPosInt, WayPointPathPrent.instance.greenWayPointPath));
                                else if (isQuickLudo == true)
                                {
                                    //if (knockoutsInt == 1 && killerGreen == 0) // killer
                                    //{
                                    //    WayPointPathPrent.instance.greenWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(25).GetComponent<WayPoints>());
                                    //    for (int ri = 0; ri < OptionPanel_Controller.instance.greenPathParent.childCount; ri++)
                                    //        WayPointPathPrent.instance.greenWayPointPath.Add(OptionPanel_Controller.instance.greenPathParent.GetChild(ri).GetComponent<WayPoints>());

                                    //    killerGreen = 1;
                                    //}

                                    StartCoroutine(GameManager.instance.greenPlayers[j].GetComponent<GreenPlayer>().MoveStepsNewQuickLudo(chipsPosInt, distanceInt, knockoutsInt, chipsLostInt, playerColor, WayPointPathPrent.instance.greenWayPointPath));
                                }
                            }
                            else if (playerColor == "blue")
                            {
                                GameManager.instance.bluePlayers[j].GetComponent<BluePlayer>().previousWayPointInt = GameManager.instance.bluePlayers[j].GetComponent<BluePlayer>().currentWayPointInt;
                                GameManager.instance.bluePlayers[j].GetComponent<BluePlayer>().currentWayPointInt = chipsPosInt;
                                GameManager.instance.bluePlayers[j].GetComponent<BluePlayer>().distance = distanceInt;

                                if (isClassicLudo)
                                    StartCoroutine(GameManager.instance.bluePlayers[j].GetComponent<BluePlayer>().MoveStepsNew(chipsPosInt, WayPointPathPrent.instance.blueWayPointPath));
                                else if (isQuickLudo == true)
                                {
                                    //if (knockoutsInt == 1 && killerBlue == 0) // killer
                                    //{
                                    //    WayPointPathPrent.instance.blueWayPointPath.Remove(WayPointPathPrent.instance.wayPointsParent.GetChild(38).GetComponent<WayPoints>());
                                    //    for (int ri = 0; ri < OptionPanel_Controller.instance.bluePathParent.childCount; ri++)
                                    //        WayPointPathPrent.instance.blueWayPointPath.Add(OptionPanel_Controller.instance.bluePathParent.GetChild(ri).GetComponent<WayPoints>());

                                    //    killerBlue = 1;
                                    //}

                                    StartCoroutine(GameManager.instance.bluePlayers[j].GetComponent<BluePlayer>().MoveStepsNewQuickLudo(chipsPosInt, distanceInt, knockoutsInt, chipsLostInt, playerColor, WayPointPathPrent.instance.blueWayPointPath));
                                }
                            }
                        }
                    }


                    // for dice value 6
                    if (playerTurnIndex == myPlayerIndex && gamePlayerId == data[0]["players"][i]["playerId"].ToString())
                    {
                        if (iPlayers.Contains("preservedMoves"))
                        {
                            // to add preservedMoves in public int array
                            myPreservedMoves.Clear();

                            for (int pmv = 0; pmv < data[0]["players"][i]["preservedMoves"].Count; pmv++)
                                myPreservedMoves.Add(Int32.Parse(data[0]["players"][i]["preservedMoves"][pmv].ToString()));

                            HideDice6OptionsSelfPanel();

                            if (myPreservedMoves.Count > 0)
                            {
                                for (int iPrCnt = 0; iPrCnt < myPreservedMoves.Count; iPrCnt++)
                                {
                                    int tempDice = myPreservedMoves[iPrCnt] - 1;
                                    Panel_Controller.instance.blueUserDiceWhen6[iPrCnt].sprite = DiceManager.instance.Dice[tempDice];
                                    Panel_Controller.instance.blueUserDiceWhen6[iPrCnt].gameObject.SetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        // for self turn skip
        if (playerTurnIndex == myPlayerIndex)
            Panel_Controller.instance.selfTurnSkipBtn.interactable = true;
        else
            Panel_Controller.instance.selfTurnSkipBtn.interactable = false;

        if (playerTurnIndex != myPlayerIndex)
        {
            if (myColor == "red")
                GameManager.instance.HideOptionsRedPlayers();
            else if (myColor == "green")
                GameManager.instance.HideOptionsGreenPlayers();
            else if (myColor == "yellow")
                GameManager.instance.HideOptionsYellowPlayers();
            else if (myColor == "blue")
                GameManager.instance.HideOptionsBluePlayers();
        }

        if (isFirstTime)
        {
            Panel_Controller.instance.RotateBoard(selectedGotiColor, myColor, myPlayerIndex);

            for (int i = 0; i < data[0]["players"].Count; i++)
            {
                if (data[0]["players"][i] != null)
                {
                    if (isQuickLudo == true)
                    {
                        //for quick ludo gameplay.
                        for (int j = 0; j < data[0]["players"][i]["chips"].Count; j++)
                        {
                            if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "16")
                            {
                                for (int h = 0; h < WayPointPathPrent.instance.wayPointsParent.childCount; h++)
                                {
                                    if (WayPointPathPrent.instance.wayPointsParent.GetChild(h).name == "Spot " + data[0]["players"][i]["chips"][0]["pos"].ToString())
                                    {
                                        GameManager.instance.redPlayers[j].transform.position = WayPointPathPrent.instance.wayPointsParent.GetChild(h).position;
                                        GameManager.instance.redPlayers[j].transform.parent = WayPointPathPrent.instance.wayPointsParent.GetChild(h);
                                        GameManager.instance.redPlayers[j].transform.localPosition = Vector3.zero;
                                        GameManager.instance.redPlayers[j].ReScaleAndRepositionAllPlayerPiece(WayPointPathPrent.instance.wayPointsParent.GetChild(h));
                                    }
                                }
                            }
                            else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "29")
                            {
                                for (int h = 0; h < WayPointPathPrent.instance.wayPointsParent.childCount; h++)
                                {
                                    if (WayPointPathPrent.instance.wayPointsParent.GetChild(h).name == "Spot " + data[0]["players"][i]["chips"][0]["pos"].ToString())
                                    {
                                        GameManager.instance.yellowPlayers[j].transform.position = WayPointPathPrent.instance.wayPointsParent.GetChild(h).position;
                                        GameManager.instance.yellowPlayers[j].transform.parent = WayPointPathPrent.instance.wayPointsParent.GetChild(h);
                                        GameManager.instance.yellowPlayers[j].transform.localPosition = Vector3.zero;
                                        GameManager.instance.yellowPlayers[j].ReScaleAndRepositionAllPlayerPiece(WayPointPathPrent.instance.wayPointsParent.GetChild(h));
                                    }
                                }
                            }
                            else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "42")
                            {
                                for (int h = 0; h < WayPointPathPrent.instance.wayPointsParent.childCount; h++)
                                {
                                    if (WayPointPathPrent.instance.wayPointsParent.GetChild(h).name == "Spot " + data[0]["players"][i]["chips"][0]["pos"].ToString())
                                    {
                                        GameManager.instance.greenPlayers[j].transform.position = WayPointPathPrent.instance.wayPointsParent.GetChild(h).position;
                                        GameManager.instance.greenPlayers[j].transform.parent = WayPointPathPrent.instance.wayPointsParent.GetChild(h);
                                        GameManager.instance.greenPlayers[j].transform.localPosition = Vector3.zero;
                                        GameManager.instance.greenPlayers[j].ReScaleAndRepositionAllPlayerPiece(WayPointPathPrent.instance.wayPointsParent.GetChild(h));
                                    }
                                }
                            }
                            else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "55")
                            {
                                for (int h = 0; h < WayPointPathPrent.instance.wayPointsParent.childCount; h++)
                                {
                                    if (WayPointPathPrent.instance.wayPointsParent.GetChild(h).name == "Spot " + data[0]["players"][i]["chips"][0]["pos"].ToString())
                                    {
                                        GameManager.instance.bluePlayers[j].transform.position = WayPointPathPrent.instance.wayPointsParent.GetChild(h).position;
                                        GameManager.instance.bluePlayers[j].transform.parent = WayPointPathPrent.instance.wayPointsParent.GetChild(h);
                                        GameManager.instance.bluePlayers[j].transform.localPosition = Vector3.zero;
                                        GameManager.instance.bluePlayers[j].ReScaleAndRepositionAllPlayerPiece(WayPointPathPrent.instance.wayPointsParent.GetChild(h));
                                    }
                                }
                            }
                        }
                    }

                    if (data[0]["players"][i]["playerId"].ToString() == PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        Panel_Controller.instance.allUsersPanel[3].SetActive(true);
                        Panel_Controller.instance.allPlayersName[3].text = data[0]["players"][i]["playerName"].ToString();
                    }

                    // default red
                    //0 => panel 3
                    //1 => panel 0
                    //2 => panel 1
                    //3 => panel 2
                    if (myColor == "red" && (i > 0))
                    {
                        int k = (i - 1);
                        Panel_Controller.instance.allUsersPanel[k].SetActive(true);
                        Panel_Controller.instance.allPlayersName[k].text = data[0]["players"][i]["playerName"].ToString();
                    }

                    // default green
                    //0 => panel 1
                    //1 => panel 2
                    //2 => panel 3
                    //3 => panel 0
                    if (myColor == "green" && (i != 2))
                    {
                        if (i == 0)
                        {
                            Panel_Controller.instance.allUsersPanel[1].SetActive(true);
                            Panel_Controller.instance.allPlayersName[1].text = data[0]["players"][i]["playerName"].ToString();
                        }
                        else if (i == 1)
                        {
                            Panel_Controller.instance.allUsersPanel[2].SetActive(true);
                            Panel_Controller.instance.allPlayersName[2].text = data[0]["players"][i]["playerName"].ToString();
                        }
                        else
                        {
                            Panel_Controller.instance.allUsersPanel[0].SetActive(true);
                            Panel_Controller.instance.allPlayersName[0].text = data[0]["players"][i]["playerName"].ToString();
                        }
                    }

                    // default yellow
                    //0 => panel 2
                    //1 => panel 3
                    //2 => panel 0
                    //3 => panel 1
                    if (myColor == "yellow" && (i != 1))
                    {
                        if (i == 0)
                        {
                            Panel_Controller.instance.allUsersPanel[2].SetActive(true);
                            Panel_Controller.instance.allPlayersName[2].text = data[0]["players"][i]["playerName"].ToString();
                        }
                        else if (i == 2)
                        {
                            Panel_Controller.instance.allUsersPanel[0].SetActive(true);
                            Panel_Controller.instance.allPlayersName[0].text = data[0]["players"][i]["playerName"].ToString();
                        }
                        else
                        {
                            Panel_Controller.instance.allUsersPanel[1].SetActive(true);
                            Panel_Controller.instance.allPlayersName[1].text = data[0]["players"][i]["playerName"].ToString();
                        }
                    }

                    // default blue
                    //0 => panel 0
                    //1 => panel 1
                    //2 => panel 2
                    //3 => panel 3
                    if (myColor == "blue" && (i != 3))
                    {
                        Panel_Controller.instance.allUsersPanel[i].SetActive(true);
                        Panel_Controller.instance.allPlayersName[i].text = data[0]["players"][i]["playerName"].ToString();
                    }
                }
            }
        }

        swapIndex = IndexSwapForTableRotation(myColor, playerTurnIndex);

        for (int i = 0; i < data[0]["players"].Count; i++)
        {
            if (data[0]["players"][i] != null)
            {
                //scoreTotalDistance = data[0]["players"][myPlayerIndex]["stats"]["totalDistance"].ToString();
                //Panel_Controller.instance.scoreText.text = "Score : " + scoreTotalDistance;

                if (data[0]["players"][i]["playerId"].ToString() == PlayerManager.instance.GetPlayerGameData().userId)
                {
                    Panel_Controller.instance.scoreTextArr[3].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                }

                if (myColor == "red" && (i > 0))
                {
                    int k = (i - 1);
                    Panel_Controller.instance.scoreTextArr[k].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                }

                if (myColor == "green" && (i != 2))
                {
                    if (i == 0)
                    {
                        Panel_Controller.instance.scoreTextArr[1].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                    else if (i == 1)
                    {
                        Panel_Controller.instance.scoreTextArr[2].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                    else
                    {
                        Panel_Controller.instance.scoreTextArr[0].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                }

                if (myColor == "yellow" && (i != 1))
                {
                    if (i == 0)
                    {
                        Panel_Controller.instance.scoreTextArr[2].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                    else if (i == 2)
                    {
                        Panel_Controller.instance.scoreTextArr[0].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                    else
                    {
                        Panel_Controller.instance.scoreTextArr[1].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                }

                if (myColor == "blue" && (i != 3))
                {
                    Panel_Controller.instance.scoreTextArr[i].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                }
            }
        }

        int throwsLeftConverted = 0;
        if (waitingForMove == false)
        {
            if (Int32.TryParse(data[0]["throwsLeft"].ToString(), out throwsLeftConverted))
            {
                if (throwsLeftConverted > 0)
                {
                    Panel_Controller.instance.ShowDice(swapIndex);

                    if (playerTurnIndex == myPlayerIndex)
                    {
                        if (L_SoundManager.instance.isVibrate)
                            L_SoundManager.instance.PlayVibrate();

                        if (L_SoundManager.instance.isSound)
                            L_SoundManager.instance.PlaySound(L_SoundType.YourTurnSound, transform);
                    }
                }
            }
        }

        SwapForIdleTurnCount(data[0]["players"], myColor);

        try
        {
            IDictionary iData = data[0] as IDictionary;
            if (iData.Contains("idleTimeout"))
            {
                idleTimeout = data[0]["idleTimeout"].ToString();

                if (waitingForMove == false)
                {
                    if (throwsLeftConverted > 0)
                        Panel_Controller.instance.IdleTimerFunc(swapIndex);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("timeout catch: " + e.Message);
        }

        if (waitingForMove == true)
            Panel_Controller.instance.HighlightKukriAnimation(playerTurnIndex);

    }

    int IndexSwapForTableRotation(string myColor, int playerTurnIndex)
    {
        int swapIndex = playerTurnIndex;

        // turn index 0/1/2/3

        // default red
        //0 => panel 3
        //1 => panel 0
        //2 => panel 1
        //3 => panel 2
        if (myColor == "red")
        {
            if (playerTurnIndex == 0)
                swapIndex = 3;
            else if (playerTurnIndex == 1)
                swapIndex = 0;
            else if (playerTurnIndex == 2)
                swapIndex = 1;
            else if (playerTurnIndex == 3)
                swapIndex = 2;
        }
        // default yellow
        //0 => panel 2
        //1 => panel 3
        //2 => panel 0
        //3 => panel 1
        else if (myColor == "yellow")
        {
            if (playerTurnIndex == 0)
                swapIndex = 2;
            else if (playerTurnIndex == 1)
                swapIndex = 3;
            else if (playerTurnIndex == 2)
                swapIndex = 0;
            else if (playerTurnIndex == 3)
                swapIndex = 1;
        }
        // default green
        //0 => panel 1
        //1 => panel 2
        //2 => panel 3
        //3 => panel 0
        else if (myColor == "green")
        {
            if (playerTurnIndex == 0)
                swapIndex = 1;
            else if (playerTurnIndex == 1)
                swapIndex = 2;
            else if (playerTurnIndex == 2)
                swapIndex = 3;
            else if (playerTurnIndex == 3)
                swapIndex = 0;
        }
        // default blue
        //0 => panel 0
        //1 => panel 1
        //2 => panel 2
        //3 => panel 3
        else if (myColor == "blue")
        {
            if (playerTurnIndex == 0)
                swapIndex = 0;
            else if (playerTurnIndex == 1)
                swapIndex = 1;
            else if (playerTurnIndex == 2)
                swapIndex = 2;
            else if (playerTurnIndex == 3)
                swapIndex = 3;
        }

        return swapIndex;
    }

    void SwapForIdleTurnCount(JsonData data, string myColor)
    {
        // server index = server color
        // 0 = red
        // 1 = yellow
        // 2 = green
        // 3 = blue


        for (int i = 0; i < data.Count; i++)  //data = data[0]["players"]
        {
            if (data[i] != null)
            {
                int turnIdle = Int32.Parse(data[i]["turnsIdle"].ToString());
                // turnIdle = 0, 1, 2, 3, 4=win/lose

                int thisPlayerSwapIndex = IndexSwapForTableRotation(myColor, i); // index

                //Debug.Log("thisPlayerSwapIndex="+ thisPlayerSwapIndex, Panel_Controller.instance.idleTimeDots.gParent[thisPlayerSwapIndex].gChild[0].gameObject);

                switch (turnIdle)
                {
                    case 0:
                        for (int j = 0; j < Panel_Controller.instance.idleTimeDots.gParent[thisPlayerSwapIndex].gChild.Count; j++)
                        {
                            Panel_Controller.instance.idleTimeDots.gParent[thisPlayerSwapIndex].gChild[j].SetActive(true);
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                        Panel_Controller.instance.idleTimeDots.gParent[thisPlayerSwapIndex].gChild[(turnIdle - 1)].SetActive(false);
                        break;
                }
            }
        }
    }

    void HideDice6OptionsSelfPanel()
    {
        Panel_Controller.instance.blueUserDiceWhen6[0].gameObject.SetActive(false);
        Panel_Controller.instance.blueUserDiceWhen6[1].gameObject.SetActive(false);
        Panel_Controller.instance.blueUserDiceWhen6[2].gameObject.SetActive(false);

        btnClickRecordWhenDiceValue6[0] = 0;
        btnClickRecordWhenDiceValue6[1] = 0;
        btnClickRecordWhenDiceValue6[2] = 0;
    }

    IEnumerator ShowWinLoseScreen(JsonData data)
    {
        yield return new WaitForSeconds(2f);
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.Winner);

        if (L_SoundManager.instance.isSound)
            L_SoundManager.instance.StopLoopSound(L_SoundType.BoardGamePlaySound, transform);

        if (data[0].ToString() == myPlayerIndex.ToString())
        {
            // self winner
            if (WinnerLooser.instance != null)
            {
                WinnerLooser.instance.SelfWinner(true, winAmount.ToString()); //gameAmount

                if (L_SoundManager.instance.isSound)
                    L_SoundManager.instance.PlaySound(L_SoundType.WonSound, transform);
            }
        }
        else
        {
            // self lose
            if (WinnerLooser.instance != null)
            {
                WinnerLooser.instance.SelfWinner(false, winAmount.ToString()); //gameAmount

                if (L_SoundManager.instance.isSound)
                    L_SoundManager.instance.PlaySound(L_SoundType.LoseSound, transform);
            }
        }
    }

    





    void OnServerConnect(Socket socket, Packet packet, params object[] args)
    {
        //if (L_GlobalGameManager.instance.CanDebugThis(L_SocketEvetns.CONNECT))
        //{
            Debug.Log("Enter in OnServerConnect Time = " + System.DateTime.Now);
        //}
        L_SocketResponse response = new L_SocketResponse();
        response.eventType = L_SocketEvetns.CONNECT;
        L_SocketResponse.Add(response);
    }


    void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
    {
        //if (L_GlobalGameManager.instance.CanDebugThis(L_SocketEvetns.DISCONNECT))
        //{

        //}
        L_SocketResponse response = new L_SocketResponse();
        response.eventType = L_SocketEvetns.DISCONNECT;
        L_SocketResponse.Add(response);
    }



    void OnError(Socket socket, Packet packet, params object[] args)
    {
        if (string.IsNullOrEmpty(TABLE_ID)) //first time connecting
        {
            ReConnect();
        }

        Error error = args[0] as Error;
        switch (error.Code)
        {
            case SocketIOErrors.User:
                Debug.LogError("Exception in an event handler! Time = " + System.DateTime.Now);
                break;
            case SocketIOErrors.Internal:
                Debug.LogError("Internal error! Time = " + System.DateTime.Now);
                break;
            default:
                Debug.LogError("server error! Time = " + System.DateTime.Now);
                break;
        }

    }


    void OnReconnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Reconnected Time = " + System.DateTime.Now);
    }


    void OnReconnecting(Socket socket, Packet packet, params object[] args)
    {

        Debug.Log("Reconnecting Time = " + System.DateTime.Now);
    }

    void OnReconnectAttempt(Socket socket, Packet packet, params object[] args)
    {
        //if (L_GlobalGameManager.instance.CanDebugThis(L_SocketEvetns.RECONNECT_ATTEMPT))
        //{
            Debug.Log("Enter in OnReconnectAttempt Time = " + System.DateTime.Now);
        //}

        L_SocketResponse response = new L_SocketResponse();
        response.eventType = L_SocketEvetns.RECONNECT_ATTEMPT;
        L_SocketResponse.Add(response);
    }

    void OnReconnectFailed(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("ReconnectFailed Time = " + System.DateTime.Now);
    }


    #endregion





    // EMIT_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------


    #region EMIT_METHODS

    public void RollDice()
    {
        Debug.Log("RollDice() gamePlayerId=" + gamePlayerId);
        //string turnUserName = ""; // L_GlobalGameManager.instance.gamePlayerInfos[playerTurnIndex].playerName;
        //if (turnUserName != PlayerManager.instance.GetPlayerGameData().userId)
        //{

        //    return;
        //}

        string requestStringData = "{\"gameId\":\"" + gameId + "\"," +
            "\"playerId\":" + gamePlayerId + "," +
              "\"pos\":" + "92" + "," +
              "\"number\":" + "0" + "," +
              "\"chipsToMove\":" + "1" + "}";

        //Debug.Log("Roll Dice for Ludo ---> " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        L_SocketRequest request = new L_SocketRequest();
        request.emitEvent = "move";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        L_SocketRequest.Add(request);

    }

    //public void RollDice100(string number)
    //{
    //    string requestStringData = "{\"gameId\":\"" + gameId + "\"," +
    //        "\"playerId\":" + gamePlayerId + "," +
    //          "\"pos\":100," +
    //          "\"chipsToMove\":" + "1" + "," +
    //          "\"number\":" + number + "}";

    //    //Debug.Log("Roll Dice for Ludo ---> " + requestStringData);
    //    object requestObjectData = Json.Decode(requestStringData);

    //    L_SocketRequest request = new L_SocketRequest();
    //    request.emitEvent = "move";

    //    request.plainDataToBeSend = null;
    //    request.jsonDataToBeSend = requestObjectData;
    //    request.requestDataStructure = requestStringData;
    //    L_SocketRequest.Add(request);
    //}

    public void PlayerMove(int tokenCurrentPos, string number = "0")
    {
        string requestStringData = "{\"gameId\":\"" + gameId + "\"," +
            "\"playerId\":\"" + gamePlayerId + "\"," +
              "\"pos\":" + tokenCurrentPos.ToString() + "," + // token current square number 
              "\"number\":" + number + "," +
              "\"chipsToMove\":1}"; //"," + // dice value
                                    //"\"moveChipsIn\":\"" + diceValue + "\"," +

        //"\"chatMessage\":\"" + null + "\"," +
        //"\"leave\":\"" + null + "\"}";

        //Debug.Log("Move for Ludo ---> " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        L_SocketRequest request = new L_SocketRequest();
        request.emitEvent = "move";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        L_SocketRequest.Add(request);


        //if ((playerTurnIndex == myPlayerIndex) && myPreservedMoves.Length > 0)  // for dice value 6
        //{
        //    for (int dice6i2 = 0; dice6i2 < diceValueListWhen6.Count; dice6i2++)
        //        Panel_Controller.instance.blueUserDiceWhen6[dice6i2].GetComponent<Button>().interactable = true;
        //}

        //if ((playerTurnIndex == myPlayerIndex) && diceValueListWhen6.Count > 0)  // for dice value 6
        //{
        //    for (int dice6i2 = 0; dice6i2 < diceValueListWhen6.Count; dice6i2++)
        //    {
        //        if (btnClickRecordWhenDiceValue6[dice6i2] == 0)
        //        {
        //            //Panel_Controller.instance.blueUserDiceWhen6[dice6i2].GetComponent<Button>().interactable = true;

        //            //Panel_Controller.instance.blueUserOptBtnTxtWhen6[dice6i2].transform.parent.gameObject.SetActive(true);
        //            //Panel_Controller.instance.blueUserOptBtnTxtWhen6[dice6i2].transform.parent.GetComponent<Button>().interactable = true;
        //        }
        //    }
        //}
    }


    public void RemovePlayerFromGame()
    {
        string requestStringData = "{" //"{" //\"playerName\":\"" + PlayerManager.instance.GetPlayerGameData().userName + "\"," 
                                       //+ "\"gameId\":\"" + gameId + "\"," 
            + "\"userid\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

        Debug.Log("<color=yellow>removePlayer</color> for Ludo ---> " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);


        socketManager.Socket.Emit("removePlayer", OnAckCallback, requestObjectData);

        if (L_SoundManager.instance.isSound)
            L_SoundManager.instance.StopLoopSound(L_SoundType.BoardGamePlaySound, transform);
    }

    public void SendSkipTurn()
    {
        string requestStringData = "{\"gameId\":\"" + gameId + "\"}";

        Debug.Log("<color=yellow>skipTurn</color> for Ludo ---> " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);


        socketManager.Socket.Emit("skipTurn", OnAckCallback, requestObjectData);
    }

    void OnAckCallback(Socket socket, Packet originalPacket, params object[] args)
    {
        Debug.Log("OnAckCallback - " + socket.Id);
        string responseText = JsonMapper.ToJson(args);
        JsonData data = JsonMapper.ToObject(responseText);

        Debug.Log("OnAckCallback! " + responseText);


        //if (L_MainMenuController.instance != null)
        //{
        //    SceneManager.LoadScene("GameScene");
        //}
        //else if (L_InGameUiManager.instance != null)
        //{
        //    needToFetchLobby = true;
        //    L_GlobalGameManager.instance.LoadScene(L_Scenes.MainMenuScene);
        //}
    }

    public void RequestRegisterForGame(string gameId, string varientId) //(int totalPlayer, string timeData)
    {
        //string requestStringData = "{\"playerName\":\"" + PlayerManager.instance.GetPlayerGameData().userName + "\"," +
        //    "\"gameType\":\"" + totalPlayer + "\"," +
        //    "\"time\":\"" + timeData + "\"," +
        //     "\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

        //string color = "blue";
        //switch (selectedGotiColor)
        //{
        //    case "RedToggle":
        //        color = "red";
        //        break;
        //    case "YellowToggle":
        //        color = "yellow";
        //        break;
        //    case "GreenToggle":
        //        color = "green";
        //        break;
        //    case "BlueToggle":
        //        color = "blue";
        //        break;
        //}

        string requestStringData = "{" +
            "\"gameId\":" + gameId + "," +
             "\"varient\": \"" + varientId + "\"," +
             "\"userId\":" + PlayerManager.instance.GetPlayerGameData().userId + "}";
        //"\"color\":\"" + color + "\"}";

        Debug.Log("Registering for Ludo ---> " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        L_SocketRequest request = new L_SocketRequest();
        request.emitEvent = "register";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        L_SocketRequest.Add(request);
        isRegisterSend = true;

    }

    void SendRejoin()
    {
        if (!string.IsNullOrEmpty(gameId))
        {
            string requestStringData = "{\"playerId\":" + PlayerManager.instance.GetPlayerGameData().userId + ","
                                           + "\"gameId\":\"" + gameId + "\"}";

            Debug.Log("<color=yellow>rejoin</color> for Ludo ---> " + requestStringData);
            object requestObjectData = Json.Decode(requestStringData);


            socketManager.Socket.Emit("rejoin", requestObjectData);
        }
    }

   

    //public void SentEmoji(int otherUserId, int emojiIndex)
    //{

    //    string requestStringData = "{\"sentBy\":\"" + ((int.Parse(PlayerManager.instance.GetPlayerGameData().userId)).ToString() + "\"," +
    //        "\"sentTo\":\"" + otherUserId + "\"," +
    //        "\"deductionValue\":\"" + 2 + "\"," +
    //        "\"emojiIndex\":\"" + emojiIndex + "\"," +
    //        "\"tableId\":\"" + int.Parse(TABLE_ID)).ToString() + "\"}";


    //    object requestObjectData = Json.Decode(requestStringData);


    //    L_SocketRequest request = new L_SocketRequest();
    //    request.emitEvent = "sendEmoji";
    //    request.plainDataToBeSend = null;
    //    request.jsonDataToBeSend = requestObjectData;
    //    request.requestDataStructure = requestStringData;
    //    L_SocketRequest.Add(request);
    //}

    #endregion















    // OTHER_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SocketClose()
    {
        ClearVariable();
        ResetConnection();
        //socketManager.Socket.Disconnect();
        //MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
        L_GlobalGameManager.instance.socketController.enabled = false;
        L_GlobalGameManager.instance.socketController.gameObject.SetActive(false);
        if (L_GlobalGameManager.instance.currentScreenPathList.Contains("ClassicLudoSelection") || L_GlobalGameManager.instance.currentScreenPathList.Contains("QuickLudoSelection"))
            L_GlobalGameManager.instance.currentScreenPathList.Clear();
        Debug.Log("list clear count=" + L_GlobalGameManager.instance.currentScreenPathList.Count);
    }

    void ClearVariable()
    {
        TABLE_ID = "";
        gamePlayerId = "";
        gamePlayerToken = "";
        myColor = "";
        //gameTypeId = "";
        gameVarientId = "";
        gameAmount = "";
        selectedGotiColor = "";
        isGameObjectFirstTime = 0;
        playerTurnIndex = 0; lastPlayerTurnIndex = -1; oneTokenOpenPos = -1; totalPlayersInGame = 0; myPlayerIndex = -1;
        isRegisterSend = false;
        posiblePos = new int[0];
        posiblePosDest = new int[0];
        idleTimeout = "";
        gameId = "";
        diceValue = "";
        winAmount = 0;
    }

    public L_SocketState GetSocketState()
    {
        return L_SocketState;
    }

    public void SetSocketState(L_SocketState state)
    {
        Debug.Log("Set Socket State " + state);
        L_SocketState = state;
    }

    public void SetTableId(string tableIdToAssign)
    {
        //L_GlobalGameManager.instance.GetRoomData().socketTableId = tableIdToAssign;
        TABLE_ID = tableIdToAssign;

        //PrefsManager.SetData(PrefsKey.RoomData, JsonUtility.ToJson(L_GlobalGameManager.instance.GetRoomData()));

    }

    public string GetTableID()
    {
        return TABLE_ID;
    }

    public bool IsSocketOpen()
    {
        if (socketManager != null && socketManager.Socket.IsOpen)
            return true;
        else
            return false;
    }

    public void ResetConnection(bool isReconnecting = false)
    {
        if (socketManager != null && socketManager.Socket.IsOpen)
        {
            socketManager.Close();
        }

        L_SocketRequest.Clear();
        L_SocketResponse.Clear();

        if (!isReconnecting)
        {
            if (IsInvoking("HandleSocketResponse"))
            {
                CancelInvoke("HandleSocketResponse");
            }

            if (IsInvoking("SendSocketRequest"))
            {
                CancelInvoke("SendSocketRequest");
            }

            if (GetSocketState() != L_SocketState.NULL)
                SetSocketState(L_SocketState.NULL);
        }

    }









    #region ReconnectProtocols

    private void StartReconnectProcedure()
    {
        if (!isPreocedureRunning)
        {
            StartCoroutine(WaitForReconnect());
        }
    }

    private bool isPreocedureRunning = false;
    private IEnumerator WaitForReconnect()
    {
        isPreocedureRunning = true;

        SetSocketState(L_SocketState.ReConnecting);
        socketManager = null;
        ReConnect();

        int counter = 0, maxCount = 30, reInitialisationCount = 3;

        while (counter < maxCount)
        {
            yield return new WaitForSeconds(1f);

            if (socketManager.Socket.IsOpen)
            {
                counter = maxCount;
            }
            else
            {
                if (counter % reInitialisationCount == 0)
                {
                    ReConnect();
                }
            }

            ++counter;
        }

        if (!socketManager.Socket.IsOpen)
        {
            //InGameUiManager.instance.DestroyScreen(InGameScreens.Reconnecting);
            //InGameUiManager.instance.ShowMessage("Connection error, check your network connection and try again.", () =>
            //{
                StartCoroutine(WaitForReconnect());
            //},

            // () =>
            // {
            //     InGameManager.instance.LoadMainMenu();
            // }, "Retry", "Cancel"
            //);
        }

        isPreocedureRunning = false;
    }

    private IEnumerator WaitAndCheckInternetConnection()
    {
        while (true)
        {
            yield return new WaitForSeconds(L_GameConstant.NETWORK_CHECK_DELAY);

            if (GetSocketState() == L_SocketState.Game_Running)
            {
                if (!L_WebServices.instance.IsInternetAvailable())
                {
                    if (socketManager != null)
                    {
                        socketManager.Close();
                    }
                }
            }
        }
    }

    #endregion


}




public enum L_SocketEvetns
{
    CONNECT,
    DISCONNECT,
    RECONNECT_ATTEMPT,
    NULL
}

[Serializable]
public class L_RabitData
{
    public string tableId;
    public string userId;
}

[Serializable]
public class L_StandUpdata
{
    public string tableId;
    public string userId;
    public int isStatndOut;
}

[Serializable]
public class L_SocketRequest
{
    public object jsonDataToBeSend;
    public string plainDataToBeSend;
    public string emitEvent;
    public string requestDataStructure;
}

[Serializable]
public class L_SocketResponse
{
    public L_SocketEvetns eventType;
    public string data;
}


[Serializable]
public class L_FoldData
{
    public string tableId;
    public string userId;
    public string gameType;
    public L_UserBetData userData;
}

[Serializable]
public class L_MinEvent
{
    public string tableId;
    public string userId;
    public string appStatus;
}

[Serializable]
public class L_BetData
{
    public string tableId;
    public string userId;
    public string bet;
    public L_UserBetData userData;
    public string userAction;

}

[Serializable]
public class L_ReconnectData
{
    public string tableId;
    public string userId;
    public string isYesOrNo;
}


[Serializable]
public class L_SendMessageData
{
    public string userId;
    public string tableId;

    public string from;
    public string to;
    public string title;
    public string desc;
}

[Serializable]
public class L_CoinUpdateData
{
    public string email;
    public string coins;
}

[Serializable]
public class L_UserBetData
{
    public int betData, roundNo;
    public string playerAction;
}

[Serializable]
public class L_RematchData
{
    public string isBuyIn, userId, tableId, coins;
}

[Serializable]
public class L_RoomCreateData
{
    public string roomId;
    public string players;
    public string isPrivate;
    public string isFree;
}


[Serializable]
public class L_PrivateRoomJoinData
{
    public string userId;
    public string roomId;
    public string players;
    public string playerType;

    public string isPrivate;
    public string isFree;
    public string tableId;
}

[Serializable]
public enum L_SocketState
{
    Connecting,
    Connected,
    WaitingForOpponent,
    Game_Running,
    //InitializingCards,
    ReConnecting,
    NULL
}
