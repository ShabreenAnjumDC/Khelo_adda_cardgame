using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TournamentNextRound : MonoBehaviour
{
    public static TournamentNextRound instance;

    public TMP_Text wowTMP, nextRountTMP, timerTMP, roundNoTMP, durationTMP, playersTMP, tournamentNameTMP, playersTypeTMP, winAmountTMP;
    public GameObject nextRoundLbl, line1;
    public Button homeButton;

    void Awake()
    {
        Debug.Log("next round instant awake()");
        instance = this;
        if (instance != null) { Debug.Log("next round instant NOT NULL"); }
        else { Debug.Log("next round instant NULL"); }
    }

    public void OnHomeButtonClick()
    {
        //if (L_SocketController.instance != null)
        //{
        //    L_SocketController.instance.RemovePlayerFromGame();
        //}
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.Tournaments);
    }

    public void OnNextButtonClick()
    {
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.TournamentWinning);
    }

    public void SetWinData(TournamentSelected ts)
    {
        wowTMP.text = "WOW!!!";
        nextRountTMP.text = "YOU ARE QUALIFIED\nTO THE NEXT ROUND";
        //roundNoTMP.text = "Next Round";
        //durationTMP.text = ts.duration + " Mins";
        //playersTMP.text = "1/" + ts.playerSize;
        tournamentNameTMP.text = ts.title;
        playersTypeTMP.text = ts.playerType + " PLAYERS";
        winAmountTMP.text = "₹" + ts.winningAmount;
        //homeButton.interactable = false;
    }

    public void SetLoseData(TournamentSelected ts)
    {
        wowTMP.text = "OOPS!!!";
        nextRountTMP.text = "YOU ARE DISQUALIFIED\nTO THE NEXT ROUND";
        //durationTMP.text = ts.duration + " Mins";
        //playersTMP.text = "1/" + ts.playerSize;
        tournamentNameTMP.text = ts.title;
        playersTypeTMP.text = ts.playerType + " PLAYERS";
        winAmountTMP.text = "₹" + ts.winningAmount;
        //homeButton.interactable = true;

        //timerTMP.transform.parent.gameObject.SetActive(false);
        //roundNoTMP.gameObject.SetActive(false);
        //nextRoundLbl.SetActive(false);
        //line1.SetActive(false);
    }
}
