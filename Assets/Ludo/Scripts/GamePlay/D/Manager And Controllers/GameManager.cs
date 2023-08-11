using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Players[] redPlayers, yellowPlayers, greenPlayers, bluePlayers;
    public Players[] allPlayers;

    public DiceManager rollingDice;
    public DiceManager[] manageRollingDice;
    public GameObject[] manageHighlightPanel;

    public AudioSource invalidMoveAudio;

    void Awake()
    {
        instance = this;
    }

    // method use for hide options of red tokens
    public void HideOptionsRedPlayers()
    {
        for (int i = 0; i < redPlayers.Length; i++) //4
        {
            int tempI = i;
            for (int j = 0; j < redPlayers[tempI].DiceValue6Options1.Length; j++)  //3
            {
                int tempJ = j;
                redPlayers[i].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(false);
            }
            redPlayers[tempI].tokenOptionForMove.SetActive(false);
        }
    }

    // method use for hide options of yellow tokens
    public void HideOptionsYellowPlayers()
    {
        for (int i = 0; i < yellowPlayers.Length; i++) //4
        {
            int tempI = i;
            for (int j = 0; j < yellowPlayers[tempI].DiceValue6Options1.Length; j++)  //3
            {
                int tempJ = j;
                yellowPlayers[i].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(false);
            }
            yellowPlayers[tempI].tokenOptionForMove.SetActive(false);
        }
    }

    // method use for hide options of green tokens
    public void HideOptionsGreenPlayers()
    {
        for (int i = 0; i < greenPlayers.Length; i++) //4
        {
            int tempI = i;
            for (int j = 0; j < greenPlayers[tempI].DiceValue6Options1.Length; j++)  //3
            {
                int tempJ = j;
                greenPlayers[i].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(false);
            }
            greenPlayers[tempI].tokenOptionForMove.SetActive(false);
        }
    }

    // method use for hide options of blue tokens
    public void HideOptionsBluePlayers()
    {
        for (int i = 0; i < bluePlayers.Length; i++) //4
        {
            int tempI = i;
            for (int j = 0; j < bluePlayers[tempI].DiceValue6Options1.Length; j++)  //3
            {
                int tempJ = j;
                bluePlayers[i].DiceValue6Options1[tempJ].transform.parent.gameObject.SetActive(false);
            }
            bluePlayers[tempI].tokenOptionForMove.SetActive(false);
        }
    }
   
}