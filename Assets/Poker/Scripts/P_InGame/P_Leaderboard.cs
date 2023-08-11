using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class P_Leaderboard : MonoBehaviour
{
    public static P_Leaderboard instance;

    public Transform firstRankPlayer, secondRankPlayer, thirdRankPlayer, scrollContent, leaderboardItem;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GetLeaderboardData();
        P_InGameUiManager.instance.ShowScreen(P_InGameScreens.Loading);
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "close":
                {
                    if (P_InGameUiManager.instance != null)
                        P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Leaderboard);
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in RealTimeResultUiManager = " + eventName);
                }
                break;
        }
    }

    void GetLeaderboardData()
    {
        StartCoroutine(P_WebServices.instance.GETRequestDataURL(P_GameConstant.GAME_URLS[(int)P_RequestType.PokerLeaderboard] + P_SocketController.instance.TABLE_ID, (string downloadText, bool isError, string errorString) =>
        {
            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Loading);
            if (isError)
            {
                if (P_GameConstant.enableErrorLog)
                    Debug.Log("Get leaderboard error: " + errorString);

                //errorText.text = "Error from server";
                //errorText.gameObject.SetActive(true);
            }
            else
            {
                if (P_GameConstant.enableLog)
                    Debug.Log("Get leaderboard data: " + downloadText);

                for (int i = 0; i < scrollContent.childCount; i++)
                {
                    Destroy(scrollContent.GetChild(i).gameObject);
                }

                JsonData data = JsonMapper.ToObject(downloadText);

                if (data["data"].Count > 0)
                {
                    firstRankPlayer.GetChild(1).GetComponent<Text>().text = data["data"][0]["user_name"].ToString();
                    firstRankPlayer.GetChild(2).GetComponent<Text>().text = "<size=16>₹</size> " + data["data"][0]["win_amount"].ToString();

                    secondRankPlayer.GetChild(1).GetComponent<Text>().text = data["data"][1]["user_name"].ToString();
                    secondRankPlayer.GetChild(2).GetComponent<Text>().text = "<size=16>₹</size> " + data["data"][1]["win_amount"].ToString();

                    thirdRankPlayer.GetChild(1).GetComponent<Text>().text = data["data"][2]["user_name"].ToString();
                    thirdRankPlayer.GetChild(2).GetComponent<Text>().text = "<size=16>₹</size> " + data["data"][2]["win_amount"].ToString();

                    for (int i = 3; i < data["data"].Count; i++)
                    {
                        //int tempI = i;

                        IDictionary iDataI = data["data"][i] as IDictionary;
                        Transform lbItem = Instantiate(leaderboardItem.gameObject, scrollContent).transform;
                        lbItem.gameObject.SetActive(true);

                        lbItem.GetChild(0).GetComponent<Text>().text = data["data"][i]["rank"].ToString();
                        lbItem.GetChild(1).GetComponent<Text>().text = data["data"][i]["user_name"].ToString();
                        lbItem.GetChild(3).GetComponent<Text>().text = /*"<size=16>₹</size> " + */data["data"][i]["win_amount"].ToString();
                    }
                }
            }
        }));
    }
}
