using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class T_Panel_Controller : MonoBehaviour
{
    public static T_Panel_Controller instance;

    [Header("-----Parents-----")]
    public GameObject ChatPanel;

    [Header("-----Players Info-----")]
    public GameObject[] allUsersPanel;
    public TMP_Text[] allPlayersName;

    [Header("-----dice when 6 dice images-----")]
    public Image[] redUserDiceWhen6;
    public Image[] yellowUserDiceWhen6;
    public Image[] greenUserDiceWhen6;
    public Image[] blueUserDiceWhen6;


    [Space(10)]
    public Text roomCodeText;
    public TMP_Text tempInfoTMP;
    public TMP_Text winAmountTMP;
    public Text gameTypeText; // CLASSIC or QUICK

    [Header("-----for back button-----")]
    public GameObject hidePanelWhenBack;
    public Button backButton;

    [Header("-----timer reference-----")]
    public Image[] timerImages;
    public TMP_Text[] timerTexts;

    [Header("-----animator reference-----")]
    public Animator[] highlightPanelAnims;

    [Header("-----idle time chance-----")]
    public T_IdleTimerDotsReference idleTimeDots;

    [Space(10)]
    public TMP_Text selfPanelMsgText;

    [Space(10)]
    public Transform boardImage;
    public Transform boardParent;
    public Sprite redPlayersSprite, yellowPlayersSprite, greenPlayersSprite, bluePlayersSprite;

    [Space(10)]
    public Button selfTurnSkipBtn;

    [Space(10)]
    public Coroutine gameTimerCo;
    public Text scoreText;
    public TMP_Text[] scoreTextArr;
    public Text gameTimerText;

    Coroutine diceTimerCo;

    void Awake()
    {
        instance = this;
    }

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "chat_panel":
                ChatPanel.SetActive(true);
                break;

            case "back":
                backButton.interactable = false;
                hidePanelWhenBack.SetActive(true);
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.QuitConfirm);
                if (QuitConfirm.instance != null)
                    QuitConfirm.instance.isQuitApp = false;
                break;

            case "skip_turn_self":
                T_SocketController.instance.SendSkipTurn();
                selfTurnSkipBtn.interactable = false;
                break;

            case "rules":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Rules);
                break;
        }
    }

    // method use for show tokens first time
    public void ShowPlayers(T_Players[] playerPiece)
    {
        for (int i = 0; i < playerPiece.Length; i++)
        {
            playerPiece[i].gameObject.SetActive(true);
        }
    }

    // method use for hide tokens when leave player from backend (status=1)
    public void HidePlayers(T_Players[] playerPiece)
    {
        for (int i = 0; i < playerPiece.Length; i++)
        {
            playerPiece[i].gameObject.SetActive(false);
        }
    }

    // show dice
    public void ShowDice(int playerTurn)
    {
        for (int i = 0; i < T_GameManager.instance.manageRollingDice.Length; i++)
        {
            if (i == playerTurn)
            {
                T_GameManager.instance.manageRollingDice[playerTurn].gameObject.SetActive(true);
                T_GameManager.instance.manageRollingDice[playerTurn].GetComponent<Button>().interactable = true;
            }
            else
                T_GameManager.instance.manageRollingDice[i].gameObject.SetActive(false);
        }
        StopIdleTimerFunc();
        ToeknAnimOff();
    }

    public void ShowBackButtonAndPanel()
    {
        backButton.interactable = true;
        hidePanelWhenBack.SetActive(false);
    }

    // method use to start coroutine for player turn timer (playerTurn is swapIndex)
    public void IdleTimerFunc(int playerTurn)
    {
        //StopCoroutine(diceTimerCo);
        diceTimerCo = StartCoroutine(IdleTimer(playerTurn, T_SocketController.instance.idleTimeout));
    }

    // method use for stop player turn timer
    public void StopIdleTimerFunc()
    {
        if (diceTimerCo != null)
        {
            StopCoroutine(diceTimerCo);

            for (int i = 0; i < timerImages.Length; i++)
            {
                timerImages[i].fillAmount = 0f;
                timerTexts[i].text = "00s";
            }
        }
    }

    // coroutine for player turn timer & panel highlight animation on/off
    public IEnumerator IdleTimer(int playerTurn, string idleTimerSeconds)
    {
        TMP_Text timerText = timerTexts[0];
        Image timerRing = timerImages[0];

        for (int i = 0; i < timerImages.Length; i++)
        {
            if (i == playerTurn)
            {
                timerRing = timerImages[i];
                timerText = timerTexts[i];
            }
        }

        for (int i = 0; i < highlightPanelAnims.Length; i++)
            highlightPanelAnims[i].SetBool("highlightOn", false);

        highlightPanelAnims[T_SocketController.instance.playerTurnIndex].SetBool("highlightOn", true);

        timerRing.fillAmount = 1f;

        int timerInt = (Int32.Parse(idleTimerSeconds) / 1000);
        float divideAmt = (1f / (float)timerInt);
        while (timerInt >= 0)
        {
            timerText.text = timerInt.ToString() + "s";
            timerInt--;
            yield return new WaitForSeconds(1f);
            timerRing.fillAmount = timerRing.fillAmount - divideAmt;

            if (timerInt == 5)
            {
                if (L_SoundManager.instance.isSound)
                    L_SoundManager.instance.PlaySound(L_SoundType.TimeOutSound, T_SocketController.instance.transform);
            }
        }
    }



    // method use to start coroutine for game time (display on top-right corner)
    public void IdleGameTimerFunc()
    {
        //gameTimerCo = StartCoroutine(IdleGameTimer(L_SocketController.instance.gameTime));
    }

    // coroutine for game time
    public IEnumerator IdleGameTimer(string idleGameTimer)
    {
        int timerInt = (Int32.Parse(idleGameTimer) / 1000);
        //float divideAmt = (1f / (float)timerInt);
        while (timerInt >= 0)
        {
            //TimeSpan ts = TimeSpan.FromSeconds(timerInt);
            //gameTimerText.text = ts.Minutes.ToString("D2") + " : " + ts.Seconds.ToString("D2");
            int h = (timerInt / 3600);
            int m = (timerInt - (3600 * h)) / 60;
            int s = (timerInt - (3600 * h) - (m * 60));
            gameTimerText.text = m.ToString("D2") + " : " + s.ToString("D2");
            timerInt--;
            yield return new WaitForSeconds(1f);
        }
    }

    // stop coroutine for game time
    public void StopGameTime()
    {
        if (gameTimerCo != null)
        {
            StopCoroutine(gameTimerCo);
        }
    }


    // token highlight animation start
    public void HighlightKukriAnimation(int playerTurn)
    {
        if (T_SocketController.instance.posiblePos.Length > 0)
        {
            for (int j = 0; j < T_SocketController.instance.posiblePos.Length; j++)
            {
                if (playerTurn == 0)
                {
                    for (int i = 0; i < T_GameManager.instance.redPlayers.Length; i++)
                    {
                        if (T_GameManager.instance.redPlayers[i].GetComponent<T_Players>().currentWayPointInt == T_SocketController.instance.posiblePos[j])
                            T_GameManager.instance.redPlayers[i].GetComponent<Animator>().SetBool("tokenAnim", true);
                    }
                }
                else if (playerTurn == 1)
                {
                    for (int i = 0; i < T_GameManager.instance.yellowPlayers.Length; i++)
                    {
                        if (T_GameManager.instance.yellowPlayers[i].GetComponent<T_Players>().currentWayPointInt == T_SocketController.instance.posiblePos[j])
                            T_GameManager.instance.yellowPlayers[i].GetComponent<Animator>().SetBool("tokenAnim", true);
                    }
                }
                else if (playerTurn == 2)
                {
                    for (int i = 0; i < T_GameManager.instance.greenPlayers.Length; i++)
                    {
                        if (T_GameManager.instance.greenPlayers[i].GetComponent<T_Players>().currentWayPointInt == T_SocketController.instance.posiblePos[j])
                            T_GameManager.instance.greenPlayers[i].GetComponent<Animator>().SetBool("tokenAnim", true);
                    }
                }
                else if (playerTurn == 3)
                {
                    for (int i = 0; i < T_GameManager.instance.bluePlayers.Length; i++)
                    {
                        if (T_GameManager.instance.bluePlayers[i].GetComponent<T_Players>().currentWayPointInt == T_SocketController.instance.posiblePos[j])
                            T_GameManager.instance.bluePlayers[i].GetComponent<Animator>().SetBool("tokenAnim", true);
                    }
                }
            }

            
        }
    }

    // token highlight animation end
    void ToeknAnimOff()
    {
        for (int i = 0; i < T_GameManager.instance.allPlayers.Length; i++) //16
        {
            T_GameManager.instance.allPlayers[i].GetComponent<Animator>().SetBool("tokenAnim", false);
        }
    }

    // change board with waypoint rotation to bottom-left & also rotate board sprite according to user selected color
    public void RotateBoard(string loginPlayerToggleColor, string serverMyColor, int serverMyPlayerIndex)
    {
        int rotationDegrees = 0;
        switch (serverMyPlayerIndex)
        {
            case 0:
                rotationDegrees = 90;
                RotateAllPlayerTokenPosition(-90);
                RotateAllWaypoints(-90);
                boardParent.Rotate(Vector3.forward * rotationDegrees);
                Debug.Log("Rotating board sprite by " + rotationDegrees + " degrees.");

                // default red
                //0 => panel 3
                //1 => panel 0
                //2 => panel 1
                //3 => panel 2

                break;
            case 1:
                rotationDegrees = 180;
                RotateAllPlayerTokenPosition(180);
                RotateAllWaypoints(-180);
                boardParent.Rotate(Vector3.forward * rotationDegrees);
                Debug.Log("Rotating board sprite by " + rotationDegrees + " degrees.");

                // default yellow
                //0 => panel 2
                //1 => panel 3
                //2 => panel 0
                //3 => panel 1

                break;
            case 2:
                rotationDegrees = 270;
                RotateAllPlayerTokenPosition(-270);
                RotateAllWaypoints(-270);
                boardParent.Rotate(Vector3.forward * rotationDegrees);
                Debug.Log("Rotating board sprite by " + rotationDegrees + " degrees.");

                // default green
                //0 => panel 1
                //1 => panel 2
                //2 => panel 3
                //3 => panel 0

                break;
            case 3:
                rotationDegrees = 0;
                RotateAllPlayerTokenPosition(0);
                RotateAllWaypoints(0);
                boardParent.Rotate(Vector3.forward * rotationDegrees);
                Debug.Log("Rotating board sprite by " + rotationDegrees + " degrees.");

                // default blue
                //0 => panel 0
                //1 => panel 1
                //2 => panel 2
                //3 => panel 3

                break;
        }


        // for login player selected color, set boardImage rotation
        switch (loginPlayerToggleColor)
        {
            case "RedToggle":
                switch (serverMyPlayerIndex)
                {
                    case 1:
                        // yellow -> red
                        boardImage.Rotate(Vector3.forward * 270);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, redPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, yellowPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, greenPlayersSprite);
                        break;
                    case 2:
                        // green -> red
                        boardImage.Rotate(Vector3.forward * 180);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, greenPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, redPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, yellowPlayersSprite);
                        break;
                    case 3:
                        // blue -> red
                        boardImage.Rotate(Vector3.forward * 90);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, yellowPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, greenPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, redPlayersSprite);
                        break;
                }
                break;
            case "YellowToggle":
                switch (serverMyPlayerIndex)
                {
                    case 0:
                        //red  -> yellow
                        boardImage.Rotate(Vector3.forward * 90);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, yellowPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, greenPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, redPlayersSprite);
                        break;
                    case 2:
                        // green -> yellow
                        boardImage.Rotate(Vector3.forward * 270);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, redPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, yellowPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, greenPlayersSprite);
                        break;
                    case 3:
                        // blue -> yellow
                        boardImage.Rotate(Vector3.forward * 180);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, greenPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, redPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, yellowPlayersSprite);
                        break;
                }
                break;
            case "GreenToggle":
                switch (serverMyPlayerIndex)
                {
                    case 0:
                        //red  -> green
                        boardImage.Rotate(Vector3.forward * 180);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, greenPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, redPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, yellowPlayersSprite);
                        break;
                    case 1:
                        // yellow -> green
                        boardImage.Rotate(Vector3.forward * 90);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, yellowPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, greenPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, redPlayersSprite);
                        break;
                    case 3:
                        // blue -> green
                        boardImage.Rotate(Vector3.forward * 270);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, redPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, yellowPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, greenPlayersSprite);
                        break;
                }
                break;
            case "BlueToggle":
                switch (serverMyPlayerIndex)
                {
                    case 0:
                        //red  -> blue
                        boardImage.Rotate(Vector3.forward * 270);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, redPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, yellowPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, greenPlayersSprite);
                        break;
                    case 1:
                        // yellow -> blue
                        boardImage.Rotate(Vector3.forward * 180);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, greenPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, redPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, yellowPlayersSprite);
                        break;
                    case 2:
                        // green -> blue
                        boardImage.Rotate(Vector3.forward * 90);
                        ChangePlayerSprite(T_GameManager.instance.redPlayers, yellowPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.yellowPlayers, greenPlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.greenPlayers, bluePlayersSprite);
                        ChangePlayerSprite(T_GameManager.instance.bluePlayers, redPlayersSprite);
                        break;
                }
                break;
        }

    }

    // change all token sprite for RotateBoard() logic
    void ChangePlayerSprite(T_Players[] currentPlayer, Sprite changeSprite)
    {
        for (int i = 0; i < currentPlayer.Length; i++)
        {
            currentPlayer[i].GetComponent<Image>().sprite = changeSprite;
        }
    }

    // rotate all token for RotateBoard() logic
    void RotateAllPlayerTokenPosition(float zRotation)
    {
        for (int i = 0; i < T_GameManager.instance.allPlayers.Length; i++)
        {
            T_GameManager.instance.allPlayers[i].transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
        }
    }

    // rotate board waypoint for RotateBoard() logic
    void RotateAllWaypoints(float zRotation)
    {
        for (int i = 0; i < T_WayPointPathPrent.instance.commonWayPoint.Count; i++)
        {
            T_WayPointPathPrent.instance.commonWayPoint[i].transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
        }
    }
}

[System.Serializable]
public class T_IdleTimerDotsReference
{
    public List<T_IdleTimerDotsChildReference> gParent;
}

[System.Serializable]
public class T_IdleTimerDotsChildReference
{
    public List<GameObject> gChild;
}