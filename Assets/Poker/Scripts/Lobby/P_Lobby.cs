using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class P_Lobby : MonoBehaviour
{
    public static P_Lobby instance;

    public string[] gameTypeStr = { "TEXAS", "PLO", "SIT N GO", "ANONYMOUS", "PRACTICE", "TOURNAMENT" };

    public Transform gameTypeContent;
    public Transform mainScrollViewContent;
    public GameObject gameTypePrefab;
    public GameObject texasPrefab;
    public GameObject holdemPrefab;
    public GameObject PLOPrefab;
    public GameObject sitNGoPrefab;
    public GameObject practicePrefab;
    public GameObject tournamentPrefab;
    public GameObject secondTexasPrefab;

    public Color nonSelectedGameTypeColor;
    public Color selectedGameTypeColor;

    public Text errorText;

    public string currentCategory = "TEXAS";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetGameTypeInScrollView();
    }

    public void OnClickOnButton(string buttonName)
    {
        switch(buttonName)
        {
            case "back":
                P_MainSceneManager.instance.ScreenDestroy();
                GlobalGameManager.instance.LoadScene(Scenes.MainDashboard);

                //P_SocketController.instance.ExitGamePlay();
                P_SocketController.instance.lobbySelectedGameType = "";
                break;
        }
    }

    void SetGameTypeInScrollView()
    {
        for (int i = 0; i < gameTypeStr.Length; i++)
        {
            int tempI = i;
            string tempName = gameTypeStr[i];
            GameObject go = Instantiate(gameTypePrefab, gameTypeContent);
            go.name = i + "_" + gameTypeStr[i];
            Text txt = go.transform.GetChild(0).GetComponent<Text>();
            txt.text = gameTypeStr[i];
            if (i == 0)
            {
                go.transform.GetChild(1).gameObject.SetActive(true);
                txt.color = selectedGameTypeColor;
            }
            Button btn = go.transform.GetComponent<Button>();
            btn.onClick.AddListener(() => OnGameTypeButtonClick(tempI, tempName));

        }

        P_SocketController.instance.SendGetRooms();
        //GetLobby1Data("TEXAS");
    }

    public void OnGameTypeButtonClick(int clickedI, string tempName)
    {
        for (int i = 0; i < gameTypeContent.childCount; i++)
        {
            gameTypeContent.GetChild(i).GetChild(0).GetComponent<Text>().color = nonSelectedGameTypeColor;
            gameTypeContent.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
        gameTypeContent.GetChild(clickedI).GetChild(0).GetComponent<Text>().color = selectedGameTypeColor;
        gameTypeContent.GetChild(clickedI).GetChild(1).gameObject.SetActive(true);

        ClearMainScrollView();

        if (tempName.Equals("TEXAS"))
        {
            currentCategory = "TEXAS";
            //GetLobby1Data("TEXAS");
        }
        else if (tempName.Equals("HOLDEM"))
        {
            currentCategory = "HOLDEM";
            //GetLobby1Data("HOLDEM");
        }
        else if (tempName.Equals("PLO"))
        {
            currentCategory = "PLO";
            //GetLobby1Data("PLO");
        }
        else if (tempName.Equals("SIT N GO"))
        {
            //Instantiate(sitNGoPrefab, mainScrollViewContent);
            //Instantiate(sitNGoPrefab, mainScrollViewContent);
            currentCategory = "SIT N GO";
            //GetLobby1Data("SIT N GO");
        }
        else if (tempName.Equals("ANONYMOUS"))
        {
            //Instantiate(texasPrefab, mainScrollViewContent);
            //Instantiate(PLOPrefab, mainScrollViewContent);
            //Instantiate(sitNGoPrefab, mainScrollViewContent);
            currentCategory = "ANONYMOUS";
            //GetLobby1Data("ANONYMOUS");
        }
        else if (tempName.Equals("PRACTICE"))
        {
            //Instantiate(practicePrefab, mainScrollViewContent);
            //Instantiate(practicePrefab, mainScrollViewContent);
            currentCategory = "PRACTICE";
            //GetLobby1Data("PRACTICE");
        }
        else if (tempName.Equals("TOURNAMENT"))
        {
            //Instantiate(tournamentPrefab, mainScrollViewContent);
            //Instantiate(tournamentPrefab, mainScrollViewContent);
            currentCategory = "TOURNAMENT";
            //GetLobby1Data("TOURNAMENT");
        }
        P_SocketController.instance.SendGetRooms();
    }

    public void CreateLobby1Data(string responseData)
    {
        JsonData data = JsonMapper.ToObject(responseData);
        if (data["data"].Count > 0)
        {
            for (int i = 0; i < mainScrollViewContent.childCount; i++)
            {
                Destroy(mainScrollViewContent.GetChild(i).gameObject);
            }

            for (int i = 0; i < data["data"].Count; i++)
            {
                int tempI = i;
                string categoryData = data["data"][i]["game_type"]["name"].ToString();

                if (currentCategory.Contains(categoryData) || categoryData.Contains(currentCategory))
                {
                    GameObject texas1 = Instantiate(texasPrefab, mainScrollViewContent);
                    P_Lobby_Texas pLobbyTexas1 = texas1.GetComponent<P_Lobby_Texas>();

                    IDictionary iDataI = data["data"][i] as IDictionary;
                    if (iDataI.Contains("game_json_data"))
                    {
                        IDictionary iDataIgame = data["data"][i]["game_json_data"] as IDictionary;
                        string smallBlindData = "0", bigBlindData = "0", minimumBuyin = "0";
                        if (iDataIgame.Contains("small_blind"))
                            smallBlindData = data["data"][i]["game_json_data"]["small_blind"].ToString();

                        if (iDataIgame.Contains("big_blind"))
                            bigBlindData = data["data"][i]["game_json_data"]["big_blind"].ToString();

                        pLobbyTexas1.blindsText.text = smallBlindData + "/" + bigBlindData;

                        if (iDataIgame.Contains("minimum_buyin"))
                            minimumBuyin = data["data"][i]["game_json_data"]["minimum_buyin"].ToString();

                        pLobbyTexas1.minBuyInText.text = minimumBuyin;
                    }

                    pLobbyTexas1.playerCountText.text = data["data"][i]["totalPlayers"].ToString();

                    if (currentCategory.Equals("TEXAS"))
                    {
                        pLobbyTexas1.heading.text = "HOLD'EM";
                        pLobbyTexas1.bgButton.onClick.AddListener(() =>
                        {
                            SecondPrefab("TEXAS", data["data"][tempI]);
                        });
                    }
                    else if (currentCategory.Equals("PLO"))
                    {
                        pLobbyTexas1.heading.text = categoryData;
                        pLobbyTexas1.bgButton.onClick.AddListener(() =>
                        {
                            SecondPrefab("PLO", data["data"][tempI]);
                        });
                    }
                    else if (currentCategory.Equals("SIT N GO"))
                    {
                        pLobbyTexas1.heading.text = categoryData;
                        pLobbyTexas1.bgButton.onClick.AddListener(() =>
                        {
                            SecondPrefab("SIT N GO", data["data"][tempI]);
                        });
                    }
                    else if (currentCategory.Equals("ANONYMOUS"))
                    {
                        pLobbyTexas1.heading.text = categoryData;
                        pLobbyTexas1.bgButton.onClick.AddListener(() =>
                        {
                            SecondPrefab("ANONYMOUS", data["data"][tempI]);
                        });
                    }
                    else if (currentCategory.Equals("PRACTICE"))
                    {
                        pLobbyTexas1.heading.text = categoryData;
                        pLobbyTexas1.bgButton.onClick.AddListener(() =>
                        {
                            SecondPrefab("PRACTICE", data["data"][tempI]);
                        });
                    }
                }
            }
        }
        else
        {
            errorText.text = "Data not found from server";
            errorText.gameObject.SetActive(true);
        }
    }

    public void GetLobby1Data(string selectedCategoryStr)
    {
        if (!P_WebServices.instance.IsInternetAvailable())
        {
            errorText.text = "No Internet";
            errorText.gameObject.SetActive(true);
        }
        else
        {
            errorText.text = "";
            errorText.gameObject.SetActive(false);

            P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.Loading);
            StartCoroutine(P_WebServices.instance.GETRequestData(P_RequestType.PokerGameList, (string downloadText, bool isError, string errorString) =>
            {
                P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.Loading);
                if (isError)
                {
                    if (P_GameConstant.enableErrorLog)
                        Debug.Log("Get game list error: " + errorString);

                    errorText.text = "Error from server";
                    errorText.gameObject.SetActive(true);
                }
                else
                {
                    if (P_GameConstant.enableLog)
                        Debug.Log("Get game list: " + downloadText);

                    JsonData data = JsonMapper.ToObject(downloadText);
                    if (data["data"].Count > 0)
                    {
                        for (int i = 0; i < data["data"].Count; i++)
                        {
                            int tempI = i;
                            string categoryData = data["data"][i]["game_type"]["name"].ToString();

                            if (selectedCategoryStr.Contains(categoryData) || categoryData.Contains(selectedCategoryStr))
                            {
                                GameObject texas1 = Instantiate(texasPrefab, mainScrollViewContent);
                                P_Lobby_Texas pLobbyTexas1 = texas1.GetComponent<P_Lobby_Texas>();

                                IDictionary iDataI = data["data"][i] as IDictionary;
                                if (iDataI.Contains("game_json_data"))
                                {
                                    IDictionary iDataIgame = data["data"][i]["game_json_data"] as IDictionary;
                                    string smallBlindData = "0", bigBlindData = "0", minimumBuyin = "0";
                                    if (iDataIgame.Contains("small_blind"))
                                        smallBlindData = data["data"][i]["game_json_data"]["small_blind"].ToString();

                                    if (iDataIgame.Contains("big_blind"))
                                        bigBlindData = data["data"][i]["game_json_data"]["big_blind"].ToString();

                                    pLobbyTexas1.blindsText.text = smallBlindData + "/" + bigBlindData;

                                    if (iDataIgame.Contains("minimum_buyin"))
                                        minimumBuyin = data["data"][i]["game_json_data"]["minimum_buyin"].ToString();

                                    pLobbyTexas1.minBuyInText.text = minimumBuyin;
                                }

                                pLobbyTexas1.playerCountText.text = data["data"][i]["totalPlayers"].ToString();

                                if (selectedCategoryStr.Equals("TEXAS"))
                                {
                                    pLobbyTexas1.heading.text = "HOLD'EM";
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        SecondPrefab("TEXAS", data["data"][tempI]);
                                    });
                                }
                                else if (selectedCategoryStr.Equals("PLO"))
                                {
                                    pLobbyTexas1.heading.text = categoryData;
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        SecondPrefab("PLO", data["data"][tempI]);
                                    });
                                }
                                else if (selectedCategoryStr.Equals("SIT N GO"))
                                {
                                    pLobbyTexas1.heading.text = categoryData;
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        SecondPrefab("SIT N GO", data["data"][tempI]);
                                    });
                                }
                                else if (selectedCategoryStr.Equals("ANONYMOUS"))
                                {
                                    pLobbyTexas1.heading.text = categoryData;
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        SecondPrefab("ANONYMOUS", data["data"][tempI]);
                                    });
                                }
                                else if (selectedCategoryStr.Equals("PRACTICE"))
                                {
                                    pLobbyTexas1.heading.text = categoryData;
                                    pLobbyTexas1.bgButton.onClick.AddListener(() =>
                                    {
                                        SecondPrefab("PRACTICE", data["data"][tempI]);
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        errorText.text = "Data not found from server";
                        errorText.gameObject.SetActive(true);
                    }
                }
            }));
        }
    }

    void SecondPrefab(string gameType, JsonData dataOfI)
    {
        //ClearMainScrollView();

        //GameObject subCategory = Instantiate(secondTexasPrefab, mainScrollViewContent);
        //subCategory.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    P_SocketController.instance.LoadGamePlay();
        //});

        if (P_GameConstant.enableLog)
            Debug.Log(JsonMapper.ToJson(dataOfI));

        P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.LobbySecond);

        if (P_Lobby_Second.instance != null)
        {
            P_Lobby_Second.instance.OnLoadScrollDetails(gameType, dataOfI);
        }

        P_SocketController.instance.lobbySelectedGameType = gameType;
    }

    void ClearMainScrollView()
    {
        for (int i = 0; i < mainScrollViewContent.childCount; i++)
        {
            Destroy(mainScrollViewContent.GetChild(i).gameObject);
        }
    }
}
