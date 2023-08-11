using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP.SocketIO3;
using System.Collections;
using BestHTTP.JSON;
using LitJson;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BestHTTP.SocketIO3.Events;
//using TMPro;
//using DG.Tweening;
//using System.Linq;

public class P_LobbySocketController : MonoBehaviour
{
    public static P_LobbySocketController instance;

    private const float RESPONSE_READ_DELAY = 0.2f, REQUEST_SEND_DELAY = 0.1f;
    private SocketManager socketManager;

    private List<P_LobbySocketResponse> P_SocketResponse = new List<P_LobbySocketResponse>();
    private List<P_LobbySocketRequest> P_SocketRequest = new List<P_LobbySocketRequest>();

    [SerializeField]
    private P_LobbySocketState P_SocketState;

    public string TABLE_ID = "";
    public string gameId;
    public string gamePlayerId; // user id
    public string gamePlayerName; // user name
    public string gamePlayerToken;
    public string myColor;
    public int swapIndex;
    int isGameObjectFirstTime = 0;
    public JsonData tableData;
    public string smallBlindTableData = "0", bigBlindTableData = "0", minimumBuyinTableData = "0";
    public JsonData gameTableData;
    //public JsonData firstSeatTableData;
    public float firstSeatSmallBlind;


    public int playerTurnIndex = 0, lastPlayerTurnIndex = -1, oneTokenOpenPos, totalPlayersInGame = 0, myPlayerIndex = -1;






    [HideInInspector]
    public string winnerName;

    float timeRemaining, userServerTimer;
    bool isStartTime = false;
    [HideInInspector]
    public bool needToFetchLobby = false;
    int turnValue = 0;

    public JsonData ReturnToStartPoint = null;
    [HideInInspector]


    Coroutine applicationQuitCo;

    GameObject go;

    bool isClose = false;

    public string[] communityCardsRank, communityCardsSuit;
    public string idleTimeout;
    public string turnTimerStr;
    public string currentTurnUserId;

    public int currentBet;  // TURN_CHANGED event ka callAmount ya raise send kiya ho to wo amount
    public bool isViewer = false;
    public bool isMyBalanceZero = false;
    public bool isJoinSended = false;
    public bool isTopUpSended = false;
    public bool isGameCounterStart = true;
    public bool isTopUpSend = false;
    public bool isCheckForInternet = false;

    void Awake()
    {
        instance = this;
        //if (Application.isEditor)
        Application.runInBackground = true;
        //SetSocketState(P_SocketState.NULL);
        turnValue = 0;

        //Application.wantsToQuit += OnQuit;
    }

    void Start()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 30;

        Connect();
        //StartCoroutine(WaitAndCheckInternetConnection());
        //SetStartingDetails();
    }

    private bool OnQuit()
    {
        return isClose;
    }

    private void OnEnable()
    {
        //Debug.Log("ONENABLE SOCKET");
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 30;

        //Connect();
        //StartCoroutine(WaitAndCheckInternetConnection());
    }

    void StartTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int m = Mathf.FloorToInt(timeRemaining / 60.0f);
            int s = Mathf.FloorToInt(timeRemaining - m * 60f);
            //P_InGameUiManager.instance.timerText.text = string.Format("{0:00}:{1:00}", m, s);
        }
        else
        {
            //P_InGameUiManager.instance.timerText.text = string.Format("{0:00}:{1:00}", 0, 0);
            isStartTime = false;
        }
    }


    private void OnDestroy()
    {
        //    SendLeaveMatchRequest();
        //Debug.Log("socket controller OnDestroy");

    }

    bool isPaused = false;

    //void OnGUI()
    //{
    //    if (isPaused)
    //    {
    //        GUI.Label(new Rect(100, 100, 50, 30), "Game paused");
    //    }
    //}

