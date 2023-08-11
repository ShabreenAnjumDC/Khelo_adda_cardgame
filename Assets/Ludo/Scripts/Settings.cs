using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    //public static Settings instance;

    public Toggle notificationToggle;
    public Toggle soundToggle;
    public Toggle vibrationToggle;

    void Start()
    {
        if (L_SoundManager.instance.isNotification)
            notificationToggle.isOn = true;
        else
            notificationToggle.isOn = false;

        if (L_SoundManager.instance.isSound)
            soundToggle.isOn = true;
        else
            soundToggle.isOn = false;

        if (L_SoundManager.instance.isVibrate)
            vibrationToggle.isOn = true;
        else
            vibrationToggle.isOn = false;
    }

    public void OnNotificationToggleChange()
    {
        L_MainMenuController.instance.PlayButtonSound();
        L_SoundManager.instance.SetNotificationSetting(notificationToggle.isOn);
    }

    public void OnSoundToggleChange()
    {
        if (soundToggle.isOn)
        {
            L_SoundManager.instance.PlaySound(L_SoundType.ButtonSound, L_MainMenuController.instance.transform);
            L_SoundManager.instance.PlayLoopSound(L_SoundType.MainAppSound, L_MainMenuController.instance.transform);
        }
        else if (!soundToggle.isOn)
        {
            L_SoundManager.instance.StopLoopSound(L_SoundType.MainAppSound, L_MainMenuController.instance.transform);
        }

        L_SoundManager.instance.SetSoundSetting(soundToggle.isOn);
    }

    public void OnVibrationToggleChange()
    {
        if (vibrationToggle.isOn)
            L_SoundManager.instance.PlayVibrate();

        L_MainMenuController.instance.PlayButtonSound();
        L_SoundManager.instance.SetVibrateSetting(vibrationToggle.isOn);
    }

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "close":
                //MainMenuController.instance.DestroyScreen(MainMenuScreens.Profile);
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                break;

            case "helpdesk":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.HelpDesk);
                break;

            case "about_us":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.AboutUs);
                break;

            case "privacy_policy":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.PrivacyPolicy);
                break;

            case "terms_and_condition":
                //MainMenuController.instance.ShowScreen(MainMenuScreens.TermsNCondition);
                break;

            case "refund_and_cancellation":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.RefundNCancellations);
                break;

            case "logout":
                L_GlobalGameManager.playerToken = "";
                L_PlayerManager.instance.DeletePlayerGameData();
                L_SoundManager.instance.SetSoundSetting(true);
                L_SoundManager.instance.SetVibrateSetting(true);
                L_SoundManager.instance.SetNotificationSetting(true);
                L_MainMenuController.instance.DestroyScreen(MainMenuScreens.Settings);
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Login_SignUp_Otp);
                break;

            case "quit":
                Debug.Log("APP QUIT");
                Application.Quit();
                //MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                break;
        }
    }
}
