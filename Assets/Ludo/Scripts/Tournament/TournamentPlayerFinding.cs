using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class TournamentPlayerFinding : MonoBehaviour
{
    public static TournamentPlayerFinding instance;

    public TMP_Text tournamentTitle;
    public TMP_Text priceMoney;
    public TMP_Text playerType;
    public TMP_Text enterAmount;
    public TMP_Text roomCode;
    public TMP_Text selfName;
    public TMP_Text opponentName;
    public TMP_Text timer1;
    public TMP_Text timer2;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        selfName.text = PlayerManager.instance.GetPlayerGameData().fullName;
    }

    public void OnClickOnCloseButton()
    {
        L_MainMenuController.instance.PlayButtonSound();
        //MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentJoin);
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.QuitConfirm);
        if (QuitConfirm.instance != null)
            QuitConfirm.instance.isQuitApp = false;
    }

    public void OnGameStart(string unixTimestampStr)
    {
        roomCode.text = "ROOM CODE : " + unixTimestampStr;
        int parseUnixTimeStamp = 0;
        if (Int32.TryParse(unixTimestampStr, out parseUnixTimeStamp))
        {
            DateTimeOffset dtOffset = DateTimeOffset.FromUnixTimeSeconds(parseUnixTimeStamp);
        }
    }
}
