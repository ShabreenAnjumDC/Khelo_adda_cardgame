using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Lobby_Texas : MonoBehaviour
{
    public static P_Lobby_Texas instance;

    public Text heading, blindsText, playerCountText, minBuyInText;
    public Button bgButton;

    private void Awake()
    {
        instance = this;
    }
}