#if UNITY_ANDROID && !UNITY_EDITOR
    //void OnApplicationFocus(bool focus)
    //{
    //    if (focus)
    //    {
    //        //ResumeApplication();
    //        Debug.Log("111111 FOCUS: TRUE");
    //        if (applicationQuitCo != null)
    //        {
    //            Debug.Log("111111 FOCUS: TRUE StopCoroutine WaitAfterFocusLost()");
    //            //StopCoroutine(applicationQuitCo);
    //            //SendRejoin();
    //        }
    //    }
    //    else
    //    {
    //        //LeaveApplication();
    //        Debug.Log("111111 FOCUS: FALSE APP QUIT");
    //        //applicationQuitCo = StartCoroutine(WaitAfterFocusLost());
    //    }
    //}
#endif

#if UNITY_EDITOR || UNITY_IOS
    //void OnApplicationPause(bool pause)
    //{
    //    if (pause)
    //    {
    //        //LeaveApplication();
    //        Debug.Log("111111 PAUSE: TRUE");
    //    }
    //    else
    //    {
    //        Debug.Log("111111 PAUSE: FALSE");
    //        //if (applicationQuitCo != null)
    //        {
    //           // Debug.Log("111111 PAUSE: FALSE StopCoroutine WaitAfterFocusLost()");
    //            //StopCoroutine(applicationQuitCo);
    //            //SendRejoin();
    //        }
    //    }
    //}
