using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerFinding : MonoBehaviour
{
    public static PlayerFinding instance;

    public TMP_Text priceMoney;  //now priceMoney set as enter amount (entry fees)
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

    void Update()
    {
        //OnEscape();
    }

    void OnEscape()
    {
        if (Input.GetKeyDown("escape"))
        {
            L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
        }
    }

    public void OnClickPanel()
    {
        string lastPath = L_GlobalGameManager.instance.currentScreenPathList.Last();

        if (lastPath == "ClassicLudoSelection")
        {
            //MainMenuController.instance.ShowScreen(MainMenuScreens.ClassicLudoGamePlay);
            //GlobalGameManager.instance.currentScreenPathList.Clear();
        }
        else if (lastPath == "QuickLudoSelection")
        {
            //MainMenuController.instance.ShowScreen(MainMenuScreens.Winner);
            //GlobalGameManager.instance.currentScreenPathList.Clear();
        }
    }

    public void OnClickBackButton()
    {
        L_MainMenuController.instance.PlayButtonSound();

        L_MainMenuController.instance.ShowScreen(MainMenuScreens.QuitConfirm);
        if (QuitConfirm.instance != null)
            QuitConfirm.instance.isQuitApp = false;

        //if (L_SocketController.instance != null)
        //{
        //    if (L_SocketController.instance.IsSocketOpen())
        //    {
        //        if (L_SocketController.instance.isRegisterSend)
        //        {
        //            L_SocketController.instance.RemovePlayerFromGame();
        //        }
        //    }
        //}
    }

    public void OnGameObjectFirstTime(string opponentNameData)
    {
        opponentName.text = opponentNameData;

        //JsonData data = JsonMapper.ToObject(responseText);
        
        //IDictionary idata = data as IDictionary;
        //if (idata.Contains("players"))
        //{
            //Debug.Log("player count=" + data["players"].Count);
            //for (int i = 0; i < data["players"].Count; i++)
            //{
            //    Debug.Log("player i=" + i + ", id=" + data["players"]["playerId"] + ", name=" + data["players"]["playerName"]);
            //}
        //}
    }

    public void OnGameStart(string unixTimestampStr)
    {
        Debug.Log("on game start data="+ unixTimestampStr);
        roomCode.text = "ROOM CODE : " + unixTimestampStr;
        int parseUnixTimeStamp = 0;
        if (Int32.TryParse(unixTimestampStr, out parseUnixTimeStamp))
        {
            DateTimeOffset dtOffset = DateTimeOffset.FromUnixTimeSeconds(parseUnixTimeStamp);
            Debug.Log("datetime="+dtOffset.DateTime);
            Debug.Log("seconds="+dtOffset.Second);
        }
    }
}
