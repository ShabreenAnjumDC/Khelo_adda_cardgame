using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentWinLose : MonoBehaviour
{
    public static TournamentWinLose instance;

    public TMP_Text tournamentNameTMP, playersTypeTMP, firstPrizeTMP, winAmountTMP;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (T_SocketController.instance != null && T_SocketController.instance.tournamentSelected != null)
        {
            TournamentSelected ts = T_SocketController.instance.tournamentSelected;
            tournamentNameTMP.text = ts.title;
            playersTypeTMP.text = ts.playerType + " PLAYER";
            winAmountTMP.text = "₹" + ts.winningAmount;
        }
    }

    public void OnHomeButtonClick()
    {
        //if (T_SocketController.instance != null)
        //{
        //    T_SocketController.instance.RemovePlayerFromGame();
        //}
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.Tournaments);
    }
}
