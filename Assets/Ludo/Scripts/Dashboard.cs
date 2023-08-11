using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Dashboard : MonoBehaviour
{
    public static Dashboard instance;

    public Text nameText;

    [Header("-----Game Type GameObject-----")]
    public Transform classicLudoObj;
    public Transform quickLudoObj;
    public Transform tournamentsObj;
    public Image bottomPanel;

    public Image logoBG;
    public Image profilePic;

    void Awake()
    {
        instance = this;
    }

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "wallet":
                //MainMenuController.instance.DestroyScreen(MainMenuScreens.Dashboard);

                //L_MainMenuController.instance.ShowScreen(MainMenuScreens.Wallet);
                SceneManager.LoadScene("GameScene");
                break;

            case "profile":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Profile);
                break;

            case "classic_ludo":
                //PathManager.instance.currentScreenPathList.Add("Dashboard_ClassicLudo");
                //MainMenuController.instance.ShowScreen(MainMenuScreens.PlayerFinding);
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.QuickLudoSelection);
                QuickLudoSelection.instance.isClassicLudo = true;
                QuickLudoSelection.instance.isQuickLudo = false;
                QuickLudoSelection.instance.gameTypeId = classicLudoObj.GetChild(0).GetChild(3).GetComponent<Text>().text;
                break;

            case "quick_ludo":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.QuickLudoSelection);
                QuickLudoSelection.instance.isQuickLudo = true;
                QuickLudoSelection.instance.isClassicLudo = false;
                QuickLudoSelection.instance.gameTypeId = quickLudoObj.GetChild(0).GetChild(3).GetComponent<Text>().text;
                break;

            case "tournaments":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Tournaments);
                break;

            case "how_to_play":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.GamePlayInstruction);
                break;

            case "settings":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Settings);
                break;

            case "leaderboard":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Leaderboard);
                break;

            case "helpdesk":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.HelpDesk);
                break;

            case "referNEarn":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.ReferAndEarn);
                break;
        }
    }

    void Start()
    {
        logoBG.GetComponent<Animator>().SetBool("tokenAnim", true);

        //nameText.text = L_PlayerManager.instance.GetPlayerGameData().name;

        //StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameType], (serverResponse, errorBool, error) =>
        StartCoroutine(WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameType], (serverResponse, errorBool, error) =>
        {
            if (errorBool)
            {
                Debug.Log("Error in Game Type: " + error);
            }
            else
            {
                Debug.Log("Game Type response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("code"))
                {
                    if (data["code"].ToString() == "200")
                    {
                        if (iData1.Contains("data"))
                        {
                            for (int i = 0; i < data["data"].Count; i++)
                            {
                                //Debug.Log(data["data"][i]["name"]);
                                //Debug.Log(data["data"][i]["status"]);
                                //Debug.Log(data["data"][i]["description"]);

                                if (data["data"][i]["status"].ToString() == "1")
                                {
                                    if (data["data"][i]["name"].ToString() == "CLASSIC LUDO")
                                    {
                                        classicLudoObj.GetChild(0).GetChild(0).GetComponent<Text>().text = data["data"][i]["name"].ToString();
                                        classicLudoObj.GetChild(0).GetChild(2).GetComponent<Text>().text = data["data"][i]["description"].ToString();
                                        classicLudoObj.GetChild(0).GetChild(3).GetComponent<Text>().text = data["data"][i]["id"].ToString();
                                        classicLudoObj.gameObject.SetActive(true);
                                    }

                                    if (data["data"][i]["name"].ToString() == "QUICK LUDO")
                                    {
                                        quickLudoObj.GetChild(0).GetChild(0).GetComponent<Text>().text = data["data"][i]["name"].ToString();
                                        quickLudoObj.GetChild(0).GetChild(2).GetComponent<Text>().text = data["data"][i]["description"].ToString();
                                        quickLudoObj.GetChild(0).GetChild(3).GetComponent<Text>().text = data["data"][i]["id"].ToString();
                                        quickLudoObj.gameObject.SetActive(true);
                                    }

                                    if (data["data"][i]["name"].ToString() == "TOURNAMENTS")
                                    {
                                        tournamentsObj.GetChild(0).GetChild(0).GetComponent<Text>().text = data["data"][i]["name"].ToString();
                                        tournamentsObj.GetChild(0).GetChild(2).GetComponent<Text>().text = data["data"][i]["description"].ToString();
                                        tournamentsObj.GetChild(0).GetChild(3).GetComponent<Text>().text = data["data"][i]["id"].ToString();
                                        tournamentsObj.gameObject.SetActive(true);
                                    }
                                    StartCoroutine(UIAnimation.instance.PanelAnimation());
                                }
                            }
                            StartCoroutine(UIAnimation.instance.PanelAnimation());
                            bottomPanel.GetComponent<CanvasGroup>().DOFade(1, 0.25f);
                            bottomPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 75), 1f);
                        }
                    }
                }
            }
            GetProfile("dashboard");
        }));
        if (!string.IsNullOrEmpty(PlayerManager.instance.GetPlayerGameData().avatarURL))
        {
            Davinci.get().setFadeTime(0).load(PlayerManager.instance.GetPlayerGameData().avatarURL).into(profilePic).start();
        }
    }


    public void GetProfile(string callFrom)
    {
        // callFrom: dashboard, myprofile

        //StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.Profile], (serverResponse, errorBool, error) =>
        StartCoroutine(WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.Profile], (serverResponse, errorBool, error) =>
        {
            if (errorBool)
            {
                Debug.Log("Error in get profile: " + error);
            }
            else
            {
                Debug.Log("Game Type response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("statusCode"))
                {
                    if (data["statusCode"].ToString() == "200")
                    {
                        if (iData1.Contains("data"))
                        {
                            PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
                            int changeCount = 0;
                            if (data["data"]["username"] != null)
                            {
                                if (!data["data"]["username"].ToString().Equals(PlayerManager.instance.GetPlayerGameData().userName))
                                {
                                    playerData.userName = data["data"]["username"].ToString();
                                    changeCount++;
                                }
                            }

                            if (!data["data"]["full_name"].ToString().Equals(PlayerManager.instance.GetPlayerGameData().fullName))
                            {
                                playerData.fullName = data["data"]["full_name"].ToString();
                                changeCount++;
                            }

                            if (!data["data"]["email"].ToString().Equals(PlayerManager.instance.GetPlayerGameData().userEmail))
                            {
                                playerData.userEmail = data["data"]["email"].ToString();
                                changeCount++;
                            }

                            if (changeCount > 0)
                                PlayerManager.instance.SetPlayerGameData(playerData);

                            if (callFrom == "dashboard")
                            {
                                try
                                {
                                    L_MainMenuController.instance.GetProfilePictureLink(data["data"]["profile_image"].ToString(), profilePic);
                                }
                                catch (System.Exception e)
                                {
                                    Debug.Log("error in Profile Picture Link " + e.Message);
                                }
                            }
                        }
                    }
                }
            }
        }));
    }
}
