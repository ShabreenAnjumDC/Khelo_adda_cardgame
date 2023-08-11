using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP.SocketIO;
using System.Collections;
using BestHTTP.JSON;
using LitJson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class T_SocketController : MonoBehaviour
{
    public static T_SocketController instance;

    private const float RESPONSE_READ_DELAY = 0.2f, REQUEST_SEND_DELAY = 0.1f;
    private SocketManager socketManager;

    private List<T_SocketResponse> T_SocketResponse = new List<T_SocketResponse>();
    private List<T_SocketRequest> T_SocketRequest = new List<T_SocketRequest>();

    [SerializeField]
    private T_SocketState T_SocketState;

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
    public int winAmount;

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
    bool isRemovePlayerOnReceived = false;
    [HideInInspector]
    public bool needToFetchLobby = false;
    int turnValue = 0;
    //public List<Token> AllTokens = new List<Token>();
    public JsonData ReturnToStartPoint = null;
    [HideInInspector]
    //public BoardHighlighter lightHighLighter;

    Coroutine applicationQuitCo;
    bool isClose = false;

    //public bool isFromTournament = false;
    public string tourneyId;
    public string registartionId;

    public TournamentSelected tournamentSelected;

    void Awake()
    {
        Debug.Log("L socket Awake()");
        instance = this;
        //if (Application.isEditor)
        Application.runInBackground = true;
        SetSocketState(T_SocketState.NULL);
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
        //if (T_InGameUiManager.instance != null && isStartTime)
        //    StartTimer();
    }


    void StartTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int m = Mathf.FloorToInt(timeRemaining / 60.0f);
            int s = Mathf.FloorToInt(timeRemaining - m * 60f);
            //T_InGameUiManager.instance.timerText.text = string.Format("{0:00}:{1:00}", m, s);
        }
        else
        {
            //T_InGameUiManager.instance.timerText.text = string.Format("{0:00}:{1:00}", 0, 0);
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

//#if UNITY_ANDROID && !UNITY_EDITOR
//        void OnApplicationFocus(bool focus)
//        {
//            if (focus)
//            {
//                //ResumeApplication();
//                Debug.Log("111111 FOCUS: TRUE");
//                if (applicationQuitCo != null)
//                {
//                    Debug.Log("111111 FOCUS: TRUE StopCoroutine WaitAfterFocusLost()");
//                    //StopCoroutine(applicationQuitCo);
//                    //SendRejoin();
//                }
//            }
//            else
//            {
//                //LeaveApplication();
//                Debug.Log("111111 FOCUS: FALSE APP QUIT");
//                //applicationQuitCo = StartCoroutine(WaitAfterFocusLost());
//            }
//        }
//#endif

    //#if UNITY_EDITOR || UNITY_IOS
    //    void OnApplicationPause(bool pause)
    //    {
    //        if (pause)
    //        {
    //            //LeaveApplication();
    //            Debug.Log("111111 PAUSE: TRUE");

    //            //RemovePlayerFromGame();
    //            applicationQuitCo = StartCoroutine(WaitAfterFocusLost());
    //        }
    //        else
    //        {
    //            //ResumeApplication();
    //            Debug.Log("111111 PAUSE: FALSE");
    //            if (applicationQuitCo != null)
    //            {
    //                Debug.Log("111111 PAUSE: FALSE StopCoroutine WaitAfterFocusLost()");
    //                StopCoroutine(applicationQuitCo);
    //                SendRejoin();
    //            }
    //        }
    //    }
    //#endif

    IEnumerator WaitAfterFocusLost()
    {
        Debug.Log("WaitAfterFocusLost() ENTER WaitForSecondsRealtime");
        yield return new WaitForSecondsRealtime(60f);
        Debug.Log("WaitAfterFocusLost() after 60 seconds WaitForSecondsRealtime");
        SendRemovePlayer();
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
        SetSocketState(T_SocketState.Connecting);


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

        socketManager = new SocketManager(new Uri(L_GameConstant.TOURNAMENT_SOCKET_URL + "/socket.io/"), socketOptions);

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

        //if (isFromTournament)
        //{
            socketManager.Socket.On("tournamentStatus", OnTournamentStatus);
            socketManager.Socket.On("userJoined", OnUserJoined);
            socketManager.Socket.On("tourneyList", OnTourneyList);
        //}

        socketManager.Open();
    }

    private void HandleSocketResponse()
    {
        if (T_SocketResponse.Count > 0)
        {
            T_SocketResponse responseObject = T_SocketResponse[0];
            T_SocketResponse.RemoveAt(0);

//#if DEBUG

//#if UNITY_EDITOR

//#else
//            Debug.LogError("Handling ServerResponse = " + responseObject.eventType + "  T_SocketState = " + T_SocketState + "   data = " + responseObject.data);
//#endif

//#endif
            Debug.Log(gameObject.transform.parent.name + " <color=yellow> " + responseObject.eventType + ",</color> " + responseObject.data);
            switch (responseObject.eventType)
            {
                case T_SocketEvetns.CONNECT:
                    {
                        //SendPlayerDetails();

                        switch (GetSocketState())
                        {
                            case T_SocketState.Connecting:
                                //SetSocketState(T_SocketState.WaitingForOpponent);
                                //Debug.Log(responseObject.eventType + "<color=yellow> IsJoiningPreviousGame " + T_GlobalGameManager.IsJoiningPreviousGame + "</color>");
                                //CreateRoom();
                                SetSocketState(T_SocketState.Connected);
                                break;

                            case T_SocketState.ReConnecting:
                                //SendRejoin();
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case T_SocketEvetns.DISCONNECT:
                    StartReconnectProcedure();
                    break;


                case T_SocketEvetns.RECONNECT_ATTEMPT:
                    break;

                default:
                    Debug.LogError("UnHandlled EventType Found in response eventType = " + responseObject.eventType + "   responseStructure = " + responseObject.data);
                    break;
            }

        }
    }


    private void SendSocketRequest()
    {
        if (T_SocketRequest.Count > 0)
        {
            T_SocketRequest request = T_SocketRequest[0];
            T_SocketRequest.RemoveAt(0);

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
        if (TournamentPlayerFinding.instance != null)
        {
            TournamentPlayerFinding.instance.OnGameStart(gameId);
        }
        //if (isFromTournament)
        //{
            isGameObjectFirstTime = 0;
            if (L_MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentGamePlay))
            {
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentGamePlay);
            }
        //}
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

        //if (QuickLudoSelection.instance != null)
        //    StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(QuickLudoSelection.instance.errorTMP, "Error in connecting game server", "red", 2f));
        //else 
        if (T_Panel_Controller.instance !=null && responseText == "INVALID")
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(T_Panel_Controller.instance.selfPanelMsgText, "Invalid Move", "red", 2f));

        //if (isFromTournament)
        //{
            if (Tournaments.instance != null)
            {
                Tournaments.instance.GameErrorReturn(responseText);
            }
            else if (TournamentJoin.instance != null)
            {
                TournamentJoin.instance.GameErrorReturn(responseText);
            }
        //}
    }

    private void OnGameTimeLeft(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        //Debug.Log("Socket => <color=yellow>GAME_TIME_LEFT</color>: " + responseText);
        if (T_Panel_Controller.instance != null)
        {
            string trimedStr = responseText.Remove(0, 2); //["10:00"]   10:00"]
            trimedStr = trimedStr.Remove((trimedStr.Length - 2), 2); //10:00"]   10:00
            T_Panel_Controller.instance.gameTimerText.text = trimedStr;
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
        if (T_Panel_Controller.instance != null)
        {
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(T_Panel_Controller.instance.selfPanelMsgText, "Invalid Move", "red", 2f));

            if (L_SoundManager.instance.isSound)
                T_GameManager.instance.invalidMoveAudio.Play();
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
        isRemovePlayerOnReceived = true;
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>removePlayer</color>: " + responseText);

        //if (T_Panel_Controller.instance != null)
        //{
        //    T_Panel_Controller.instance.hidePanelWhenBack.SetActive(false);
        //    T_Panel_Controller.instance.backButton.interactable = true;
        //}

        //SocketClose();


        //if (isClassicLudo)
        //{
        //    QuickLudoSelection.instance.isClassicLudo = true;
        //    QuickLudoSelection.instance.isQuickLudo = false;
        //}
        //else
        //{
        //    QuickLudoSelection.instance.isQuickLudo = true;
        //    QuickLudoSelection.instance.isClassicLudo = false;
        //}
        //QuickLudoSelection.instance.gameTypeId = gameTypeId;

        //if (MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentPlayerFinding) || MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentGamePlay)
        //    || MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentWinning))
        //{
        //    MainMenuController.instance.ShowScreen(MainMenuScreens.Tournaments);
        //}
    }


    private void PlayerAdded(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>playerAdded</color>: " + responseText);
        JsonData data = JsonMapper.ToObject(responseText);
        Debug.Log("Socket => PlayerAdded: " + data[0]["playerId"].ToString());
        gamePlayerId = data[0]["playerId"].ToString();

        //if (isFromTournament)
        //{
            if (Tournaments.instance != null)
            {
                tourneyId = data[0]["tourneyId"].ToString();
                registartionId = data[0]["registartionId"].ToString();

                if (string.IsNullOrEmpty(registartionId))
                    Tournaments.instance.PlayerAddedReturn("Some problem in Registration");
                else
                {
                    Tournaments.instance.isRegisterResponseSuccess = true;
                    Tournaments.instance.PlayerAddedReturn("Registration Success");
                }
            }
        //}
        //else
        //{
        //    gamePlayerToken = data[0]["token"].ToString();
        //    if (QuickLudoSelection.instance != null)
        //    {
        //        QuickLudoSelection.instance.OnPlayerAddedReceived();
        //    }
        //}
    }


    private void OnTournamentStatus(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>tournamentStatus</color>: " + responseText);
        //[{ "id":1.0,"status":1.0}]  //[{"id":1.0,"status":2.0,"winnerId":15.0}]

        if (L_MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentNextRound))
            Debug.Log("TournamentNextRound screen active ");
        else
            Debug.Log("TournamentNextRound screen Not active ");

        if (TournamentNextRound.instance == null)
        {
            Debug.Log("TournamentNextRound is null");
        }
        if (TournamentNextRound.instance != null)
        {
            Debug.Log("TournamentNextRound is NOT null");
            JsonData data = JsonMapper.ToObject(responseText);
            IDictionary iData = data[0] as IDictionary;
            if (iData.Contains("winnerId"))
            {
                Debug.Log("winnerId equal userid: " + data[0]["winnerId"].ToString().Equals(PlayerManager.instance.GetPlayerGameData().userId));
                Debug.Log("tournamentid is localtourneyid: " + data[0]["id"].ToString().Equals(tournamentSelected.id));
                if (data[0]["winnerId"].ToString().Equals(PlayerManager.instance.GetPlayerGameData().userId) && data[0]["id"].ToString().Equals(tournamentSelected.id))
                {
                    Debug.Log("winnerId-userid & tournamentid-localtourneyid");
                    L_MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentWinning);
                }
                //else
                //{
                //    Debug.Log("inside second else");
                //    if (TournamentNextRound.instance == null)
                //    {
                //        MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentNextRound);
                //    }
                //    TournamentNextRound.instance.SetLoseData(tournamentSelected);
                //}
            }
        }
    }

    private void OnUserJoined(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>OnUserJoined</color>: " + responseText);
        JsonData data = JsonMapper.ToObject(responseText);
        gamePlayerId = data[0]["playerId"].ToString();
        gamePlayerToken = data[0]["token"].ToString();
        //if (QuickLudoSelection.instance != null)
        //{
        //    QuickLudoSelection.instance.OnPlayerAddedReceived();
        //}
        //else 
        if (TournamentJoin.instance != null)
        {
            TournamentJoin.instance.OnPlayerAddedReceived();
        }
    }

    private void OnTourneyList(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>OnTourneyList</color>: " + responseText);
        if (Tournaments.instance != null)
            Tournaments.instance.OnTournamentListStatusChange(responseText);
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

        if (T_GameManager.instance.manageRollingDice[0].gameObject.activeInHierarchy) //red
        {
            StartCoroutine(T_GameManager.instance.manageRollingDice[0].DiceTime(diceValueInt)); //Int32.Parse(data[0].ToString())
        }
        else if (T_GameManager.instance.manageRollingDice[1].gameObject.activeInHierarchy) //yellow
        {
            StartCoroutine(T_GameManager.instance.manageRollingDice[1].DiceTime(diceValueInt)); //Int32.Parse(data[0].ToString())
        }
        else if (T_GameManager.instance.manageRollingDice[2].gameObject.activeInHierarchy) //green
        {
            StartCoroutine(T_GameManager.instance.manageRollingDice[2].DiceTime(diceValueInt)); //Int32.Parse(data[0].ToString())
        }
        else if (T_GameManager.instance.manageRollingDice[3].gameObject.activeInHierarchy) //blue
        {
            StartCoroutine(T_GameManager.instance.manageRollingDice[3].DiceTime(diceValueInt, (int diceValueIntOfAction) => {
               
            })); 
        }

    }


    private void GameObjectData(Socket socket, Packet packet, object[] args)
    {
        totalPlayersInGame = 0;
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("Socket => <color=yellow>gameObject</color>: " + responseText);
        JsonData data = JsonMapper.ToObject(responseText);

        bool isFirstTime = false;
        Debug.Log("isGameObjectFirstTime: "+ isGameObjectFirstTime);
        if (isGameObjectFirstTime == 0)
        {
            Debug.Log("isGameObjectFirstTime IF");
            isFirstTime = true;
            Debug.Log("isFirstTime:" + isFirstTime);
            isGameObjectFirstTime = 1;

            if (L_MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentNextRound))
                L_MainMenuController.instance.DestroyScreen(MainMenuScreens.TournamentNextRound);
            else if (L_MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentPlayerFinding))
                L_MainMenuController.instance.DestroyScreen(MainMenuScreens.TournamentPlayerFinding);

            L_MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentGamePlay);

            gameId = data[0]["gameId"].ToString();  //if (string.IsNullOrEmpty(gameId)) { gameId = data[0]["gameId"].ToString(); }
            T_Panel_Controller.instance.roomCodeText.text = gameId;

            if (L_SoundManager.instance.isSound)
                L_SoundManager.instance.PlayLoopSound(L_SoundType.BoardGamePlaySound, transform);

            if (isClassicLudo)
            {
                T_OptionPanel_Controller.instance.OnClickClassicLudo();
                T_Panel_Controller.instance.gameTypeText.text = "CLASSIC";
            }
            else
            {
                T_OptionPanel_Controller.instance.OnClickQuickLudo();
                T_Panel_Controller.instance.gameTypeText.text = "QUICK";
            }


            int amountInt = 0;
            if (int.TryParse(gameAmount, out amountInt))
            {
                winAmount = amountInt;
                T_Panel_Controller.instance.winAmountTMP.text = "₹" + winAmount.ToString();
            }

            try
            {
                IDictionary iData = data[0] as IDictionary;
                if (iData.Contains("counter"))
                {
                    gameTime = data[0]["counter"].ToString();

                    T_Panel_Controller.instance.IdleGameTimerFunc();
                }
            }
            catch (Exception e)
            {
                Debug.Log("counter catch: " + e.Message);
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
        //    T_Panel_Controller.instance.selfTurnSkipBtn.interactable = true;
        //else
        //    T_Panel_Controller.instance.selfTurnSkipBtn.interactable = false;

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
            //if (isFromTournament)
                StartCoroutine(ShowWinLoseScreenTournament(data[0]["winners"]));
            //else
            //    StartCoroutine(ShowWinLoseScreen(data[0]["winners"]));
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
                        T_Panel_Controller.instance.allUsersPanel[IndexSwapForTableRotation(myColor, tempIwinner)].SetActive(false);

                        if (data[0]["players"][i]["color"].ToString() == "red")
                        {
                            T_Panel_Controller.instance.HidePlayers(T_GameManager.instance.redPlayers);

                            for (int ipl = 0; ipl < T_GameManager.instance.redPlayers.Length; ipl++)
                            {
                                T_GameManager.instance.redPlayers[ipl].gameObject.transform.parent = T_WayPointPathPrent.instance.homePoints[ipl].transform;
                                T_GameManager.instance.redPlayers[ipl].gameObject.transform.position = T_WayPointPathPrent.instance.homePoints[ipl].transform.position;
                            }
                        }
                        else if (data[0]["players"][i]["color"].ToString() == "yellow")
                        {
                            T_Panel_Controller.instance.HidePlayers(T_GameManager.instance.yellowPlayers);

                            for (int ipl = 0; ipl < T_GameManager.instance.yellowPlayers.Length; ipl++)
                            {
                                int iplHome = (ipl + 4);
                                T_GameManager.instance.yellowPlayers[ipl].gameObject.transform.parent = T_WayPointPathPrent.instance.homePoints[iplHome].transform;
                                T_GameManager.instance.yellowPlayers[ipl].gameObject.transform.position = T_WayPointPathPrent.instance.homePoints[iplHome].transform.position;
                            }
                        }
                        else if (data[0]["players"][i]["color"].ToString() == "green")
                        {
                            T_Panel_Controller.instance.HidePlayers(T_GameManager.instance.greenPlayers);

                            for (int ipl = 0; ipl < T_GameManager.instance.greenPlayers.Length; ipl++)
                            {
                                int iplHome = (ipl + 8);
                                T_GameManager.instance.greenPlayers[ipl].gameObject.transform.parent = T_WayPointPathPrent.instance.homePoints[iplHome].transform;
                                T_GameManager.instance.greenPlayers[ipl].gameObject.transform.position = T_WayPointPathPrent.instance.homePoints[iplHome].transform.position;
                            }
                        }
                        else if (data[0]["players"][i]["color"].ToString() == "blue")
                        {
                            T_Panel_Controller.instance.HidePlayers(T_GameManager.instance.bluePlayers);

                            for (int ipl = 0; ipl < T_GameManager.instance.bluePlayers.Length; ipl++)
                            {
                                int iplHome = (ipl + 12);
                                T_GameManager.instance.bluePlayers[ipl].gameObject.transform.parent = T_WayPointPathPrent.instance.homePoints[iplHome].transform;
                                T_GameManager.instance.bluePlayers[ipl].gameObject.transform.position = T_WayPointPathPrent.instance.homePoints[iplHome].transform.position;
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
                                    T_Panel_Controller.instance.ShowPlayers(T_GameManager.instance.redPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        T_Panel_Controller.instance.tempInfoTMP.text = "Your color is Red";
                                        myColor = "red";
                                        myPlayerIndex = 0;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "4")
                                {
                                    // yellow user panel on, name dispaly
                                    T_Panel_Controller.instance.ShowPlayers(T_GameManager.instance.yellowPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        T_Panel_Controller.instance.tempInfoTMP.text = "Your color is yellow";
                                        myColor = "yellow";
                                        myPlayerIndex = 1;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "8")
                                {
                                    // green user panel on, name dispaly
                                    T_Panel_Controller.instance.ShowPlayers(T_GameManager.instance.greenPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        T_Panel_Controller.instance.tempInfoTMP.text = "Your color is green";
                                        myColor = "green";
                                        myPlayerIndex = 2;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "12")
                                {
                                    // blue user panel on, name dispaly
                                    T_Panel_Controller.instance.ShowPlayers(T_GameManager.instance.bluePlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        T_Panel_Controller.instance.tempInfoTMP.text = "Your color is blue";
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
                                    T_Panel_Controller.instance.ShowPlayers(T_GameManager.instance.redPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        T_Panel_Controller.instance.tempInfoTMP.text = "Your color is Red";
                                        myColor = "red";
                                        myPlayerIndex = 0;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "29")
                                {
                                    T_Panel_Controller.instance.ShowPlayers(T_GameManager.instance.yellowPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        T_Panel_Controller.instance.tempInfoTMP.text = "Your color is yellow";
                                        myColor = "yellow";
                                        myPlayerIndex = 1;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "42")
                                {
                                    T_Panel_Controller.instance.ShowPlayers(T_GameManager.instance.greenPlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        T_Panel_Controller.instance.tempInfoTMP.text = "Your color is green";
                                        myColor = "green";
                                        myPlayerIndex = 2;
                                    }
                                }
                                else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "55")
                                {
                                    T_Panel_Controller.instance.ShowPlayers(T_GameManager.instance.bluePlayers);

                                    if (data[0]["players"][i]["playerId"].ToString() == gamePlayerId)
                                    {
                                        T_Panel_Controller.instance.tempInfoTMP.text = "Your color is blue";
                                        myColor = "blue";
                                        myPlayerIndex = 3;
                                    }
                                }
                            }

                            if (playerColor == "red")
                                T_GameManager.instance.redPlayers[j].GetComponent<T_RedPlayer>().currentWayPointInt = chipsPosInt;
                            else if (playerColor == "yellow")
                                T_GameManager.instance.yellowPlayers[j].GetComponent<T_YellowPlayer>().currentWayPointInt = chipsPosInt;
                            else if (playerColor == "green")
                                T_GameManager.instance.greenPlayers[j].GetComponent<T_GreenPlayer>().currentWayPointInt = chipsPosInt;
                            else if (playerColor == "blue")
                                T_GameManager.instance.bluePlayers[j].GetComponent<T_BluePlayer>().currentWayPointInt = chipsPosInt;

                        }
                        else
                        {
                            if (playerColor == "red")
                            {
                                T_GameManager.instance.redPlayers[j].GetComponent<T_RedPlayer>().previousWayPointInt = T_GameManager.instance.redPlayers[j].GetComponent<T_RedPlayer>().currentWayPointInt;
                                T_GameManager.instance.redPlayers[j].GetComponent<T_RedPlayer>().currentWayPointInt = chipsPosInt;
                                T_GameManager.instance.redPlayers[j].GetComponent<T_RedPlayer>().distance = distanceInt;

                                if (isClassicLudo)
                                    StartCoroutine(T_GameManager.instance.redPlayers[j].GetComponent<T_RedPlayer>().MoveStepsNew(chipsPosInt, T_WayPointPathPrent.instance.redWayPointPath));
                                else if (isQuickLudo == true)
                                {
                                    
                                    StartCoroutine(T_GameManager.instance.redPlayers[j].GetComponent<T_RedPlayer>().MoveStepsNewQuickLudo(chipsPosInt, distanceInt, knockoutsInt, chipsLostInt, playerColor, T_WayPointPathPrent.instance.redWayPointPath));
                                }
                            }
                            else if (playerColor == "yellow")
                            {
                                T_GameManager.instance.yellowPlayers[j].GetComponent<T_YellowPlayer>().previousWayPointInt = T_GameManager.instance.yellowPlayers[j].GetComponent<T_YellowPlayer>().currentWayPointInt;
                                T_GameManager.instance.yellowPlayers[j].GetComponent<T_YellowPlayer>().currentWayPointInt = chipsPosInt;
                                T_GameManager.instance.yellowPlayers[j].GetComponent<T_YellowPlayer>().distance = distanceInt;

                                if (isClassicLudo)
                                    StartCoroutine(T_GameManager.instance.yellowPlayers[j].GetComponent<T_YellowPlayer>().MoveStepsNew(chipsPosInt, T_WayPointPathPrent.instance.yellowWayPointPath));
                                else if (isQuickLudo == true)
                                {
                                    
                                    StartCoroutine(T_GameManager.instance.yellowPlayers[j].GetComponent<T_YellowPlayer>().MoveStepsNewQuickLudo(chipsPosInt, distanceInt, knockoutsInt, chipsLostInt, playerColor, T_WayPointPathPrent.instance.yellowWayPointPath));
                                }
                            }
                            else if (playerColor == "green")
                            {
                                T_GameManager.instance.greenPlayers[j].GetComponent<T_GreenPlayer>().previousWayPointInt = T_GameManager.instance.greenPlayers[j].GetComponent<T_GreenPlayer>().currentWayPointInt;
                                T_GameManager.instance.greenPlayers[j].GetComponent<T_GreenPlayer>().currentWayPointInt = chipsPosInt;
                                T_GameManager.instance.greenPlayers[j].GetComponent<T_GreenPlayer>().distance = distanceInt;

                                if (isClassicLudo)
                                    StartCoroutine(T_GameManager.instance.greenPlayers[j].GetComponent<T_GreenPlayer>().MoveStepsNew(chipsPosInt, T_WayPointPathPrent.instance.greenWayPointPath));
                                else if (isQuickLudo == true)
                                {
                                    
                                    StartCoroutine(T_GameManager.instance.greenPlayers[j].GetComponent<T_GreenPlayer>().MoveStepsNewQuickLudo(chipsPosInt, distanceInt, knockoutsInt, chipsLostInt, playerColor, T_WayPointPathPrent.instance.greenWayPointPath));
                                }
                            }
                            else if (playerColor == "blue")
                            {
                                T_GameManager.instance.bluePlayers[j].GetComponent<T_BluePlayer>().previousWayPointInt = T_GameManager.instance.bluePlayers[j].GetComponent<T_BluePlayer>().currentWayPointInt;
                                T_GameManager.instance.bluePlayers[j].GetComponent<T_BluePlayer>().currentWayPointInt = chipsPosInt;
                                T_GameManager.instance.bluePlayers[j].GetComponent<T_BluePlayer>().distance = distanceInt;

                                if (isClassicLudo)
                                    StartCoroutine(T_GameManager.instance.bluePlayers[j].GetComponent<T_BluePlayer>().MoveStepsNew(chipsPosInt, T_WayPointPathPrent.instance.blueWayPointPath));
                                else if (isQuickLudo == true)
                                {
                                    
                                    StartCoroutine(T_GameManager.instance.bluePlayers[j].GetComponent<T_BluePlayer>().MoveStepsNewQuickLudo(chipsPosInt, distanceInt, knockoutsInt, chipsLostInt, playerColor, T_WayPointPathPrent.instance.blueWayPointPath));
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
                                    T_Panel_Controller.instance.blueUserDiceWhen6[iPrCnt].sprite = T_DiceManager.instance.Dice[tempDice];
                                    T_Panel_Controller.instance.blueUserDiceWhen6[iPrCnt].gameObject.SetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        // for self turn skip
        if (playerTurnIndex == myPlayerIndex)
            T_Panel_Controller.instance.selfTurnSkipBtn.interactable = true;
        else
            T_Panel_Controller.instance.selfTurnSkipBtn.interactable = false;

        if (playerTurnIndex != myPlayerIndex)
        {
            if (myColor == "red")
                T_GameManager.instance.HideOptionsRedPlayers();
            else if (myColor == "green")
                T_GameManager.instance.HideOptionsGreenPlayers();
            else if (myColor == "yellow")
                T_GameManager.instance.HideOptionsYellowPlayers();
            else if (myColor == "blue")
                T_GameManager.instance.HideOptionsBluePlayers();
        }

        if (isFirstTime)
        {
            T_Panel_Controller.instance.RotateBoard(selectedGotiColor, myColor, myPlayerIndex);

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
                                for (int h = 0; h < T_WayPointPathPrent.instance.wayPointsParent.childCount; h++)
                                {
                                    if (T_WayPointPathPrent.instance.wayPointsParent.GetChild(h).name == "Spot " + data[0]["players"][i]["chips"][0]["pos"].ToString())
                                    {
                                        T_GameManager.instance.redPlayers[j].transform.position = T_WayPointPathPrent.instance.wayPointsParent.GetChild(h).position;
                                        T_GameManager.instance.redPlayers[j].transform.parent = T_WayPointPathPrent.instance.wayPointsParent.GetChild(h);
                                        T_GameManager.instance.redPlayers[j].transform.localPosition = Vector3.zero;
                                        T_GameManager.instance.redPlayers[j].ReScaleAndRepositionAllPlayerPiece(T_WayPointPathPrent.instance.wayPointsParent.GetChild(h));
                                    }
                                }
                            }
                            else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "29")
                            {
                                for (int h = 0; h < T_WayPointPathPrent.instance.wayPointsParent.childCount; h++)
                                {
                                    if (T_WayPointPathPrent.instance.wayPointsParent.GetChild(h).name == "Spot " + data[0]["players"][i]["chips"][0]["pos"].ToString())
                                    {
                                        T_GameManager.instance.yellowPlayers[j].transform.position = T_WayPointPathPrent.instance.wayPointsParent.GetChild(h).position;
                                        T_GameManager.instance.yellowPlayers[j].transform.parent = T_WayPointPathPrent.instance.wayPointsParent.GetChild(h);
                                        T_GameManager.instance.yellowPlayers[j].transform.localPosition = Vector3.zero;
                                        T_GameManager.instance.yellowPlayers[j].ReScaleAndRepositionAllPlayerPiece(T_WayPointPathPrent.instance.wayPointsParent.GetChild(h));
                                    }
                                }
                            }
                            else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "42")
                            {
                                for (int h = 0; h < T_WayPointPathPrent.instance.wayPointsParent.childCount; h++)
                                {
                                    if (T_WayPointPathPrent.instance.wayPointsParent.GetChild(h).name == "Spot " + data[0]["players"][i]["chips"][0]["pos"].ToString())
                                    {
                                        T_GameManager.instance.greenPlayers[j].transform.position = T_WayPointPathPrent.instance.wayPointsParent.GetChild(h).position;
                                        T_GameManager.instance.greenPlayers[j].transform.parent = T_WayPointPathPrent.instance.wayPointsParent.GetChild(h);
                                        T_GameManager.instance.greenPlayers[j].transform.localPosition = Vector3.zero;
                                        T_GameManager.instance.greenPlayers[j].ReScaleAndRepositionAllPlayerPiece(T_WayPointPathPrent.instance.wayPointsParent.GetChild(h));
                                    }
                                }
                            }
                            else if (data[0]["players"][i]["chips"][0]["pos"].ToString() == "55")
                            {
                                for (int h = 0; h < T_WayPointPathPrent.instance.wayPointsParent.childCount; h++)
                                {
                                    if (T_WayPointPathPrent.instance.wayPointsParent.GetChild(h).name == "Spot " + data[0]["players"][i]["chips"][0]["pos"].ToString())
                                    {
                                        T_GameManager.instance.bluePlayers[j].transform.position = T_WayPointPathPrent.instance.wayPointsParent.GetChild(h).position;
                                        T_GameManager.instance.bluePlayers[j].transform.parent = T_WayPointPathPrent.instance.wayPointsParent.GetChild(h);
                                        T_GameManager.instance.bluePlayers[j].transform.localPosition = Vector3.zero;
                                        T_GameManager.instance.bluePlayers[j].ReScaleAndRepositionAllPlayerPiece(T_WayPointPathPrent.instance.wayPointsParent.GetChild(h));
                                    }
                                }
                            }
                        }
                    }

                    if (data[0]["players"][i]["playerId"].ToString() == PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        T_Panel_Controller.instance.allUsersPanel[3].SetActive(true);
                        T_Panel_Controller.instance.allPlayersName[3].text = data[0]["players"][i]["playerName"].ToString();
                    }

                    // default red
                    //0 => panel 3
                    //1 => panel 0
                    //2 => panel 1
                    //3 => panel 2
                    if (myColor == "red" && (i > 0))
                    {
                        int k = (i - 1);
                        T_Panel_Controller.instance.allUsersPanel[k].SetActive(true);
                        T_Panel_Controller.instance.allPlayersName[k].text = data[0]["players"][i]["playerName"].ToString();
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
                            T_Panel_Controller.instance.allUsersPanel[1].SetActive(true);
                            T_Panel_Controller.instance.allPlayersName[1].text = data[0]["players"][i]["playerName"].ToString();
                        }
                        else if (i == 1)
                        {
                            T_Panel_Controller.instance.allUsersPanel[2].SetActive(true);
                            T_Panel_Controller.instance.allPlayersName[2].text = data[0]["players"][i]["playerName"].ToString();
                        }
                        else
                        {
                            T_Panel_Controller.instance.allUsersPanel[0].SetActive(true);
                            T_Panel_Controller.instance.allPlayersName[0].text = data[0]["players"][i]["playerName"].ToString();
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
                            T_Panel_Controller.instance.allUsersPanel[2].SetActive(true);
                            T_Panel_Controller.instance.allPlayersName[2].text = data[0]["players"][i]["playerName"].ToString();
                        }
                        else if (i == 2)
                        {
                            T_Panel_Controller.instance.allUsersPanel[0].SetActive(true);
                            T_Panel_Controller.instance.allPlayersName[0].text = data[0]["players"][i]["playerName"].ToString();
                        }
                        else
                        {
                            T_Panel_Controller.instance.allUsersPanel[1].SetActive(true);
                            T_Panel_Controller.instance.allPlayersName[1].text = data[0]["players"][i]["playerName"].ToString();
                        }
                    }

                    // default blue
                    //0 => panel 0
                    //1 => panel 1
                    //2 => panel 2
                    //3 => panel 3
                    if (myColor == "blue" && (i != 3))
                    {
                        T_Panel_Controller.instance.allUsersPanel[i].SetActive(true);
                        T_Panel_Controller.instance.allPlayersName[i].text = data[0]["players"][i]["playerName"].ToString();
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
                //T_Panel_Controller.instance.scoreText.text = "Score : " + scoreTotalDistance;

                if (data[0]["players"][i]["playerId"].ToString() == PlayerManager.instance.GetPlayerGameData().userId)
                {
                    T_Panel_Controller.instance.scoreTextArr[3].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                }

                if (myColor == "red" && (i > 0))
                {
                    int k = (i - 1);
                    T_Panel_Controller.instance.scoreTextArr[k].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                }

                if (myColor == "green" && (i != 2))
                {
                    if (i == 0)
                    {
                        T_Panel_Controller.instance.scoreTextArr[1].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                    else if (i == 1)
                    {
                        T_Panel_Controller.instance.scoreTextArr[2].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                    else
                    {
                        T_Panel_Controller.instance.scoreTextArr[0].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                }

                if (myColor == "yellow" && (i != 1))
                {
                    if (i == 0)
                    {
                        T_Panel_Controller.instance.scoreTextArr[2].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                    else if (i == 2)
                    {
                        T_Panel_Controller.instance.scoreTextArr[0].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                    else
                    {
                        T_Panel_Controller.instance.scoreTextArr[1].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
                    }
                }

                if (myColor == "blue" && (i != 3))
                {
                    T_Panel_Controller.instance.scoreTextArr[i].text = data[0]["players"][i]["stats"]["totalDistance"].ToString();
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
                    T_Panel_Controller.instance.ShowDice(swapIndex);

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
                        T_Panel_Controller.instance.IdleTimerFunc(swapIndex);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("timeout catch: " + e.Message);
        }

        if (waitingForMove == true)
            T_Panel_Controller.instance.HighlightKukriAnimation(playerTurnIndex);

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

                //Debug.Log("thisPlayerSwapIndex="+ thisPlayerSwapIndex, T_Panel_Controller.instance.idleTimeDots.gParent[thisPlayerSwapIndex].gChild[0].gameObject);

                switch (turnIdle)
                {
                    case 0:
                        for (int j = 0; j < T_Panel_Controller.instance.idleTimeDots.gParent[thisPlayerSwapIndex].gChild.Count; j++)
                        {
                            T_Panel_Controller.instance.idleTimeDots.gParent[thisPlayerSwapIndex].gChild[j].SetActive(true);
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                        T_Panel_Controller.instance.idleTimeDots.gParent[thisPlayerSwapIndex].gChild[(turnIdle - 1)].SetActive(false);
                        break;
                }
            }
        }
    }

    void HideDice6OptionsSelfPanel()
    {
        T_Panel_Controller.instance.blueUserDiceWhen6[0].gameObject.SetActive(false);
        T_Panel_Controller.instance.blueUserDiceWhen6[1].gameObject.SetActive(false);
        T_Panel_Controller.instance.blueUserDiceWhen6[2].gameObject.SetActive(false);

        btnClickRecordWhenDiceValue6[0] = 0;
        btnClickRecordWhenDiceValue6[1] = 0;
        btnClickRecordWhenDiceValue6[2] = 0;
    }

    //IEnumerator ShowWinLoseScreen(JsonData data)
    //{
    //    yield return new WaitForSeconds(2f);
    //    MainMenuController.instance.ShowScreen(MainMenuScreens.Winner);

    //    if (SoundManager.instance.isSound)
    //        SoundManager.instance.StopLoopSound(SoundType.BoardGamePlaySound, transform);

    //    if (data[0].ToString() == myPlayerIndex.ToString())
    //    {
    //        // self winner
    //        if (WinnerLooser.instance != null)
    //        {
    //            WinnerLooser.instance.SelfWinner(true, gameAmount);

    //            if (SoundManager.instance.isSound)
    //                SoundManager.instance.PlaySound(SoundType.WonSound, transform);
    //        }
    //    }
    //    else
    //    {
    //        // self lose
    //        if (WinnerLooser.instance != null)
    //        {
    //            WinnerLooser.instance.SelfWinner(false, gameAmount);

    //            if (SoundManager.instance.isSound)
    //                SoundManager.instance.PlaySound(SoundType.LoseSound, transform);
    //        }
    //    }
    //}

    IEnumerator ShowWinLoseScreenTournament(JsonData data)
    {
        yield return new WaitForSeconds(0.2f);
        isGameObjectFirstTime = 0;
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentNextRound);

        //if (isFromTournament)
        //{
        //    if (TournamentNextRound.instance != null)
        //    {
        //if (MainMenuController.instance.IsScreenActive(MainMenuScreens.ClassicLudoGamePlay))
        //{
        //    MainMenuController.instance.ShowScreen(MainMenuScreens.ClassicLudoGamePlay);
        //}
        //    }
        //}

        if (L_SoundManager.instance.isSound)
            L_SoundManager.instance.StopLoopSound(L_SoundType.BoardGamePlaySound, transform);

        if (TournamentNextRound.instance != null)
        {
            if (data[0].ToString() == myPlayerIndex.ToString())
            {
                // self winner
                TournamentNextRound.instance.SetWinData(tournamentSelected);

                //ClearVariable(); //opponent panel not showing after clear variable here

                if (L_SoundManager.instance.isSound)
                    L_SoundManager.instance.PlaySound(L_SoundType.WonSound, transform);
            }
            else
            {
                // self lose
                TournamentNextRound.instance.SetLoseData(tournamentSelected);

                if (L_SoundManager.instance.isSound)
                    L_SoundManager.instance.PlaySound(L_SoundType.LoseSound, transform);
            }
        }

    }





    void OnServerConnect(Socket socket, Packet packet, params object[] args)
    {
        //if (T_GlobalGameManager.instance.CanDebugThis(T_SocketEvetns.CONNECT))
        //{
        Debug.Log("Enter in OnServerConnect Time = " + System.DateTime.Now);
        //}
        T_SocketResponse response = new T_SocketResponse();
        response.eventType = T_SocketEvetns.CONNECT;
        T_SocketResponse.Add(response);
    }


    void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
    {
        //if (T_GlobalGameManager.instance.CanDebugThis(T_SocketEvetns.DISCONNECT))
        //{

        //}
        T_SocketResponse response = new T_SocketResponse();
        response.eventType = T_SocketEvetns.DISCONNECT;
        T_SocketResponse.Add(response);
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
        //if (T_GlobalGameManager.instance.CanDebugThis(T_SocketEvetns.RECONNECT_ATTEMPT))
        //{
        Debug.Log("Enter in OnReconnectAttempt Time = " + System.DateTime.Now);
        //}

        T_SocketResponse response = new T_SocketResponse();
        response.eventType = T_SocketEvetns.RECONNECT_ATTEMPT;
        T_SocketResponse.Add(response);
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
        string requestStringData = "{\"gameId\":\"" + gameId + "\"," +
            "\"playerId\":" + gamePlayerId + "," +
              "\"pos\":" + "92" + "," +
              "\"number\":" + "0" + "," +
              "\"chipsToMove\":" + "1" + "}";

        object requestObjectData = Json.Decode(requestStringData);

        T_SocketRequest request = new T_SocketRequest();
        request.emitEvent = "move";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        T_SocketRequest.Add(request);

    }

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

        object requestObjectData = Json.Decode(requestStringData);

        T_SocketRequest request = new T_SocketRequest();
        request.emitEvent = "move";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        T_SocketRequest.Add(request);
    }

    public void SendRemovePlayer()
    {
        string requestStringData = "{\"registartionId\":" + tournamentSelected.registrationId + "}";

        object requestObjectData = Json.Decode(requestStringData);

        T_SocketRequest request = new T_SocketRequest();
        request.emitEvent = "removePlayer";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        T_SocketRequest.Add(request);

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
    }

    public void RequestRegisterForGame(string gameId) //(int totalPlayer, string timeData)
    {
        string requestStringData = "{" +
            "\"gameId\":" + gameId + "," +
             //"\"userName\": \"" + PlayerManager.instance.GetPlayerGameData().userName + "\"," +
             "\"userId\":" + PlayerManager.instance.GetPlayerGameData().userId + "}";
        //"\"color\":\"" + color + "\"}";

        Debug.Log("Registering for Ludo ---> " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        T_SocketRequest request = new T_SocketRequest();
        request.emitEvent = "register";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        T_SocketRequest.Add(request);
        isRegisterSend = true;

    }

    void SendRejoin()
    {
        if (!string.IsNullOrEmpty(gameId))
        {
            string requestStringData = "{\"registrationId\":" + tournamentSelected.registrationId + ","
                                           + "\"gameId\":" + gameId + "}";
            //\"playerId\":" + PlayerManager.instance.GetPlayerGameData().userId + ","

            Debug.Log("<color=yellow>rejoin</color> for Ludo ---> " + requestStringData);
            object requestObjectData = Json.Decode(requestStringData);


            socketManager.Socket.Emit("rejoin", requestObjectData);
        }
    }

    public void SendTournamentRegister(string tournamentId)
    {
        string requestStringData = "{" +
             "\"userId\":" + PlayerManager.instance.GetPlayerGameData().userId + "," +
            "\"tourneyId\":" + tournamentId + "}";

        Debug.Log("Tournament Register ---> " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        T_SocketRequest request = new T_SocketRequest();
        request.emitEvent = "registerTournament";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        T_SocketRequest.Add(request);
    }

    public void SendTournamentJoin(string registrationId, string tournamentId)
    {
        string requestStringData = "{" +
             "\"registartionId\": " + registrationId + "," +
             "\"userId\": " + PlayerManager.instance.GetPlayerGameData().userId + "," +
            "\"tourneyId\":" + tournamentId + "}";

        Debug.Log("Tournament Join ---> " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        T_SocketRequest request = new T_SocketRequest();
        request.emitEvent = "joinTournament";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        T_SocketRequest.Add(request);
    }

    public void SendGetTourneyList()
    {
        string requestStringData = "{}";

        object requestObjectData = Json.Decode(requestStringData);

        T_SocketRequest request = new T_SocketRequest();
        request.emitEvent = "getTourneyList";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        T_SocketRequest.Add(request);
    }

    //public void SentEmoji(int otherUserId, int emojiIndex)
    //{

    //    string requestStringData = "{\"sentBy\":\"" + ((int.Parse(PlayerManager.instance.GetPlayerGameData().userId)).ToString() + "\"," +
    //        "\"sentTo\":\"" + otherUserId + "\"," +
    //        "\"deductionValue\":\"" + 2 + "\"," +
    //        "\"emojiIndex\":\"" + emojiIndex + "\"," +
    //        "\"tableId\":\"" + int.Parse(TABLE_ID)).ToString() + "\"}";


    //    object requestObjectData = Json.Decode(requestStringData);


    //    T_SocketRequest request = new T_SocketRequest();
    //    request.emitEvent = "sendEmoji";
    //    request.plainDataToBeSend = null;
    //    request.jsonDataToBeSend = requestObjectData;
    //    request.requestDataStructure = requestStringData;
    //    T_SocketRequest.Add(request);
    //}

    #endregion















    // OTHER_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SocketClose()
    {
        ClearVariable();
        ResetConnection();
        //socketManager.Socket.Disconnect();
        //MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
        L_GlobalGameManager.instance.tournamentSocketController.enabled = false;
        L_GlobalGameManager.instance.tournamentSocketController.gameObject.SetActive(false);
        if (L_GlobalGameManager.instance.currentScreenPathList.Contains("Tournament Join"))
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

        //if (isFromTournament)
            //isFromTournament = false;
    }

    public T_SocketState GetSocketState()
    {
        return T_SocketState;
    }

    public void SetSocketState(T_SocketState state)
    {
        Debug.Log("Set Socket State " + state);
        T_SocketState = state;
    }

    public void SetTableId(string tableIdToAssign)
    {
        //T_GlobalGameManager.instance.GetRoomData().socketTableId = tableIdToAssign;
        TABLE_ID = tableIdToAssign;

        //PrefsManager.SetData(PrefsKey.RoomData, JsonUtility.ToJson(T_GlobalGameManager.instance.GetRoomData()));

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

        T_SocketRequest.Clear();
        T_SocketResponse.Clear();

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

            if (GetSocketState() != T_SocketState.NULL)
                SetSocketState(T_SocketState.NULL);
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

        SetSocketState(T_SocketState.ReConnecting);
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

            if (GetSocketState() == T_SocketState.Game_Running)
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




public enum T_SocketEvetns
{
    CONNECT,
    DISCONNECT,
    RECONNECT_ATTEMPT,
    NULL
}

[Serializable]
public class T_SocketRequest
{
    public object jsonDataToBeSend;
    public string plainDataToBeSend;
    public string emitEvent;
    public string requestDataStructure;
}

[Serializable]
public class T_SocketResponse
{
    public T_SocketEvetns eventType;
    public string data;
}

[Serializable]
public class T_ReconnectData
{
    public string tableId;
    public string userId;
    public string isYesOrNo;
}


[Serializable]
public class T_SendMessageData
{
    public string userId;
    public string tableId;

    public string from;
    public string to;
    public string title;
    public string desc;
}


[Serializable]
public class T_RoomCreateData
{
    public string roomId;
    public string players;
    public string isPrivate;
    public string isFree;
}


[Serializable]
public class T_PrivateRoomJoinData
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
public enum T_SocketState
{
    Connecting,
    Connected,
    WaitingForOpponent,
    Game_Running,
    //InitializingCards,
    ReConnecting,
    NULL
}

[Serializable]
public class TournamentSelected
{
    public string id;
    public string registrationId;
    public string gameTypeId;
    //public string duration;
    public string playerSize;
    public string winningAmount;
    public string entryFee;
    public string status;
    public string winnerId;
    public string scheduledDate;
    public string title;
    public string playerType;
    public int registered_users_count;
}
