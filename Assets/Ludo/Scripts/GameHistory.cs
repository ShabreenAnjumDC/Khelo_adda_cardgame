using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using TMPro;
using System;

public class GameHistory : MonoBehaviour
{
    //public static GameHistory instance;

    public GameObject noDataTextObject;
    public GameObject gameHistoryItem;
    public Transform scrollContent;

    void Start()
    {
        StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameHistory], (serverResponse, errorBool, error) =>
        {
            bool isDisplaySuccess = false;

            if (errorBool)
            {
                Debug.Log("Error in get Game History: " + error);
            }
            else
            {
                Debug.Log("Game History response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("code"))
                {
                    if (data["code"].ToString() == "200")
                    {
                        if (iData1.Contains("data"))
                        {
                            for (int j = 0; j < scrollContent.childCount; j++)
                                Destroy(scrollContent.GetChild(j).gameObject);

                            if (data["data"].Count > 0)
                            {
                                for (int i = 0; i < data["data"].Count; i++)
                                {
                                    GameObject go = Instantiate(gameHistoryItem, scrollContent);
                                    go.transform.GetChild(0).GetComponent<TMP_Text>().text = data["data"][i]["tableId"].ToString();
                                    go.transform.GetChild(1).GetComponent<TMP_Text>().text = data["data"][i]["game"]["game_type"]["name"].ToString();
                                    go.transform.GetChild(2).GetComponent<TMP_Text>().text = data["data"][i]["betAmount"].ToString();
                                    go.transform.GetChild(3).GetComponent<TMP_Text>().text = data["data"][i]["winAmount"].ToString();
                                    DateTime createdTime = Convert.ToDateTime(data["data"][i]["createdAt"].ToString()).ToLocalTime();
                                    go.transform.GetChild(4).GetComponent<TMP_Text>().text = createdTime.ToString("MM/dd/yyyy hh:mm tt");
                                }
                                noDataTextObject.SetActive(false);
                                isDisplaySuccess = true;
                            }
                        }
                    }
                }

            }
            
            if (!isDisplaySuccess)
                noDataTextObject.SetActive(true);

        }));
    }

    public void OnClickClose()
    {
        L_MainMenuController.instance.PlayButtonSound();

        L_MainMenuController.instance.DestroyScreen(MainMenuScreens.GameHistory);
    }
}
