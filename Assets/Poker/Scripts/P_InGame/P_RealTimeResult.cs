using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_RealTimeResult : MonoBehaviour
{
    public GameObject LoadingText1, LoadingText2;
    public GameObject resultPrefab;
    public GameObject spectatorPrefab;
    public Transform RealTimeContent;
    public Transform spectatorContent;
    public Text TimeText;

    private string tableId;
    P_InGameUiManager inGameUiManager;

    private void Start()
    {
        //P_SocketController.instance.currentScreenName = "RealTimeResult";
        //inGameUiManager = transform.parent.parent.GetComponent<P_InGameUiManager>();
        OnOpen(P_SocketController.instance.TABLE_ID);
    }

    public void OnOpen(string tId)
    {
        TimeText.text = System.DateTime.Now.ToLocalTime().ToShortTimeString();

        tableId = tId;
        Debug.Log("Table ID --> " + tableId);

        string requestData = "{\"tableId\":\"" + tableId + "\"}";

        StartCoroutine(P_WebServices.instance.GETRequestDataURL(P_GameConstant.GAME_URLS[(int)P_RequestType.RealtimeResult] + P_SocketController.instance.TABLE_ID, (string downloadText, bool isError, string errorString) =>
        {
            if (isError)
            {
                if (P_GameConstant.enableErrorLog)
                    Debug.Log("Get realTimeResult error: " + errorString);
            }
            else
            {
                if (P_GameConstant.enableLog)
                    Debug.Log("Get realTimeResult data: " + downloadText);

                for (int i = 0; i < RealTimeContent.childCount; i++)
                {
                    Destroy(RealTimeContent.GetChild(i).gameObject);
                }

                for (int i = 0; i < spectatorContent.childCount; i++)
                {
                    Destroy(spectatorContent.GetChild(i).gameObject);
                }

                JsonData data = JsonMapper.ToObject(downloadText);

                for (int i = 0; i < data["data"]["result"].Count; i++)
                {
                    LoadingText1.SetActive(false);
                    LoadingText2.SetActive(false);
                    GameObject gm = Instantiate(resultPrefab, RealTimeContent) as GameObject;
                    gm.SetActive(true);
                    gm.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = data["data"]["result"][i]["username"].ToString();
                    if (data["data"]["result"][i]["buy_in"] != null)
                        gm.transform.Find("BuyIn").GetComponent<TMPro.TextMeshProUGUI>().text = data["data"]["result"][i]["buy_in"].ToString();
                    else
                        gm.transform.Find("BuyIn").GetComponent<TMPro.TextMeshProUGUI>().text = "-";

                    float amt = float.Parse(data["data"]["result"][i]["winnings"].ToString());
                    Debug.Log(PrefsManager.GetPlayerData().userName + " = " + data["data"]["result"][i]["username"].ToString() + " - " + amt);

                    if (amt > 0)
                        gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().text = "+" + amt;
                    else
                        gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().text = "" + amt;

                    if (PrefsManager.GetPlayerData().userName == data["data"]["result"][i]["username"].ToString())
                    {
                        gm.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
                        gm.transform.Find("BuyIn").GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
                        gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
                    }
                    else if (amt <= 0)
                    {
                        gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
                    }
                    else
                        gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().color = Color.green;
                }

                /*if (data["data"]["standoutArr"].Count > 0)
                {
                    for (int i = 0; i < data["data"]["standoutArr"].Count; i++)
                    {
                        GameObject gm = Instantiate(spectatorPrefab, spectatorContent) as GameObject;
                        //gm.GetComponent<DownloadAvatar>().avatarUrl = data["data"]["standoutArr"][i]["profileImage"].ToString();
                        gm.transform.Find("Text").GetComponent<Text>().text = data["data"]["standoutArr"][i]["userName"].ToString();
                        gm.SetActive(true);
                    }
                }*/
            }
        }));
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "close":
                {
                    //P_SocketController.instance.currentScreenName = "Game";
                    //inGameUiManager.DestroyScreen(P_InGameScreens.RealTimeResult);
                    if (P_InGameUiManager.instance != null)
                        P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.RealTimeResult);
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in RealTimeResultUiManager = " + eventName);
                }
                break;
        }
    }
}
