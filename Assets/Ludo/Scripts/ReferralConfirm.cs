using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferralConfirm : MonoBehaviour
{
    public void OnClickYes()
    {
        L_MainMenuController.instance.PlayButtonSound();
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.ReferralInput);
    }
    public void OnClickNo()
    {
        L_MainMenuController.instance.PlayButtonSound();
        L_MainMenuController.instance.DestroyScreen(MainMenuScreens.ReferralConfirm);
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
    }
}
