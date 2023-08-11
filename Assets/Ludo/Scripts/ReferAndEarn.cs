using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferAndEarn : MonoBehaviour
{
    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();
        switch (buttonName)
        {
            case "close":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                break;

            case "invite":
                Debug.Log("Invite");
#if UNITY_ANDROID || UNITY_IOS
                new NativeShare().SetText("H56oWqNH").Share();
#endif
                break;

            case "enter_referral":
                Debug.Log("Enter Referral");
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.ReferralInput);
                break;
        }
    }
}
