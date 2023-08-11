using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;
using System;

public class Tournaments : MonoBehaviour
{
    public static Tournaments instance;

    [Header("-----Main Tab Toggles-----")]
    public Text upcomingTabText;
    public Text joinedTabText;
    public Text finishedTabText;

    [Header("------Main Tab On Off Sprites & Color-----")]
    public Color mainTabOnTextColor;
    public Color mainTabOffTextColor;

    [Header("-----Child Tab Toggles-----")]
    public TMP_Text allTabText;
    public TMP_Text players2TabText;
    public TMP_Text players4TabText;
    public TMP_Text singleWinnerTabText;
    public TMP_Text multiWinnerTabText;

    [Header("------Child Tab On Off Sprites & Color-----")]
    public Color childTabOnTextColor;
    public Color childTabOffTextColor;

    [Header("------Toggle Groups-----")]
    public ToggleGroup topToggleGroup;
    public ToggleGroup childToggleGroup;

    [Header("------Top Toggle Options-----")]
    public Toggle upcomingToggle;
    public Toggle joinedToggle;
    public Toggle finishedToggle;

    [Header("------Child Toggle Options-----")]
    public Toggle allToggle;
    public Toggle players2Toggle;
    public Toggle players4Toggle;

    [Header("------Scroll-----")]
    public GameObject upcomingScrollView;
    public GameObject joinedScrollView;
    public GameObject finishedScrollView;
    public GameObject tournyItem;
    public Transform upcomingContent;
    public Transform joinedContent;
    public Transform finishedContent;
    public Text noTournamentText;

    [Header("------Button Sprite-----")]
    public Sprite registerSprite;
    public Sprite joinSprite;
    public Sprite finishSprite;

    [Header("------Button Sprite-----")]
    public GameObject topParent;
    public GameObject bottomParent;
    public GameObject waitingParent;
    public TMP_Text waitingParentMsgText;

    public bool isRegisterResponseSuccess = false;


    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        Debug.Log("tournament list start() socket on");

        L_GlobalGameManager.instance.tournamentSocketController.enabled = true;
        //GlobalGameManager.instance.tournamentSocketController.isFromTournament = true;
        L_GlobalGameManager.instance.tournamentSocketController.gameObject.SetActive(true);

