using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferralInput : MonoBehaviour
{
    public InputField referralCodeInput;

    public void OnClickSubmit()
    {
        L_MainMenuController.instance.PlayButtonSound();
        //MainMenuController.instance.ShowScreen(MainMenuScreens.ReferralHistory);
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.ReferAndEarn);
    }
    public void OnClickSkip()
    {
        L_MainMenuController.instance.PlayButtonSound();
        //MainMenuController.instance.DestroyScreen(MainMenuScreens.ReferralInput);
        //MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.ReferAndEarn);
    }
}