#endif



    void SetStartingDetails()
    {
        gamePlayerId = PlayerManager.instance.GetPlayerGameData().userId;
        gamePlayerName = PlayerManager.instance.GetPlayerGameData().userName;
        if (P_InGameUiManager.instance != null)
            P_InGameUiManager.instance.tableInfoText.text = gameTableData["table_name"].ToString();
    }
    
    public void Connect(bool isReconnecting = false)
    {
        ResetConnection(isReconnecting);
        SetSocketState(P_LobbySocketState.Connecting);


        if (!IsInvoking("HandleSocketResponse"))
        {
            InvokeRepeating("HandleSocketResponse", RESPONSE_READ_DELAY, RESPONSE_READ_DELAY);
        }

        if (!IsInvoking("SendSocketRequest"))
        {
            InvokeRepeating("SendSocketRequest", REQUEST_SEND_DELAY, REQUEST_SEND_DELAY);
        }

        ReConnect();
        //SendJoinViewer();
    }

    private void ReConnect()
    {
        socketManager = null;
        SocketOptions socketOptions = new SocketOptions();
        socketOptions.Timeout = new TimeSpan(0, 0, 4);
        socketOptions.Reconnection = false;
        socketOptions.AutoConnect = false;
        socketOptions.ReconnectionDelayMax = new TimeSpan(0, 0, 4);

        socketManager = new SocketManager(new Uri(P_GameConstant.LOBBY_SOCKET_URL + "/"), socketOptions);
        if (P_GameConstant.enableLog)
            Debug.Log(socketManager.Uri);

        //socketManager.Open();

        socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnServerConnect);
        socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Disconnect, OnServerDisconnect);
        socketManager.Socket.On<SocketCustomError>(SocketIOEventTypes.Error, OnSocketError);
        //socketManager.Socket.On(SocketIOEventTypes.Error, OnError);
        //socketManager.Socket.On<string>("chat message", OnChatMessage);

        ////Default Events
        //socketManager.Socket.On("reconnect", OnReconnect);
        //socketManager.Socket.On("reconnecting", OnReconnecting);
        //socketManager.Socket.On("reconnect_attempt", OnReconnectAttempt);
        //socketManager.Socket.On("reconnect_failed", OnReconnectFailed);

        //Poker Events
        socketManager.Socket.On<string>("SEAT", OnSeatReceive);
        socketManager.Socket.On<string>("USER_STACK", OnUserStack);
        socketManager.Socket.On<string>("GAME_COUNTER", OnGameCounter);
        socketManager.Socket.On<string>("DEALER", OnDealer);
        socketManager.Socket.On<string>("TURN_CHANGED", OnTurnChanged);
        socketManager.Socket.On<string>("ACTIONS", OnActions);
        socketManager.Socket.On<string>("TURN_TIMER", OnTurnTimer);
        socketManager.Socket.On<string>("POT", OnPot);
        socketManager.Socket.On<string>("BET", OnBet);
        socketManager.Socket.On<string>("COMMUNITY_CARD", OnCommunityCard);
        socketManager.Socket.On<string>("HOLE_CARD", OnHoleCard);
        socketManager.Socket.On<string>("WINNER", OnWinner);
        socketManager.Socket.On<string>("ERROR_EVENT", OnError);
        socketManager.Socket.On<string>("BEST_HAND", OnBestHand);
        socketManager.Socket.On<string>("ACTION_BY_USER", OnActionByUser);
        socketManager.Socket.On<string>("SUGGESTED_ACTION", OnSuggestedAction);
        socketManager.Socket.On<string>("ROUND_CHANGE", OnRoundChange);
        socketManager.Socket.On<string>("PLAYER_COUNT", OnPlayerCount);

        socketManager.Open();
    }

    private void HandleSocketResponse()
    {
        if (P_SocketResponse.Count > 0)
        {
            P_LobbySocketResponse responseObject = P_SocketResponse[0];
            P_SocketResponse.RemoveAt(0);

            if (P_GameConstant.enableLog)
                Debug.Log(gameObject.name + " <color=yellow> " + responseObject.eventType + ",</color> " + responseObject.data);

            switch (responseObject.eventType)
            {
                case P_SocketEvetns.CONNECT:
                    {
                        //SendPlayerDetails();

                        switch (GetSocketState())
                        {
                            case P_LobbySocketState.Connecting:
                                //SetSocketState(P_SocketState.WaitingForOpponent);
                                //Debug.Log(responseObject.eventType + "<color=yellow> IsJoiningPreviousGame " + P_GlobalGameManager.IsJoiningPreviousGame + "</color>");
                                //CreateRoom();
                                SetSocketState(P_LobbySocketState.Connected);
                                //SendJoin(LoginScreen.instance.tableIdtext.text, LoginScreen.instance.userNametext.text, LoginScreen.instance.chipsToPlaytext.text);
                                break;

                            case P_LobbySocketState.ReConnecting:
                                //SendRejoin();
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case P_SocketEvetns.DISCONNECT:
                    StartReconnectProcedure();
                    break;


                case P_SocketEvetns.RECONNECT_ATTEMPT:
                    break;

                default:
                    if (P_GameConstant.enableErrorLog)
                        Debug.LogError("UnHandlled EventType Found in response eventType = " + responseObject.eventType + "   responseStructure = " + responseObject.data);
                    break;
            }
        }
    }


    private void SendSocketRequest()
    {
        if (P_SocketRequest.Count > 0)
        {
            P_LobbySocketRequest request = P_SocketRequest[0];
            P_SocketRequest.RemoveAt(0);

            if (request.plainDataToBeSend != null)
            {
                socketManager.Socket.Emit(request.emitEvent, request.plainDataToBeSend);

                if (P_GameConstant.enableLog)
                    Debug.Log("sending event = <color=yellow>" + request.emitEvent + "</color>        Plain request = " + request.requestDataStructure + "     Time = " + System.DateTime.Now);
            }
            else if (request.jsonDataToBeSend != null)
            {
                socketManager.Socket.Emit(request.emitEvent, request.jsonDataToBeSend);

                if (P_GameConstant.enableLog)
                    Debug.Log("sending event = <color=yellow>" + request.emitEvent + "</color>        Request = " + request.requestDataStructure + "     Time = " + System.DateTime.Now);
            }
        }
    }










    // LISTNER_METHODS --------------------------------------------------------------------------------------------------------------
    #region LISTNER_METHODS
    void OnServerConnect(ConnectResponse res /*Socket socket, Packet packet, params object[] args*/)
    {
        if (P_GameConstant.enableLog)
        {
            Debug.Log("Enter in OnServerConnect Time = " + System.DateTime.Now);
            Debug.Log("Enter in connect rrsp = " + res.sid);
        }

        P_LobbySocketResponse response = new P_LobbySocketResponse();
        response.eventType = P_SocketEvetns.CONNECT;
        P_SocketResponse.Add(response);
    }

    void OnServerDisconnect(ConnectResponse res)
    {
        if (P_GameConstant.enableLog)
        {
            Debug.Log("Enter in OnServerDisconnect Time = " + System.DateTime.Now);
            Debug.Log("Enter in Disconnect resp = " + res.sid);
        }

        P_LobbySocketResponse response = new P_LobbySocketResponse();
        response.eventType = P_SocketEvetns.DISCONNECT;
        P_SocketResponse.Add(response);

        SocketClose();
    }

    //private void OnGameTimeLeft(Socket socket, object[] args)
    //{
    //    string responseText = JsonMapper.ToJson(args);
    //    //Debug.Log("Socket => <color=yellow>GAME_TIME_LEFT</color>: " + responseText);
    //    //if (Panel_Controller.instance != null)
    //    {
    //        string trimedStr = responseText.Remove(0, 2); //["10:00"]   10:00"]
    //        trimedStr = trimedStr.Remove((trimedStr.Length - 2), 2); //10:00"]   10:00
    //        //Panel_Controller.instance.gameTimerText.text = trimedStr;
    //    }
    //}

    private void OnGameCounter(string str)
    {
        //if (P_GameConstant.enableLog)
        //    Debug.Log("<color=yellow>GAME_COUNTER</color>: " + gameConter);

        P_InGameUiManager.instance.OnGameCounterSet(str);
    }

    private void OnDealer(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>DEALER</color>: " + str);
        
        P_InGameManager.instance.DealerIconSetTrue(str);
    }

    private void OnSeatReceive(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>SEAT</color>: " + str);

        P_InGameManager.instance.OnSeatReceiveSet(str);
    }

    private void OnUserStack(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>USER_STACK</color>: " + str);

        P_InGameManager.instance.OnUserStackSet(str);
    }

    private void OnHoleCard(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>HOLE_CARD</color>: " + str);

        P_InGameManager.instance.OnHoleCardSet(str);
    }

    private void OnTurnChanged(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>TURN_CHANGED</color>: " + str);

        P_InGameManager.instance.OnTurnChangedSet(str);
    }

    private void OnActions(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>ACTIONS</color>: " + str);

        P_InGameManager.instance.OnActionsSet(str);
    }

    private void OnTurnTimer(string turnTime)
    {
        string tempPlayerTurnIndex = currentTurnUserId;
        //playerTurn = tempPlayerTurnIndex;
        if (P_GameConstant.enableLog)
            Debug.Log("TURN_TIMER: " + turnTime);
        //idleTimeout = turnTime;
        turnTimerStr = turnTime;

        //P_InGameUiManager.instance.StopIdleTimerFunc();
        //P_InGameUiManager.instance.IdleTimerFunc(currentTurnUserId);
        ////PanelController.instance.IdleTimerFunc(tempPlayerTurnIndex);
    }

    private void OnPot(string str)  //atyare pot event mathi je card batavya che ee nathi batavana atyar purtu j che khali.
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>POT</color>: " + str);

        P_InGameUiManager.instance.OnPotSet(str);
    }

    private void OnBet(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>BET</color>: " + str);

        try
        {
            JsonData data = JsonMapper.ToObject(str);
            //P_InGameManager.instance.onBetAmount = float.Parse(data["bet"].ToString());
            //currentBet = int.Parse(data["bet"].ToString());
            //StartCoroutine(BetAmountAnim());
        }
        catch(Exception e)
        {
            if (P_GameConstant.enableLog)
                Debug.Log(e.Message);
        }
    }

    //community card event ma je card mde e display karava mate.
    private void OnCommunityCard(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>COMMUNITY_CARD</color>: " + str);

        P_InGameManager.instance.OnCommunityCardSet(str);
    }

    private void OnWinner(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>WINNER</color>: " + str);

        P_InGameManager.instance.OnWinnerSet(str);
    }

    private void OnError(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=red>ERROR_EVENT</color>:" + str);

        P_InGameManager.instance.OnErrorSet(str);
    }
    
    private void OnBestHand(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>BEST_HAND</color>:" + str);

        P_InGameManager.instance.BestHandText(str);
    }
    
    private void OnActionByUser(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>ACTION_BY_USER</color>:" + str);

        P_InGameManager.instance.OnActionByUserSet(str);
    }

    private void OnSuggestedAction(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>SUGGESTED_ACTION</color>:" + str);

        P_InGameManager.instance.OnSuggestionActionSet(str);
    }

    private void OnRoundChange(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>ROUND_CHANGE</color>:" + str);

        P_InGameUiManager.instance.OnRoundChangeSet(str);
    }

    private void OnPlayerCount(string str)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("<color=yellow>PLAYER_COUNT</color>:" + str);

        //P_InGameUiManager.instance.OnRoundChangeSet(str);
    }

    void OnSocketError(SocketCustomError args)
    {
        if (P_GameConstant.enableLog)
            Debug.LogError(string.Format("Error: {0}", args.ToString()));
    }
    #endregion



    // EMIT_METHODS -----------------------------------------------------------------------------------------------------------------------------------------
    #region EMIT_METHODS


    // OTHER_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SocketClose()
    {
        //ClearVariable();
        ResetConnection();
        ////socketManager.Socket.Disconnect();
        ////MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
        //GlobalGameManager.instance.socketController.enabled = false;
        //GlobalGameManager.instance.socketController.gameObject.SetActive(false);
        ////if (GlobalGameManager.instance.currentScreenPathList.Contains("ClassicLudoSelection") || GlobalGameManager.instance.currentScreenPathList.Contains("QuickLudoSelection"))
        ////    GlobalGameManager.instance.currentScreenPathList.Clear();
        //Debug.Log("list clear count=" + GlobalGameManager.instance.currentScreenPathList.Count);
    }


    public P_LobbySocketState GetSocketState()
    {
        return P_SocketState;
    }

    public void SetSocketState(P_LobbySocketState state)
    {
        if (P_GameConstant.enableLog)
            Debug.Log("Set Socket State " + state);
        P_SocketState = state;
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
            try
            {
                socketManager.Close();
            }
            catch(Exception e)
            {
                if (P_GameConstant.enableLog)
                    Debug.Log("Socket close error: "+ e.Message);
            }
        }

        P_SocketRequest.Clear();
        P_SocketResponse.Clear();

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

            if (GetSocketState() != P_LobbySocketState.NULL)
                SetSocketState(P_LobbySocketState.NULL);

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

        SetSocketState(P_LobbySocketState.ReConnecting);
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
            //     P_InGameManager.instance.LoadMainMenu();
            // }, "Retry", "Cancel"
            //);
        }

        isPreocedureRunning = false;
    }

    private IEnumerator WaitAndCheckInternetConnection()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); //GameConstant.NETWORK_CHECK_DELAY

            if (GetSocketState() == P_LobbySocketState.Game_Running)
            {
                if (!WebServices.instance.IsInternetAvailable())
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

[Serializable]
public enum P_LobbySocketState
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
public class P_LobbySocketResponse
{
    public P_SocketEvetns eventType;
    public string data;
}

[Serializable]
public class P_LobbySocketRequest
{
    public object jsonDataToBeSend;
    public string plainDataToBeSend;
    public string emitEvent;
    public string requestDataStructure;
}

#endregion