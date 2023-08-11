using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinnerLooser : MonoBehaviour
{
    public static WinnerLooser instance;

    public GameObject congratulationText;
    public TMP_Text youWonTMP;
    public TMP_Text amountTMP;

    void Awake()
    {
        instance = this;
    }

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "rate_us":
                Application.OpenURL("https://play.google.com/store/games");
                break;
            case "play_again":
                if (L_SocketController.instance.isClassicLudo == true || L_SocketController.instance.isQuickLudo == true)
                {
                    if (L_SocketController.instance != null)
                    {
                        L_SocketController.instance.RemovePlayerFromGame();
                    }
                }
                else
                {
                    L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                }
                break;
            case "home":
                if (L_SocketController.instance != null)
                {
                    L_SocketController.instance.RemovePlayerFromGame();
                }
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                break;
        }
    }

    public void SelfWinner(bool isSelfWinner, string amount)
    {
        if (isSelfWinner)
        {
            congratulationText.SetActive(true);
            youWonTMP.text = "You Won";
        }
        else
        {
            congratulationText.SetActive(false);
            youWonTMP.text = "You Lose";
        }

        amountTMP.text = "₹" + amount;
    }
}
