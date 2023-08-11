using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYC : MonoBehaviour
{
    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "close":
                L_MainMenuController.instance.DestroyScreen(MainMenuScreens.KYC);
                break;

            case "submit":
                Debug.Log("submit");
                break;
        }
    }
}
