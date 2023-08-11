using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "close":
                //MainMenuController.instance.DestroyScreen(MainMenuScreens.Wallet);
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                break;

            case "add_amount":
                Debug.Log("Add Amount");
                break;

            case "withdraw":
                Debug.Log("Withdraw");
                break;

            case "coupons":
                Debug.Log("Coupons");
                break;

            case "transaction_history":
                Debug.Log("Transaction History");
                break;

        }
    }
}
