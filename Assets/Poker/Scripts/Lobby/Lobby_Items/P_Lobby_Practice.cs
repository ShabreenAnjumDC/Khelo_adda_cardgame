using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Lobby_Practice : MonoBehaviour
{
    public static P_Lobby_Practice instance;

    public Text blindsText, playerCountText, freeText;
    public Image freeImage;
    public Button bgButton;

    private void Awake()
    {
        instance = this;
    }
}
