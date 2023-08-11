using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [Header("-----Parents-----")]
    public GameObject dailyParent;
    public GameObject weeklyParent;
    public GameObject monthlyParent;

    [Header("-----Option Toggle-----")]
    public Toggle dailyOptionToggle;
    public Toggle weeklyOptionToggle;
    public Toggle monthlyOptionToggle;

    [Header("-----Option Text-----")]
    public Text dailyOptionText;
    public Text weeklyOptionText;
    public Text monthlyOptionText;

    [Header("-----Option On Off Sprites & Color-----")]
    public Sprite optionOnSprite;
    public Sprite optionOffSprite;
    public Color optionOnTextColor;
    public Color optionOffTextColor;

    [Header("-----scroll-----")]
    public GameObject scrollItem;
    public Transform contentDaily;
    public Transform contentWeekly;
    public Transform contentMonthly;

    [Header("-----top 3---")]
    public TMP_Text[] top3Username;
    public TMP_Text[] top3Amount;

    public Text noDataText;

    private void Start()
    {
        ShowDailyPanel();
    }

    public void OnOptionToggleValueChanged(string toggleName)
    {
        switch (toggleName)
        {
            case "daily":
                ShowDailyPanel();
                break;

            case "weekly":
                ShowWeeklyPanel();
                break;

            case "monthly":
                ShowMonthlyPanel();
                break;
        }
    }

    public void OnClickCloseButton()
    {
        L_MainMenuController.instance.PlayButtonSound();
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
    }

    void ShowDailyPanel()
    {
        if (dailyOptionToggle.isOn)
        {
            L_MainMenuController.instance.PlayButtonSound();

            dailyParent.SetActive(true);
            weeklyParent.SetActive(false);
            monthlyParent.SetActive(false);

            dailyOptionText.color = optionOnTextColor;
            weeklyOptionText.color = optionOffTextColor;
            monthlyOptionText.color = optionOffTextColor;

            CallLeaderboardApi(contentDaily, "daily");
        }
    }

    void ShowWeeklyPanel()
    {
        if (weeklyOptionToggle.isOn)
        {
            L_MainMenuController.instance.PlayButtonSound();

            weeklyOptionText.color = optionOnTextColor;
            dailyOptionText.color = optionOffTextColor;
            monthlyOptionText.color = optionOffTextColor;

            weeklyParent.SetActive(true);
            dailyParent.SetActive(false);
            monthlyParent.SetActive(false);

            CallLeaderboardApi(contentWeekly, "weekly");
        }
    }

    void ShowMonthlyPanel()
    {
        if (monthlyOptionToggle.isOn)
        {
            L_MainMenuController.instance.PlayButtonSound();

            monthlyOptionText.color = optionOnTextColor;
            weeklyOptionText.color = optionOffTextColor;
            dailyOptionText.color = optionOffTextColor;

            monthlyParent.SetActive(true);
            dailyParent.SetActive(false);
            weeklyParent.SetActive(false);

            CallLeaderboardApi(contentMonthly, "monthly");
        }
    }

    void CallLeaderboardApi(Transform content, string apiType)
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        top3Username[0].text = "";
        top3Amount[0].text = "";
        top3Username[1].text = "";
        top3Amount[1].text = "";
        top3Username[2].text = "";
        top3Amount[2].text = "";

        StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.Leaderboard] + "/" + apiType, (serverResponse, errorBool, error) =>
        {
            bool isDisplaySuccess = false;

            if (errorBool)
            {
                Debug.Log("Error in Leaderboard 1: " + error);
            }
            else
            {
                Debug.Log("Leaderboard response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("code"))
                {
                    if (data["code"].ToString() == "200")
                    {
                        if (data["data"].Count > 0)
                        {
                            noDataText.gameObject.SetActive(false);
                            isDisplaySuccess = true;

                            for (int i = 0; i < data["data"].Count; i++)
                            {
                                //Debug.Log("Leaderboard: " + data["data"][i]["rank"].ToString());
                                //Debug.Log("Leaderboard: " + data["data"][i]["name"].ToString());
                                //Debug.Log("Leaderboard: " + data["data"][i]["amount"].ToString());
                                //Debug.Log("Leaderboard: " + data["data"][i]["gamePlayed"].ToString());
                                //Debug.Log("Leaderboard:-----------");

                                GameObject dailyGo = Instantiate(scrollItem, content);
                                dailyGo.GetComponent<LeaderboardScrollItem>().rankText.text = data["data"][i]["rank"].ToString();
                                dailyGo.GetComponent<LeaderboardScrollItem>().nameText.text = data["data"][i]["name"].ToString();
                                dailyGo.GetComponent<LeaderboardScrollItem>().amountText.text = "₹" + data["data"][i]["amount"].ToString();
                                dailyGo.GetComponent<LeaderboardScrollItem>().gamesPlayedText.text = data["data"][i]["gamePlayed"].ToString();

                                if (i == 0)
                                {
                                    top3Username[0].text = data["data"][i]["name"].ToString();
                                    top3Amount[0].text = "₹" + data["data"][i]["amount"].ToString();
                                }
                                else if (i == 1)
                                {
                                    top3Username[1].text = data["data"][i]["name"].ToString();
                                    top3Amount[1].text = "₹" + data["data"][i]["amount"].ToString();
                                }
                                else if (i == 2)
                                {
                                    top3Username[2].text = data["data"][i]["name"].ToString();
                                    top3Amount[2].text = "₹" + data["data"][i]["amount"].ToString();
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("Error in Leaderboard, data count 0");
                        }
                    }
                    else if (iData1.Contains("errorMessage"))
                    {
                        Debug.Log("Error inLeaderboard 2");
                    }
                    else
                    {
                        Debug.Log("Error inLeaderboard 3");
                    }
                }
                else
                {
                    Debug.Log("Success in Leaderboard 4");
                }
            }

            if (!isDisplaySuccess)
            {
                noDataText.gameObject.SetActive(true);
                noDataText.text = "No Data";
            }

        }));
    }
}
