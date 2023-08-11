using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Lobby_Second_Texas : MonoBehaviour
{
    public static P_Lobby_Second_Texas instance;

    public Text headingText, avgStackText, playersText;
    public GameObject anonymousObject;
    public Button bgButton;

    void Awake()
    {
        instance = this;
    }
}
