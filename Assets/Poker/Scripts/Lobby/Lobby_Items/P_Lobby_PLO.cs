using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Lobby_PLO : MonoBehaviour
{
    public static P_Lobby_PLO instance;

    public Text heading, titleText, entryText, trophyAmountText, bagAmountText, startsText, playersText, firstAmountText, statusText;
    public Image playerLineImage, statusImage;
    public Button bgButton;

    private void Awake()
    {
        instance = this;
    }
}
