using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

public class NameUpdate : MonoBehaviour
{
    public static NameUpdate instance;

    public enum UpdateOptions
    {
        UsernameUpdate,
        NameUpdate,
        EmailUpdate
    }

    public UpdateOptions options = UpdateOptions.NameUpdate;

    public InputField nameInput;
    public TMP_Text errorText;

    void Awake()
    {
        instance = this;
    }

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "close":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                break;

            case "update":

                break;
        }
    }

    public void OnPutUser()
    {

    }
}