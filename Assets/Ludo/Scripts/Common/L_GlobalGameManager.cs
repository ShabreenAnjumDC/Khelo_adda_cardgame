using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class L_GlobalGameManager : MonoBehaviour
{
    public static L_GlobalGameManager instance;

    public bool isLoginShow = false;
    public GameObject[] gameScens; // prefab of all parent screens in game
    public static string playerToken = "";
    public List<string> currentScreenPathList = new List<string>();

    public L_SocketController socketController;
    public T_SocketController tournamentSocketController;

    GameObject previousScene = null; // contains current loaded sceneObject

    [SerializeField]
    public L_RoomData currentRoomData = new L_RoomData();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        LoadScene(L_Scenes.MainDashboard);
    }

    public void LoadScene(L_Scenes gameScene)
    {
        if (previousScene != null)
        {
            StartCoroutine(WaitAndDestroyOldScreen(previousScene));
        }
        previousScene = Instantiate(gameScens[(int)gameScene], Vector3.zero, Quaternion.identity) as GameObject;
    }

    IEnumerator WaitAndDestroyOldScreen(GameObject gm)
    {
        yield return new WaitForSeconds(1);

        if (gm != null)
        {
            //if (gm.name == "MainDashboardScene(Clone)" || gm.name == "MainMenuScene(Clone)" || gm.name == "InGame(Clone)")
            //{
                Destroy(gm);
            //}
            //else
            //{
            //    gm.SetActive(false);    //DEV_CODE This line is added
            //}
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Null reference exception found gm is null in GlobalGameManager.WaitAndDestroyOldScreen ");
#endif
        }

        yield return new WaitForSeconds(0.2f);
        System.GC.Collect();

        /*yield return new WaitForSeconds(20);
        GameObject g = Instantiate(gameScens[(int)Scenes.InGame], Vector3.zero, Quaternion.identity) as GameObject;
        g.name = "Second";*/
    }

    public IEnumerator RunAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
    /*public IEnumerator ShowPopUp(Text popUpText, string msg, float delay, Action action = null)
    {
        popUpText.gameObject.SetActive(true);
        popUpText.text = msg;
        yield return new WaitForSeconds(delay);
        //yield return new WaitForSeconds(2f);
        popUpText.gameObject.SetActive(false);
        if (action != null)
            action();
    }
    public IEnumerator ShowPopUp(Text popUpText, string msg, string color = "red", float delay = 1.5f, Action action = null)
    {
        popUpText.transform.parent.gameObject.SetActive(true);
        if (color == "white")
            popUpText.color = Color.white;
        else if (color == "red")
            popUpText.color = Color.red;
        else if (color == "green")
            popUpText.color = Color.green;
        else if (color == "theme_yellow")
            popUpText.color = new Color(0.9568f, 0.7254f, 0.04313f, 1f);
        popUpText.text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.transform.parent.gameObject.SetActive(false);
        if (action != null)
            action();
    }*/
    public IEnumerator ShowPopUpTMP(TMP_Text popUpText, string msg, string color = "red", float delay = 1.5f, Action action = null)
    {
        popUpText.gameObject.SetActive(true);
        if (color == "white")
            popUpText.color = Color.white;
        else if (color == "red")
            popUpText.color = Color.red;
        else if (color == "green")
            popUpText.color = Color.green;
        else if (color == "theme_yellow")
            popUpText.color = new Color(0.9568f, 0.7254f, 0.04313f, 1f);
        popUpText.text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.gameObject.SetActive(false);
        if (action != null)
            action();
    }
}

public enum L_Scenes
{
    MainDashboard,
    MainMenu,
    InGame
}

[System.Serializable]
public class L_RoomData
{
    public string roomId;
    public string socketTableId;
    public string title;
    public int players;
    public float commision;

    //DEV_CODE
    public string roomIconUrl;
    public string roomBG;

    public bool isLobbyRoom;
    public int totalActivePlayers;

    public int passCode;
    public string exclusiveTable;
    public string assignRole;
}
