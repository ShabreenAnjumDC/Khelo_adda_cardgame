using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferralHistory : MonoBehaviour
{
    public void OnClickClose()
    {
        L_MainMenuController.instance.PlayButtonSound();

        L_MainMenuController.instance.DestroyScreen(MainMenuScreens.ReferralHistory);
    }
}
