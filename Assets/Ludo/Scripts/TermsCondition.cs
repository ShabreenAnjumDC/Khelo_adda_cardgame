using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TermsCondition : MonoBehaviour
{
    public void OnClickClose()
    {
        L_MainMenuController.instance.PlayButtonSound();
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.Login_SignUp_Otp);
    }

    //void Update()
    //{
    //    OnEscape();
    //}

    //void OnEscape()
    //{
    //    if (Input.GetKeyDown("escape"))
    //    {
    //        CloseTermsNCondition();
    //    }
    //}

    //public void CloseTermsNCondition()
    //{
    //    if (MainMenuController.instance.IsScreenActive(MainMenuScreens.Login_SignUp_Otp))
    //    {
    //        MainMenuController.instance.DestroyScreen(MainMenuScreens.TermsNCondition);
    //    }
    //}
}