        //TournamentUpcoming();
        T_SocketController.instance.SendGetTourneyList();
    }

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "back":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                break;

            case "wallet":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Wallet);
                break;

            case "rules":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Rules);
                break;
        }
    }


    public void OnToggleValueChanged(string toggleName)
    {
        switch (toggleName)
        {
            // main tab

            case "upcoming":
                ShowUpcomingTab();
                break;

            case "joined":
                ShowJoinedTab();
                break;

            case "finished":
                ShowFinishedTab();
                break;


            // child tab

            case "all":
                ShowAllTab();
                break;

            case "2players":
                Show2PlayersTab();
                break;

            case "4players":
                Show4PlayersTab();
                break;

            case "singleWinner":
                ShowSingleWinnerTab();
                break;

            case "multiWinner":
                ShowMultiWinnerTab();
                break;
        }
    }

    /*
    upcoming all player2 player4
    joined all player2 player4
    finished all player2 player4
     */
    void ShowUpcomingTab()
    {
        Debug.Log("ShowUpcomingTab");
        if (upcomingToggle.isOn)
        {
            Debug.Log("ShowUpcomingTab upcomingToggle allToggle: " + allToggle.isOn);
            L_MainMenuController.instance.PlayButtonSound();

            if (allToggle.isOn)
            {
                //TournamentUpcoming();
                DestroyAllScrollItems();
                ShowUpcomingScrollView();
                T_SocketController.instance.SendGetTourneyList();
            }
            else
                allToggle.isOn = true;
        }

        upcomingTabText.color = mainTabOnTextColor;
        joinedTabText.color = mainTabOffTextColor;
        finishedTabText.color = mainTabOffTextColor;
        Debug.Log("ShowUpcomingTab last");
    }

    void ShowJoinedTab()
    {
        if (joinedToggle.isOn)
        {
            L_MainMenuController.instance.PlayButtonSound();

            if (allToggle.isOn)
            {
                //TournamentRegistered();
                DestroyAllScrollItems();
                ShowJoinedScrollView();
                T_SocketController.instance.SendGetTourneyList();
            }
            else
                allToggle.isOn = true;
        }

        joinedTabText.color = mainTabOnTextColor;
        upcomingTabText.color = mainTabOffTextColor;
        finishedTabText.color = mainTabOffTextColor;
    }

    void ShowFinishedTab()
    {
        if (finishedToggle.isOn)
        {
            L_MainMenuController.instance.PlayButtonSound();

            if (allToggle.isOn)
            {
                //TournamentFinished();
                DestroyAllScrollItems();
                ShowFinishedScrollView();
                T_SocketController.instance.SendGetTourneyList();
            }
            else
                allToggle.isOn = true;
        }

        finishedTabText.color = mainTabOnTextColor;
        upcomingTabText.color = mainTabOffTextColor;
        joinedTabText.color = mainTabOffTextColor;
    }

    void ShowAllTab()
    {
        if (allToggle.isOn)
        {
            L_MainMenuController.instance.PlayButtonSound();

            DestroyAllScrollItems();

            if (upcomingToggle.isOn)
            {
                //TournamentUpcoming();
                ShowUpcomingScrollView();
            }
            else if (joinedToggle.isOn)
            {
                //TournamentRegistered();
                ShowJoinedScrollView();
            }
            else if (finishedToggle.isOn)
            {
                //TournamentFinished();
                ShowFinishedScrollView();
            }
            T_SocketController.instance.SendGetTourneyList();
        }

        allTabText.color = childTabOnTextColor;
        players2TabText.color = childTabOffTextColor;
        players4TabText.color = childTabOffTextColor;
        singleWinnerTabText.color = childTabOffTextColor;
        multiWinnerTabText.color = childTabOffTextColor;
    }

    void Show2PlayersTab()
    {
        if (players2Toggle.isOn)
        {
            L_MainMenuController.instance.PlayButtonSound();

            DestroyAllScrollItems();

            if (upcomingToggle.isOn)
            {
                //TournamentUpcoming("2players");
                ShowUpcomingScrollView();
            }
            else if (joinedToggle.isOn)
            {
                //TournamentRegistered("2players");
                ShowJoinedScrollView();
            }
            else if (finishedToggle.isOn)
            {
                //TournamentFinished("2players");
                ShowFinishedScrollView();
            }
            T_SocketController.instance.SendGetTourneyList();
        }

        players2TabText.color = childTabOnTextColor;
        allTabText.color = childTabOffTextColor;
        players4TabText.color = childTabOffTextColor;
        singleWinnerTabText.color = childTabOffTextColor;
        multiWinnerTabText.color = childTabOffTextColor;
    }

    void Show4PlayersTab()
    {
        if (players4Toggle.isOn)
        {
            L_MainMenuController.instance.PlayButtonSound();

            DestroyAllScrollItems();

            if (upcomingToggle.isOn)
            {
                //TournamentUpcoming("4players");
                ShowUpcomingScrollView();
            }
            else if (joinedToggle.isOn)
            {
                //TournamentRegistered("4players");
                ShowJoinedScrollView();
            }
            else if (finishedToggle.isOn)
            {
                //TournamentFinished("4players");
                ShowFinishedScrollView();
            }
            T_SocketController.instance.SendGetTourneyList();
        }

        players4TabText.color = childTabOnTextColor;
        allTabText.color = childTabOffTextColor;
        players2TabText.color = childTabOffTextColor;
        singleWinnerTabText.color = childTabOffTextColor;
        multiWinnerTabText.color = childTabOffTextColor;
    }

    void ShowSingleWinnerTab()
    {
        singleWinnerTabText.color = childTabOnTextColor;
        allTabText.color = childTabOffTextColor;
        players2TabText.color = childTabOffTextColor;
        players4TabText.color = childTabOffTextColor;
        multiWinnerTabText.color = childTabOffTextColor;
    }

    void ShowMultiWinnerTab()
    {
        multiWinnerTabText.color = childTabOnTextColor;
        allTabText.color = childTabOffTextColor;
        players2TabText.color = childTabOffTextColor;
        players4TabText.color = childTabOffTextColor;
        singleWinnerTabText.color = childTabOffTextColor;
    }


    void ShowUpcomingScrollView()
    {
        joinedScrollView.SetActive(false);
        finishedScrollView.SetActive(false);
        upcomingScrollView.SetActive(true);
    }

    void ShowJoinedScrollView()
    {
        upcomingScrollView.SetActive(false);
        finishedScrollView.SetActive(false);
        joinedScrollView.SetActive(true);
    }

    void ShowFinishedScrollView()
    {
        upcomingScrollView.SetActive(false);
        joinedScrollView.SetActive(false);
        finishedScrollView.SetActive(true);
    }

    void DestroyAllScrollItems()
    {
        DestroyUpcomingScrollItems();
        DestroyJoinedScrollItems();
        DestroyFinishedScrollItems();
    }

    void DestroyUpcomingScrollItems()
    {
        for (int i = 0; i < upcomingContent.childCount; i++)
        {
            Destroy(upcomingContent.GetChild(i).gameObject);
        }
    }
    void DestroyJoinedScrollItems()
    {
        for (int i = 0; i < joinedContent.childCount; i++)
        {
            Destroy(joinedContent.GetChild(i).gameObject);
        }
    }
    void DestroyFinishedScrollItems()
    {
        for (int i = 0; i < finishedContent.childCount; i++)
        {
            Destroy(finishedContent.GetChild(i).gameObject);
        }
    }

    void ShowWaitingScreen()
    {
        topParent.SetActive(false);
        bottomParent.SetActive(false);
        waitingParent.SetActive(true);
    }

    public void HideWaitingScreen()
    {
        waitingParent.SetActive(false);
        waitingParentMsgText.text = "Please wait";
        topParent.SetActive(true);
        bottomParent.SetActive(true);
    }


    public void GameErrorReturn(string msg)
    {
        Debug.Log("GameErrorReturn msg: " + msg);
        if (!string.IsNullOrEmpty(msg))
        {
            try
            {
                JsonData data = JsonMapper.ToObject(msg);
                if (waitingParent.activeSelf)
                {
                    //StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(waitingParentMsgText, data[0]["message"].ToString(), "white", 3f, () => { HideWaitingScreen(); }));
                    waitingParentMsgText.text = data[0]["message"].ToString();
                }
            }
            catch (Exception e)
            {
                Debug.Log("get error: " + e.Message);
                HideWaitingScreen();
            }
        }
        else
        {
            HideWaitingScreen();
        }
    }

    public void PlayerAddedReturn(string msg)
    {
        if (waitingParent.activeSelf)
        {
            waitingParentMsgText.text = msg;
        }
    }

    public void OnClickCloseWaiting()
    {
        Debug.Log("Close clicked");
        HideWaitingScreen();

        if (isRegisterResponseSuccess)
        {
            Debug.Log("Close clicked if register success");

            //if (joinedToggle.isOn)
            //    OnToggleValueChanged("joined");
            //else
            //    joinedToggle.isOn = true;

            isRegisterResponseSuccess = false;
        }
        //else
        //{
        //    Debug.Log("Close clicked upcoming");
        //    //OnToggleValueChanged("upcoming");

            if (upcomingToggle.isOn)
                ShowUpcomingTab();

            if (joinedToggle.isOn)
                ShowJoinedTab();

            if (finishedToggle.isOn)
                ShowFinishedTab();
        //}
    }



    void TournamentUpcoming(string subCategory="all")
    {
        DestroyAllScrollItems();
        ShowUpcomingScrollView();

        StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.TournamentUpcoming] + PlayerManager.instance.GetPlayerGameData().userId, (serverResponse, errorBool, error) =>
        {
            if (errorBool)
            {
                Debug.Log("Error in Tournament List 1: " + error);
            }
            else
            {
                Debug.Log("Upcoming Tournament response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("code"))
                {
                    if (data["code"].ToString() == "200")
                    {
                        if (data["data"].Count > 0)
                        {
                            noTournamentText.gameObject.SetActive(false);

                            for (int i = 0; i < data["data"].Count; i++)
                            {
                                //Debug.Log("Tournament id: " + data["data"][i]["id"].ToString());
                                //Debug.Log("Tournament title: " + data["data"][i]["title"].ToString());
                                /*
                                    {
                                    "id": 1,
                                    "gameTypeId": 1,  //
                                    "duration": 10,
                                    "playerSize": 10,
                                    "winningAmount": 20,
                                    "entryFee": 2,
                                    "status": 0,      //
                                    "scheduledDate": "2023-04-08T00:00:00.000Z",
                                    "winnerId": -1,   //
                                    "title": "Daily Tournament",
                                    "playerType": 2,
                                    "createdAt": "2023-04-07T12:04:06.000Z",
                                    "updatedAt": "2023-04-07T12:04:06.000Z"
                                }
                                    */
                                bool sortingPlayerTypeElse = false;
                                if (subCategory == "2players")
                                {
                                    if (data["data"][i]["playerType"].ToString().Equals("4"))
                                        continue;
                                    else
                                        sortingPlayerTypeElse = true;
                                }
                                else if (subCategory == "4players")
                                {
                                    if (data["data"][i]["playerType"].ToString().Equals("2"))
                                        continue;
                                    else
                                        sortingPlayerTypeElse = true;
                                }
                                else
                                {
                                    sortingPlayerTypeElse = true;
                                }

                                if (sortingPlayerTypeElse)
                                {
                                    GameObject tournamentsObj = Instantiate(tournyItem, upcomingContent);
                                    string tourneyId = data["data"][i]["id"].ToString();
                                    TournamentsItem ti = tournamentsObj.GetComponent<TournamentsItem>();
                                    ti.id = tourneyId;
                                    ti.tournyNameTMP.text = data["data"][i]["title"].ToString();
                                    ti.playersTMP.text = data["data"][i]["playerType"].ToString() + " PLAYERS";
                                    //ti.durationTMP.text = data["data"][i]["duration"].ToString() + " Mins";
                                    ti.playersOutOfTMP.text = "0/" + data["data"][i]["playerSize"].ToString();
                                    ti.amountTMP.text = "₹" + data["data"][i]["winningAmount"].ToString();
                                    ti.enterAmountTMP.text = "ENTRY AMOUNT ₹" + data["data"][i]["entryFee"].ToString();
                                    ti.startsTMP.text = "";
                                    ti.callFrom = "upcoming";

                                    TMP_Text regBtnTxt = ti.regButtonTMP;
                                    Image regBtnImageTemp = ti.regButtonImage;
                                    Button regBtnTemp = ti.regButton;
                                    string status = data["data"][i]["status"].ToString();

                                    if (status == "0")
                                    {
                                        regBtnTxt.text = "Register";
                                        regBtnImageTemp.sprite = registerSprite;

                                        ti.startsLabel.text = "STARTS";
                                    }
                                    else if (status == "1")
                                    {
                                        regBtnTxt.text = "Join";
                                        regBtnImageTemp.sprite = joinSprite;

                                        ti.startsLabel.text = "RUNNING";
                                    }
                                    else if (status == "2")
                                    {
                                        regBtnTxt.text = "Finished";
                                        regBtnImageTemp.sprite = finishSprite;

                                        ti.startsLabel.text = "ENDED";
                                    }
                                    else if (status == "3")
                                    {
                                        regBtnTxt.text = "Cancel";
                                        regBtnImageTemp.sprite = finishSprite;

                                        ti.startsLabel.text = "CANCEL";
                                    }

                                    TMP_Text startsTMPTemp = ti.startsTMP;
                                    string scheduleDate = data["data"][i]["scheduledDate"].ToString();
                                    //string scheduleDate = "2023-04-18T10:25:00.000Z";
                                    DateTime scheduleDateConverted = Convert.ToDateTime(scheduleDate).ToLocalTime();
                                    DateTime today = Convert.ToDateTime(data["currentTime"].ToString()).ToLocalTime(); //DateTime.Now;
                                                                                                                       //TimeSpan differenceScheduleDate = today.Subtract(scheduleDateConverted);
                                    TimeSpan differenceScheduleDate = scheduleDateConverted.Subtract(today);
                                    int totalSecondsScheduleDate = (int)differenceScheduleDate.TotalSeconds;
                                    //Debug.Log("tourneyId="+tourneyId+", status="+status+", title="+ti.tournyNameTMP.text+", scheduleDateLocal="+scheduleDateConverted
                                    //    +"\n scheduleDate="+scheduleDate+", totalSecondsScheduleDate="+totalSecondsScheduleDate);

                                    if (totalSecondsScheduleDate != 0)
                                    {
                                        if (totalSecondsScheduleDate > 0)
                                        {
                                            ti.TimerStart(scheduleDateConverted, totalSecondsScheduleDate, startsTMPTemp, today);

                                            if (status == "0")
                                            {
                                                regBtnTemp.onClick.AddListener(() =>
                                                {
                                                    Debug.Log("regBtn onclick tourneyId: " + tourneyId);
                                                    ShowWaitingScreen();
                                                    //GlobalGameManager.instance.tournamentSocketController.isFromTournament = true;
                                                    if (L_GlobalGameManager.instance.tournamentSocketController.enabled == false) //if (L_SocketController.instance == null)
                                                    {
                                                        Debug.Log("socketcontroller 1 instace null");
                                                        L_GlobalGameManager.instance.tournamentSocketController.enabled = true;
                                                        L_GlobalGameManager.instance.tournamentSocketController.gameObject.SetActive(true);
                                                    }
                                                    L_GlobalGameManager.instance.tournamentSocketController.SendTournamentRegister(tourneyId);
                                                });
                                            }
                                        }
                                        else if (totalSecondsScheduleDate < 0)
                                        {
                                            //ti.startsLabel.text = "ENDED";
                                            //ti.startsTMP.text = "";
                                        }
                                    }
                                }
                            }

                            if (upcomingContent.childCount == 0)
                                OnNoData();
                        }
                        else
                        {
                            OnNoData();
                        }
                    }
                    else if (iData1.Contains("errorMessage"))
                    {
                        Debug.Log("Error in Tournament Upcoming List 2");
                    }
                    else
                    {
                        Debug.Log("Error in Tournament Upcoming List 3");
                    }
                }
                else
                {
                    Debug.Log("Success in Tournament Upcoming List 4");
                }
            }
        }));
    }



    void TournamentRegistered(string subCategory = "all")
    {
        DestroyAllScrollItems();
        ShowJoinedScrollView();

        StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.TournamentRegistered] + PlayerManager.instance.GetPlayerGameData().userId, (serverResponse, errorBool, error) =>
        {
            if (errorBool)
            {
                Debug.Log("Error in Tournament List 1: " + error);
            }
            else
            {
                Debug.Log("Regestered Tournament response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("code"))
                {
                    if (data["code"].ToString() == "200")
                    {
                        if (data["data"].Count > 0)
                        {
                            noTournamentText.gameObject.SetActive(false);

                            for (int i = 0; i < data["data"].Count; i++)
                            {
                                bool sortingPlayerTypeElse = false;
                                if (subCategory == "2players")
                                {
                                    if (data["data"][i]["playerType"].ToString().Equals("4"))
                                        continue;
                                    else
                                        sortingPlayerTypeElse = true;
                                }
                                else if (subCategory == "4players")
                                {
                                    if (data["data"][i]["playerType"].ToString().Equals("2"))
                                        continue;
                                    else
                                        sortingPlayerTypeElse = true;
                                }
                                else
                                {
                                    sortingPlayerTypeElse = true;
                                }

                                if (sortingPlayerTypeElse)
                                {
                                    IDictionary iData2 = data["data"][i] as IDictionary;
                                    int tempI = i;

                                    TournamentSelected ts = new TournamentSelected();
                                    ts.id = data["data"][i]["id"].ToString();
                                    if (iData2.Contains("registrationId"))
                                        ts.registrationId = data["data"][i]["registrationId"].ToString();
                                    ts.gameTypeId = data["data"][i]["gameTypeId"].ToString();
                                    //ts.duration = data["data"][i]["duration"].ToString();
                                    ts.playerSize = data["data"][i]["playerSize"].ToString();
                                    ts.winningAmount = data["data"][i]["winningAmount"].ToString();
                                    ts.entryFee = data["data"][i]["entryFee"].ToString();
                                    ts.status = data["data"][i]["status"].ToString();
                                    if (iData2.Contains("winnerId"))
                                        ts.winnerId = data["data"][i]["winnerId"].ToString();
                                    ts.scheduledDate = data["data"][i]["scheduledDate"].ToString();
                                    ts.title = data["data"][i]["title"].ToString();
                                    ts.playerType = data["data"][i]["playerType"].ToString();

                                    GameObject tournamentsObj = Instantiate(tournyItem, joinedContent);
                                    TournamentsItem ti = tournamentsObj.GetComponent<TournamentsItem>();
                                    ti.id = ts.id;
                                    ti.tournyNameTMP.text = ts.title;
                                    ti.playersTMP.text = ts.playerType + " PLAYERS";
                                    //ti.durationTMP.text = ts.duration;
                                    ti.playersOutOfTMP.text = "0/" + ts.playerSize;
                                    ti.amountTMP.text = "₹" + ts.winningAmount;
                                    ti.enterAmountTMP.text = "ENTRY AMOUNT ₹" + ts.entryFee;
                                    ti.gameTypeId = ts.gameTypeId;
                                    ti.startsTMP.text = "";
                                    ti.callFrom = "registered";

                                    TMP_Text regBtnTxt = ti.regButtonTMP;
                                    Image regBtnImageTemp = ti.regButtonImage;
                                    Button regBtnTemp = ti.regButton;
                                    string status = data["data"][i]["status"].ToString();
                                    string registrationId = data["data"][i]["registrationId"].ToString();

                                    if (status == "0")
                                    {
                                        regBtnTxt.text = "Register";
                                        regBtnImageTemp.sprite = registerSprite;
                                        regBtnTemp.interactable = false;

                                        ti.startsLabel.text = "STARTS";
                                    }
                                    else if (status == "1")
                                    {
                                        regBtnTxt.text = "Join";
                                        regBtnImageTemp.sprite = joinSprite;

                                        ti.startsLabel.text = "RUNNING";

                                        regBtnTemp.onClick.AddListener(() =>
                                        {
                                            //if (L_SocketController.instance != null)
                                            //{

                                            if (iData2.Contains("registrationId"))
                                            {
                                                if (!string.IsNullOrEmpty(ts.registrationId))
                                                {
                                                    ShowWaitingScreen();

                                                    L_MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentJoin);
                                                    if (TournamentJoin.instance != null)
                                                    {
                                                        TournamentJoin.instance.SetDataFromTournamentList(ts);
                                                    }
                                                    //GlobalGameManager.instance.socketController.SendTournamentJoin(registrationId, tourneyId);
                                                }
                                                else
                                                {
                                                    Debug.Log("registrationId null");
                                                }
                                            }
                                            else
                                            {
                                                Debug.Log("registrationId not found");
                                            }
                                            //}
                                            //else
                                            //{
                                            //    Debug.Log("socketcontroller instace null");
                                            //}
                                        });
                                    }
                                    else if (status == "2")
                                    {
                                        regBtnTxt.text = "Finish";
                                        regBtnImageTemp.sprite = finishSprite;

                                        ti.startsLabel.text = "ENDED";
                                    }
                                    else if (status == "3")
                                    {
                                        regBtnTxt.text = "Cancel";
                                        regBtnImageTemp.sprite = finishSprite;

                                        ti.startsLabel.text = "CANCEL";
                                    }

                                    TMP_Text startsTMPTemp = ti.startsTMP;
                                    string scheduleDate = data["data"][i]["scheduledDate"].ToString();
                                    //string scheduleDate = "2023-04-18T10:25:00.000Z";
                                    DateTime scheduleDateConverted = Convert.ToDateTime(scheduleDate).ToLocalTime();
                                    DateTime today = Convert.ToDateTime(data["currentTime"].ToString()).ToLocalTime(); //DateTime.Now;
                                    TimeSpan differenceScheduleDate = scheduleDateConverted.Subtract(today);
                                    int totalSecondsScheduleDate = (int)differenceScheduleDate.TotalSeconds;
                                    //Debug.Log("tourneyId=" + ts.id + ", status=" + status + ", title=" + ti.tournyNameTMP.text + ", scheduleDateLocal=" + scheduleDateConverted
                                    //    + "\n scheduleDate=" + scheduleDate + ", totalSecondsScheduleDate=" + totalSecondsScheduleDate);

                                    if (totalSecondsScheduleDate != 0)
                                    {
                                        if (totalSecondsScheduleDate > 0)
                                        {
                                            ti.TimerStart(scheduleDateConverted, totalSecondsScheduleDate, startsTMPTemp, today);
                                        }
                                        else if (totalSecondsScheduleDate < 0)
                                        {
                                            //ti.startsLabel.text = "ENDED";
                                            //ti.startsTMP.text = "";
                                        }
                                    }

                                    //DateTime finalEndDateTime = scheduleDateConverted.AddMinutes(Double.Parse(data["data"][i]["duration"].ToString()));
                                    //TimeSpan differenceScheduleDateFinalEnd = finalEndDateTime.Subtract(today); //DateTime.Now
                                    //int totalSecondsScheduleDateFinalEnd = (int)differenceScheduleDateFinalEnd.TotalSeconds;

                                    //if (totalSecondsScheduleDateFinalEnd != 0)
                                    //{
                                    //    if (totalSecondsScheduleDateFinalEnd > 0)
                                    //    {
                                    //        ti.TimerStart(finalEndDateTime, totalSecondsScheduleDateFinalEnd, startsTMPTemp, today);
                                    //    }
                                    //    else if (totalSecondsScheduleDateFinalEnd < 0)
                                    //    {
                                    //        //ti.startsLabel.text = "ENDED";
                                    //        //ti.startsTMP.text = "";
                                    //    }
                                    //}
                                }
                            }

                            if (joinedContent.childCount == 0)
                                OnNoData();
                        }
                        else
                        {
                            OnNoData();
                        }
                    }
                    else if (iData1.Contains("errorMessage"))
                    {
                        Debug.Log("Error in Tournament Regestered List 2");
                    }
                    else
                    {
                        Debug.Log("Error in Tournament Regestered List 3");
                    }
                }
                else
                {
                    Debug.Log("Error in Tournament Regestered List 4");
                }
            }
        }));
    }



    void TournamentFinished(string subCategory = "all")
    {
        DestroyAllScrollItems();
        ShowFinishedScrollView();

        StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.TournamentFinished] + PlayerManager.instance.GetPlayerGameData().userId, (serverResponse, errorBool, error) =>
        {
            if (errorBool)
            {
                Debug.Log("Error in Tournament List 1: " + error);
            }
            else
            {
                Debug.Log("Finished Tournament response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("code"))
                {
                    if (data["code"].ToString() == "200")
                    {
                        if (data["data"].Count > 0)
                        {
                            noTournamentText.gameObject.SetActive(false);

                            for (int i = 0; i < data["data"].Count; i++)
                            {
                                bool sortingPlayerTypeElse = false;
                                if (subCategory == "2players")
                                {
                                    if (data["data"][i]["playerType"].ToString().Equals("4"))
                                        continue;
                                    else
                                        sortingPlayerTypeElse = true;
                                }
                                else if (subCategory == "4players")
                                {
                                    if (data["data"][i]["playerType"].ToString().Equals("2"))
                                        continue;
                                    else
                                        sortingPlayerTypeElse = true;
                                }
                                else
                                {
                                    sortingPlayerTypeElse = true;
                                }

                                if (sortingPlayerTypeElse)
                                {
                                    GameObject tournamentsObj = Instantiate(tournyItem, finishedContent);
                                    string tourneyId = data["data"][i]["id"].ToString();
                                    TournamentsItem ti = tournamentsObj.GetComponent<TournamentsItem>();
                                    ti.id = tourneyId;
                                    ti.tournyNameTMP.text = data["data"][i]["title"].ToString();
                                    ti.playersTMP.text = data["data"][i]["playerType"].ToString() + " PLAYERS";
                                    //ti.durationTMP.text = data["data"][i]["duration"].ToString() + " Mins";
                                    ti.playersOutOfTMP.text = "0/" + data["data"][i]["playerSize"].ToString();
                                    ti.amountTMP.text = "₹" + data["data"][i]["winningAmount"].ToString();
                                    ti.enterAmountTMP.text = "ENTRY AMOUNT ₹" + data["data"][i]["entryFee"].ToString();
                                    ti.callFrom = "finished";

                                    TMP_Text regBtnTxt = ti.regButtonTMP;
                                    Image regBtnImageTemp = ti.regButtonImage;
                                    Button regBtnTemp = ti.regButton;
                                    string status = data["data"][i]["status"].ToString();

                                    if (status == "0")
                                    {
                                        regBtnTxt.text = "Register";
                                        regBtnImageTemp.sprite = registerSprite;
                                    }
                                    else if (status == "1")
                                    {
                                        regBtnTxt.text = "Join";
                                        regBtnImageTemp.sprite = joinSprite;
                                    }
                                    else if (status == "2")
                                    {
                                        regBtnTxt.text = "Finished";
                                        regBtnImageTemp.sprite = finishSprite;
                                    }
                                    else if (status == "3")
                                    {
                                        regBtnTxt.text = "Cancel";
                                        regBtnImageTemp.sprite = finishSprite;
                                    }

                                    if (status == "2" || status == "3")
                                        ti.startsLabel.text = "ENDED";

                                    ti.startsTMP.text = "";
                                }
                            }

                            if (finishedContent.childCount == 0)
                                OnNoData();
                        }
                        else
                        {
                            OnNoData();
                        }
                    }
                    else if (iData1.Contains("errorMessage"))
                    {
                        Debug.Log("Error in Tournament Finished List 2");
                    }
                    else
                    {
                        Debug.Log("Error in Tournament Finished List 3");
                    }
                }
                else
                {
                    Debug.Log("Error in Tournament Finished List 4");
                }
            }
        }));
    }





    public void OnTournamentListStatusChange(string responseTxt)
    {
        //Debug.Log("data: responseTxt: " + responseTxt);
        JsonData data = JsonMapper.ToObject(responseTxt);

        Debug.Log("data: upcomming: " + data[0]["upcomming"].Count);
        if (upcomingToggle.isOn)
        {
            if (data[0]["upcomming"].Count > 0)
            {
                DestroyAllScrollItems();
                ShowUpcomingScrollView();
                noTournamentText.gameObject.SetActive(false);

                for (int i = 0; i < data[0]["upcomming"].Count; i++)
                {
                    bool sortingPlayerTypeElse = false;
                    if (players2Toggle.isOn == true)
                    {
                        if (data[0]["upcomming"][i]["playerType"].ToString().Equals("4"))
                            continue;
                        else
                            sortingPlayerTypeElse = true;
                    }
                    else if (players4Toggle.isOn == true)
                    {
                        if (data[0]["upcomming"][i]["playerType"].ToString().Equals("2"))
                            continue;
                        else
                            sortingPlayerTypeElse = true;
                    }
                    else
                    {
                        sortingPlayerTypeElse = true;
                    }

                    if (sortingPlayerTypeElse)
                    {
                        //Debug.Log("UPCOMING IF");
                        

                        GameObject tournamentsObj = Instantiate(tournyItem, upcomingContent);
                        string tourneyId = data[0]["upcomming"][i]["id"].ToString();
                        TournamentsItem ti = tournamentsObj.GetComponent<TournamentsItem>();
                        ti.id = tourneyId;
                        ti.tournyNameTMP.text = data[0]["upcomming"][i]["title"].ToString();
                        ti.playersTMP.text = data[0]["upcomming"][i]["playerType"].ToString() + " PLAYERS";
                        //ti.durationTMP.text = data["data"][i]["duration"].ToString() + " Mins";
                        ti.playersOutOfTMP.text = data[0]["upcomming"][i]["registered_users"].Count + "/" + data[0]["upcomming"][i]["playerSize"].ToString();
                        ti.amountTMP.text = "₹" + data[0]["upcomming"][i]["winningAmount"].ToString();
                        ti.enterAmountTMP.text = "ENTRY AMOUNT ₹" + data[0]["upcomming"][i]["entryFee"].ToString();
                        ti.startsTMP.text = "";
                        ti.callFrom = "upcoming";

                        TMP_Text regBtnTxt = ti.regButtonTMP;
                        Image regBtnImageTemp = ti.regButtonImage;
                        Button regBtnTemp = ti.regButton;
                        string status = data[0]["upcomming"][i]["status"].ToString();

                        if (status == "0")
                        {
                            regBtnTxt.text = "Register";
                            regBtnImageTemp.sprite = registerSprite;

                            ti.startsLabel.text = "STARTS";
                        }
                        else if (status == "1")
                        {
                            regBtnTxt.text = "Join";
                            regBtnImageTemp.sprite = joinSprite;

                            ti.startsLabel.text = "RUNNING";
                        }
                        else if (status == "2")
                        {
                            regBtnTxt.text = "Finished";
                            regBtnImageTemp.sprite = finishSprite;

                            ti.startsLabel.text = "ENDED";
                        }
                        else if (status == "3")
                        {
                            regBtnTxt.text = "Cancel";
                            regBtnImageTemp.sprite = finishSprite;

                            ti.startsLabel.text = "CANCEL";
                        }
                        //string myRegistrationId = string.Empty;
                        for (int j = 0; j < data[0]["upcomming"][i]["registered_users"].Count; j++)
                        {
                            if (data[0]["upcomming"][i]["registered_users"][j]["userId"].ToString().Equals(PlayerManager.instance.GetPlayerGameData().userId))
                            {
                                Debug.Log("myRegistrationId: " + data[0]["upcomming"][i]["registered_users"][j]["registrationId"].ToString());
                                //myRegistrationId = data[0]["upcomming"][i]["registered_users"][j]["registrationId"].ToString();
                                regBtnTemp.interactable = false;
                            }
                        }

                        TMP_Text startsTMPTemp = ti.startsTMP;
                        string scheduleDate = data[0]["upcomming"][i]["scheduledDate"].ToString();
                        DateTime scheduleDateConverted = Convert.ToDateTime(scheduleDate).ToLocalTime();
                        DateTime today = Convert.ToDateTime(data[0]["currentTime"].ToString()).ToLocalTime();
                        TimeSpan differenceScheduleDate = scheduleDateConverted.Subtract(today);
                        int totalSecondsScheduleDate = (int)differenceScheduleDate.TotalSeconds;
                        //Debug.Log("tourneyId=" + tourneyId + ", status=" + status + ", title=" + ti.tournyNameTMP.text + ", scheduleDateLocal=" + scheduleDateConverted
                        //    + "\n scheduleDate=" + scheduleDate + ", totalSecondsScheduleDate=" + totalSecondsScheduleDate);

                        if (totalSecondsScheduleDate != 0)
                        {
                            if (totalSecondsScheduleDate > 0)
                            {
                                ti.TimerStart(scheduleDateConverted, totalSecondsScheduleDate, startsTMPTemp, today);

                                if (status == "0")
                                {
                                    regBtnTemp.onClick.AddListener(() =>
                                    {
                                        Debug.Log("regBtn onclick tourneyId: " + tourneyId);
                                        ShowWaitingScreen();
                                        if (L_GlobalGameManager.instance.tournamentSocketController.enabled == false)
                                        {
                                            Debug.Log("socketcontroller 1 instace null");
                                            L_GlobalGameManager.instance.tournamentSocketController.enabled = true;
                                            L_GlobalGameManager.instance.tournamentSocketController.gameObject.SetActive(true);
                                        }
                                        L_GlobalGameManager.instance.tournamentSocketController.SendTournamentRegister(tourneyId);
                                    });
                                }
                            }
                        }

                    }
                }

                //if (upcomingContent.childCount == 0)
                //    OnNoData();
            }
            //else
            //{
            //    OnNoData();
            //}
        }

        Debug.Log("data: active: " + data[0]["active"].Count);
        if (joinedToggle.isOn)
        {
            if (data[0]["active"].Count > 0)
            {
                DestroyAllScrollItems();
                ShowJoinedScrollView();
                noTournamentText.gameObject.SetActive(false);

                for (int i = 0; i < data[0]["active"].Count; i++)
                {
                    bool sortingPlayerTypeElse = false;
                    if (players2Toggle.isOn == true)
                    {
                        if (data[0]["active"][i]["playerType"].ToString().Equals("4"))
                            continue;
                        else
                            sortingPlayerTypeElse = true;
                    }
                    else if (players4Toggle.isOn == true)
                    {
                        if (data[0]["active"][i]["playerType"].ToString().Equals("2"))
                            continue;
                        else
                            sortingPlayerTypeElse = true;
                    }
                    else
                    {
                        sortingPlayerTypeElse = true;
                    }

                    if (sortingPlayerTypeElse)
                    {
                        string myRegistrationId = string.Empty;
                        for (int j = 0; j < data[0]["active"][i]["registered_users"].Count; j++)
                        {
                            if (data[0]["active"][i]["registered_users"][j]["userId"].ToString().Equals(PlayerManager.instance.GetPlayerGameData().userId))
                            {
                                myRegistrationId = data[0]["active"][i]["registered_users"][j]["registrationId"].ToString();
                                //Debug.Log("myRegistrationId: " + myRegistrationId);


                                IDictionary iData2 = data[0]["active"][i] as IDictionary;
                                int tempI = i;

                                TournamentSelected ts = new TournamentSelected();
                                ts.id = data[0]["active"][i]["id"].ToString();
                                ts.gameTypeId = data[0]["active"][i]["gameTypeId"].ToString();
                                ts.playerSize = data[0]["active"][i]["playerSize"].ToString();
                                ts.winningAmount = data[0]["active"][i]["winningAmount"].ToString();
                                ts.entryFee = data[0]["active"][i]["entryFee"].ToString();
                                ts.status = data[0]["active"][i]["status"].ToString();
                                if (iData2.Contains("winnerId"))
                                    ts.winnerId = data[0]["active"][i]["winnerId"].ToString();
                                ts.scheduledDate = data[0]["active"][i]["scheduledDate"].ToString();
                                ts.title = data[0]["active"][i]["title"].ToString();
                                ts.playerType = data[0]["active"][i]["playerType"].ToString();
                                ts.registered_users_count = data[0]["active"][i]["registered_users"].Count;

                                GameObject tournamentsObj = Instantiate(tournyItem, joinedContent);
                                TournamentsItem ti = tournamentsObj.GetComponent<TournamentsItem>();
                                ti.id = ts.id;
                                ti.tournyNameTMP.text = ts.title;
                                ti.playersTMP.text = ts.playerType + " PLAYERS";
                                ti.playersOutOfTMP.text = data[0]["active"][i]["registered_users"].Count + "/" + ts.playerSize;
                                ti.amountTMP.text = "₹" + ts.winningAmount;
                                ti.enterAmountTMP.text = "ENTRY AMOUNT ₹" + ts.entryFee;
                                ti.gameTypeId = ts.gameTypeId;
                                ti.startsTMP.text = "";
                                ti.callFrom = "registered";

                                TMP_Text regBtnTxt = ti.regButtonTMP;
                                Image regBtnImageTemp = ti.regButtonImage;
                                Button regBtnTemp = ti.regButton;
                                string status = data[0]["active"][i]["status"].ToString();

                                ts.registrationId = myRegistrationId;
                                //string registrationId = string.Empty;
                                //for (int j = 0; j < data[0]["active"][i]["registered_users"].Count; j++)
                                //{
                                //    if (data[0]["active"][i]["registered_users"][j]["userId"].ToString().Equals(PlayerManager.instance.GetPlayerGameData().userId))
                                //    {
                                //        registrationId = data[0]["active"][i]["registered_users"][j]["registrationId"].ToString();
                                //        ts.registrationId = registrationId;
                                //    }
                                //}

                                if (status == "0")
                                {
                                    regBtnTxt.text = "Register";
                                    regBtnImageTemp.sprite = registerSprite;
                                    regBtnTemp.interactable = false;

                                    ti.startsLabel.text = "STARTS";
                                }
                                else if (status == "1")
                                {
                                    regBtnTxt.text = "Join";
                                    regBtnImageTemp.sprite = joinSprite;

                                    ti.startsLabel.text = "RUNNING";

                                    regBtnTemp.onClick.AddListener(() =>
                                    {
                                        if (!string.IsNullOrEmpty(ts.registrationId))
                                        {
                                            ShowWaitingScreen();

                                            L_MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentJoin);
                                            if (TournamentJoin.instance != null)
                                            {
                                                TournamentJoin.instance.SetDataFromTournamentList(ts);
                                            }
                                        }
                                        else
                                        {
                                            Debug.Log("registrationId null");
                                        }
                                    });
                                }
                                else if (status == "2")
                                {
                                    regBtnTxt.text = "Finish";
                                    regBtnImageTemp.sprite = finishSprite;

                                    ti.startsLabel.text = "ENDED";
                                }
                                else if (status == "3")
                                {
                                    regBtnTxt.text = "Cancel";
                                    regBtnImageTemp.sprite = finishSprite;

                                    ti.startsLabel.text = "CANCEL";
                                }

                                TMP_Text startsTMPTemp = ti.startsTMP;
                                string scheduleDate = data[0]["active"][i]["scheduledDate"].ToString();
                                DateTime scheduleDateConverted = Convert.ToDateTime(scheduleDate).ToLocalTime();
                                DateTime today = Convert.ToDateTime(data[0]["currentTime"].ToString()).ToLocalTime(); //DateTime.Now;
                                TimeSpan differenceScheduleDate = scheduleDateConverted.Subtract(today);
                                int totalSecondsScheduleDate = (int)differenceScheduleDate.TotalSeconds;
                                //Debug.Log("tourneyId=" + ts.id + ", status=" + status + ", title=" + ti.tournyNameTMP.text + ", scheduleDateLocal=" + scheduleDateConverted
                                //    + "\n scheduleDate=" + scheduleDate + ", totalSecondsScheduleDate=" + totalSecondsScheduleDate);

                                if (totalSecondsScheduleDate != 0)
                                {
                                    if (totalSecondsScheduleDate > 0)
                                    {
                                        ti.TimerStart(scheduleDateConverted, totalSecondsScheduleDate, startsTMPTemp, today);
                                    }
                                }
                            }
                        }
                    }

                }

                //if (joinedContent.childCount == 0)
                //    OnNoData();
            }
            //else
            //{
            //    OnNoData();
            //}
        }

        Debug.Log("data: finished: " + data[0]["finished"].Count);
        if (finishedToggle.isOn)
        {
            if (data[0]["finished"].Count > 0)
            {
                DestroyAllScrollItems();
                ShowFinishedScrollView();
                noTournamentText.gameObject.SetActive(false);

                for (int i = 0; i < data[0]["finished"].Count; i++)
                {
                    bool sortingPlayerTypeElse = false;
                    if (players2Toggle.isOn == true)
                    {
                        if (data[0]["finished"][i]["playerType"].ToString().Equals("4"))
                            continue;
                        else
                            sortingPlayerTypeElse = true;
                    }
                    else if (players4Toggle.isOn == true)
                    {
                        if (data[0]["finished"][i]["playerType"].ToString().Equals("2"))
                            continue;
                        else
                            sortingPlayerTypeElse = true;
                    }
                    else
                    {
                        sortingPlayerTypeElse = true;
                    }

                    if (sortingPlayerTypeElse)
                    {
                        //string myRegistrationId = string.Empty;
                        for (int j = 0; j < data[0]["finished"][i]["registered_users"].Count; j++)
                        {
                            if (data[0]["finished"][i]["registered_users"][j]["userId"].ToString().Equals(PlayerManager.instance.GetPlayerGameData().userId))
                            {
                                //myRegistrationId = data[0]["finished"][i]["registered_users"][j]["registrationId"].ToString();
                                //Debug.Log("myRegistrationId: " + myRegistrationId);

                                GameObject tournamentsObj = Instantiate(tournyItem, finishedContent);
                                string tourneyId = data[0]["finished"][i]["id"].ToString();
                                TournamentsItem ti = tournamentsObj.GetComponent<TournamentsItem>();
                                ti.id = tourneyId;
                                ti.tournyNameTMP.text = data[0]["finished"][i]["title"].ToString();
                                ti.playersTMP.text = data[0]["finished"][i]["playerType"].ToString() + " PLAYERS";
                                //ti.durationTMP.text = data["data"][i]["duration"].ToString() + " Mins";
                                ti.playersOutOfTMP.text = data[0]["finished"][i]["registered_users"].Count + "/" + data[0]["finished"][i]["playerSize"].ToString();
                                ti.amountTMP.text = "₹" + data[0]["finished"][i]["winningAmount"].ToString();
                                ti.enterAmountTMP.text = "ENTRY AMOUNT ₹" + data[0]["finished"][i]["entryFee"].ToString();
                                ti.callFrom = "finished";

                                TMP_Text regBtnTxt = ti.regButtonTMP;
                                Image regBtnImageTemp = ti.regButtonImage;
                                Button regBtnTemp = ti.regButton;
                                string status = data[0]["finished"][i]["status"].ToString();

                                if (status == "0")
                                {
                                    regBtnTxt.text = "Register";
                                    regBtnImageTemp.sprite = registerSprite;
                                }
                                else if (status == "1")
                                {
                                    regBtnTxt.text = "Join";
                                    regBtnImageTemp.sprite = joinSprite;
                                }
                                else if (status == "2")
                                {
                                    regBtnTxt.text = "Finished";
                                    regBtnImageTemp.sprite = finishSprite;
                                }
                                else if (status == "3")
                                {
                                    regBtnTxt.text = "Cancel";
                                    regBtnImageTemp.sprite = finishSprite;
                                }

                                if (status == "2" || status == "3")
                                    ti.startsLabel.text = "ENDED";

                                ti.startsTMP.text = "";
                            }
                        }
                    }
                }

                //if (finishedContent.childCount == 0)
                //    OnNoData();
            }
            //else
            //{
            //    OnNoData();
            //}
        }
    }

    

    void OnNoData()
    {
        noTournamentText.gameObject.SetActive(true);
        noTournamentText.text = "No Data";
    }



    private void OnDestroy()
    {
        if (L_SocketController.instance != null)
        {
            //GlobalGameManager.instance.socketController.isFromTournament = false;
            L_GlobalGameManager.instance.socketController.SocketClose();
            //GlobalGameManager.instance.socketController.SetSocketState(L_SocketState.Connecting);
            L_GlobalGameManager.instance.socketController.enabled = false;
            L_GlobalGameManager.instance.socketController.gameObject.SetActive(false);
        }
    }
}
