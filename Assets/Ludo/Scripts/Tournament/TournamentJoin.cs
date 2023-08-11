using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class TournamentJoin : MonoBehaviour
{
    public static TournamentJoin instance;

    public GameObject joinBtnparent;
    public GameObject joinSuccessparent;
    public ToggleGroup gotiColorToggleGroup;
    public Button joinButton;
    public TMP_Text errorTMP;

    public TMP_Text tournamentNameTMP, playersTypeTMP, winAmountTMP, startsTMP, durationTMP, playersTMP, entryAmountTMP, walletAmountTMP;

    public TournamentSelected tournamentDataFromJoin;

    void Awake()
    {
        instance = this;
    }

    public void SetDataFromTournamentList(TournamentSelected ts)
    {
        tournamentDataFromJoin = ts;

        tournamentNameTMP.text = ts.title;
        playersTypeTMP.text = ts.playerType + " PLAYERS";
        winAmountTMP.text = "₹" + ts.winningAmount;
        //startsTMP.text = ts.scheduledDate;
        //durationTMP.text = ts.duration + " Mins";
        playersTMP.text = ts.registered_users_count + "/" + ts.playerSize;
        entryAmountTMP.text = "₹" + ts.entryFee;
        //walletAmountTMP.text = PlayerManager.instance.
    }

    public void OnClickOnButton(string buttonName)
    {
        switch(buttonName)
        {
            case "join":
                //joinBtnparent.SetActive(false);
                //joinSuccessparent.SetActive(true);
                joinButton.interactable = false;

                Toggle toggleGotiColor = GetSelectedToggle(gotiColorToggleGroup);

                if (!string.IsNullOrEmpty(tournamentDataFromJoin.registrationId) && !string.IsNullOrEmpty(tournamentDataFromJoin.id) 
                    && !string.IsNullOrEmpty(tournamentDataFromJoin.winningAmount) && !string.IsNullOrEmpty(tournamentDataFromJoin.gameTypeId))
                {

                    //GlobalGameManager.instance.tournamentSocketController.isFromTournament = true;
                    if (L_GlobalGameManager.instance.tournamentSocketController.enabled == false)
                    {
                        L_GlobalGameManager.instance.tournamentSocketController.enabled = true;
                        L_GlobalGameManager.instance.tournamentSocketController.gameObject.SetActive(true);
                    }

                    T_SocketController.instance.selectedGotiColor = toggleGotiColor.name;
                    T_SocketController.instance.tournamentSelected = tournamentDataFromJoin;

                    if (tournamentDataFromJoin.gameTypeId == "1")
                    {
                        T_SocketController.instance.isQuickLudo = false;
                        T_SocketController.instance.isClassicLudo = true;
                    }
                    else if (tournamentDataFromJoin.gameTypeId == "2")
                    {
                        T_SocketController.instance.isClassicLudo = false;
                        T_SocketController.instance.isQuickLudo = true;
                    }

                    T_SocketController.instance.gameAmount = tournamentDataFromJoin.winningAmount;
                    T_SocketController.instance.gameTypeId = tournamentDataFromJoin.gameTypeId;
                    //T_SocketController.instance.gameVarientId = gameVarientId;

                    L_GlobalGameManager.instance.tournamentSocketController.SendTournamentJoin(tournamentDataFromJoin.registrationId, tournamentDataFromJoin.id);
                    
                    L_GlobalGameManager.instance.currentScreenPathList.Add("Tournament Join");
                }

                break;

            case "join_success":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentPlayerFinding);
                break;

            case "rules":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Rules);
                break;

            case "back":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Tournaments);
                break;
        }
    }

    public void GameErrorReturn(string response)
    {
        Debug.Log("Game Error: " + response);
        JsonData data = JsonMapper.ToObject(response);
        IDictionary iData = data[0] as IDictionary;
        
        if (iData.Contains("message"))
        {
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, data[0]["message"].ToString(), "red", 2.5f));
        }
        joinButton.interactable = true;
    }

    public void OnPlayerAddedReceived()
    {
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentPlayerFinding);
        TournamentPlayerFinding.instance.tournamentTitle.text = tournamentDataFromJoin.title;
        TournamentPlayerFinding.instance.priceMoney.text = "₹" + tournamentDataFromJoin.winningAmount;
        TournamentPlayerFinding.instance.enterAmount.text = "₹" + tournamentDataFromJoin.entryFee;
        TournamentPlayerFinding.instance.playerType.text = tournamentDataFromJoin.playerType + " PLAYERS";
    }

    Toggle GetSelectedToggle(ToggleGroup tg)
    {
        Toggle[] toggles = tg.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles)
            if (t.isOn) return t;  //returns selected toggle
        return null;           // if nothing is selected return null
    }
}
