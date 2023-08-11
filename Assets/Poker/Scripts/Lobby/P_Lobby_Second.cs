using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;

public class P_Lobby_Second : MonoBehaviour
{
    public static P_Lobby_Second instance;

    public Sprite btnHighlightedBG;
    public Sprite btnBG;

    public Transform scrollContent;
    public GameObject texasWithHeading;
    public GameObject texasPrefab;
    public GameObject holdemPrefab;
    public GameObject PLOPrefab;

    public Text titleText, tableCountText, blindsText, playerCountText, minBuyInText;

    public Text errorText;

    JsonData roomData;

    void Awake()
    {
        instance = this;
    }

    public void OnClickOnButton(string buttonName)
    {
        switch (buttonName)
        {
            case "back":
                P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.LobbySecond);
                break;
        }
    }

    public void OnLoadScrollDetails(string gameType, JsonData dataOfI)
    {
        if (P_GameConstant.enableLog)
            Debug.Log(JsonMapper.ToJson(dataOfI));

        roomData = dataOfI;
        titleText.text = gameType;
        IDictionary iDataI = dataOfI as IDictionary;
        if (iDataI.Contains("game_json_data"))
        {
            IDictionary iDataIgame = dataOfI["game_json_data"] as IDictionary;
            string smallBlindData = "0", bigBlindData = "0", minimumBuyin = "0";
            if (iDataIgame.Contains("small_blind"))
                smallBlindData = dataOfI["game_json_data"]["small_blind"].ToString();

            if (iDataIgame.Contains("big_blind"))
                bigBlindData = dataOfI["game_json_data"]["big_blind"].ToString();

            blindsText.text = smallBlindData + "/" + bigBlindData;

            if (iDataIgame.Contains("minimum_buyin"))
                minimumBuyin = dataOfI["game_json_data"]["minimum_buyin"].ToString();

            minBuyInText.text = minimumBuyin;

            P_SocketController.instance.smallBlindTableData = smallBlindData;
            P_SocketController.instance.bigBlindTableData = bigBlindData;
            P_SocketController.instance.minimumBuyinTableData = minimumBuyin;
        }
        playerCountText.text = dataOfI["totalPlayers"].ToString();

        P_SocketController.instance.SendGetTables(dataOfI["game_id"].ToString());

        /*
        if (!P_WebServices.instance.IsInternetAvailable())
        {
            errorText.text = "No Internet";
            errorText.gameObject.SetActive(true);
        }
        else
        {
            P_LobbySceneManager.instance.ShowScreen(P_LobbyScreens.Loading);

            StartCoroutine(P_WebServices.instance.GETRequestDataURL(P_GameConstant.GAME_URLS[(int)P_RequestType.PokerTableList] + dataOfI["game_id"].ToString(), (string downloadText, bool isError, string errorString) =>
            {
                P_LobbySceneManager.instance.DestroyScreen(P_LobbyScreens.Loading);
                if (isError)
                {
                    if (P_GameConstant.enableErrorLog)
                        Debug.Log("Get game table error: " + errorString);

                    errorText.text = "Error from server";
                    errorText.gameObject.SetActive(true);
                }
                else
                {
                    if (P_GameConstant.enableLog)
                        Debug.Log("Get game table: " + downloadText);

                    JsonData data = JsonMapper.ToObject(downloadText);

                    if (data["data"].Count == 1 || data["data"].Count == 0)
                        tableCountText.text = data["data"].Count + " Table";
                    else
                        tableCountText.text = data["data"].Count + " Tables";

                    if (data["data"].Count > 0)
                    {
                        for (int i = 0; i < data["data"].Count; i++)
                        {
                            int tempI = i;
                        //string categoryData = data["data"][i]["game_type"]["name"].ToString();

                        //if (categoryStr.Equals(categoryData) || (categoryStr.Equals("PLO") && categoryData.Contains(categoryStr)))
                        //{

                        IDictionary iDataI = data["data"][i] as IDictionary;
                            GameObject texas1 = Instantiate(texasWithHeading, scrollContent);
                            P_Lobby_Second_Texas pLobbyTexas1 = texas1.GetComponent<P_Lobby_Second_Texas>();

                            if (iDataI.Contains("table_name") && (data["data"][i]["table_name"] != null))
                                pLobbyTexas1.headingText.text = data["data"][i]["table_name"].ToString();
                            else
                                pLobbyTexas1.headingText.text = "";

                            if (iDataI.Contains("table_attributes"))
                            {
                                IDictionary iDataI2 = data["data"][i]["table_attributes"] as IDictionary;
                                string playerTextDefault = "0/0";
                                if (iDataI2.Contains("players") && iDataI2.Contains("maxPlayers"))
                                    playerTextDefault = data["data"][i]["table_attributes"]["players"].Count + "/" + data["data"][i]["table_attributes"]["maxPlayers"].ToString();
                                else if (iDataI2.Contains("maxPlayers"))
                                    playerTextDefault = "0/" + data["data"][i]["table_attributes"]["maxPlayers"].ToString();

                                pLobbyTexas1.playersText.text = playerTextDefault;
                            }

                            pLobbyTexas1.bgButton.onClick.AddListener(() =>
                            {
                                P_MainSceneManager.instance.ScreenDestroy();
                                P_MainSceneManager.instance.LoadScene(P_MainScenes.InGame);
                                P_SocketController.instance.TABLE_ID = data["data"][tempI]["game_table_id"].ToString();
                                P_SocketController.instance.gameId = data["data"][tempI]["game_id"].ToString();
                                P_SocketController.instance.SendJoinViewer();
                                //P_SocketController.instance.Connect(); //Anwar
                            //P_SocketController.instance.SendJoin(P_SocketController.instance.TABLE_ID, "10000");
                            P_SocketController.instance.tableData = dataOfI;
                                P_SocketController.instance.gameTableData = data["data"][tempI];
                                if (P_InGameManager.instance != null)
                                {
                                    P_InGameUiManager.instance.AllPlayerPosPlusOn();
                                }
                                if (P_InGameUiManager.instance != null)
                                    P_InGameUiManager.instance.tableInfoText.text = data["data"][tempI]["table_name"].ToString();
                                if (P_GameConstant.enableLog)
                                    Debug.Log("Get game table click: " + JsonMapper.ToJson(data["data"][tempI]));

                            });
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
        */
    }

    public void CreateSecondLobby(string responseData) 
    {
        JsonData data = JsonMapper.ToObject(responseData);

        if (data["data"].Count == 1 || data["data"].Count == 0)
            tableCountText.text = data["data"].Count + " Table";
        else
            tableCountText.text = data["data"].Count + " Tables";

        if (data["data"].Count > 0)
        {
            for (int i = 0; i < scrollContent.childCount; i++)
            {
                Destroy(scrollContent.GetChild(i).gameObject);
            }

            int totalPlayingUsers = 0;

            for (int i = 0; i < data["data"].Count; i++)
            {
                int tempI = i;

                IDictionary iDataI = data["data"][i] as IDictionary;
                GameObject texas1 = Instantiate(texasWithHeading, scrollContent);
                P_Lobby_Second_Texas pLobbyTexas1 = texas1.GetComponent<P_Lobby_Second_Texas>();

                if (iDataI.Contains("table_name") && (data["data"][i]["table_name"] != null))
                    pLobbyTexas1.headingText.text = data["data"][i]["table_name"].ToString();
                else
                    pLobbyTexas1.headingText.text = "";

                if (iDataI.Contains("table_attributes"))
                {
                    IDictionary iDataI2 = data["data"][i]["table_attributes"] as IDictionary;
                    string playerTextDefault = "0/0";
                    if (iDataI2.Contains("players") && iDataI2.Contains("maxPlayers"))
                    {
                        playerTextDefault = data["data"][i]["table_attributes"]["players"].Count + "/" + data["data"][i]["table_attributes"]["maxPlayers"].ToString();
                        totalPlayingUsers += data["data"][i]["table_attributes"]["players"].Count;
                    }
                    else if (iDataI2.Contains("maxPlayers"))
                        playerTextDefault = "0/" + data["data"][i]["table_attributes"]["maxPlayers"].ToString();

                    pLobbyTexas1.playersText.text = playerTextDefault;
                }

                pLobbyTexas1.bgButton.onClick.AddListener(() =>
                {
                    if (roomData["game_type"]["name"].ToString() != "PLO 5") //PLO 5 remaining
                    {
                        if (P_GameConstant.enableLog)
                            Debug.Log("TableId: " + data["data"][tempI]["game_id"].ToString()); // + " - " + data["data"][tempI]["game_table_id"].ToString());

                        P_MainSceneManager.instance.ScreenDestroy();
                        P_MainSceneManager.instance.LoadScene(P_MainScenes.InGame);
                        P_SocketController.instance.TABLE_ID = data["data"][tempI]["game_table_id"].ToString();
                        //P_SocketController.instance.TABLE_ID = data["data"][tempI]["tableId"].ToString();
                        P_SocketController.instance.gameId = data["data"][tempI]["game_id"].ToString();
                        P_SocketController.instance.SendJoinViewer();
                        //P_SocketController.instance.Connect(); //Anwar
                        //P_SocketController.instance.SendJoin(P_SocketController.instance.TABLE_ID, "10000");
                        P_SocketController.instance.tableData = roomData;
                        P_SocketController.instance.gameTableData = data["data"][tempI];
                        P_SocketController.instance.gameTypeName = roomData["game_type"]["name"].ToString();
                        if (P_InGameManager.instance != null)
                        {
                            P_InGameUiManager.instance.AllPlayerPosPlusOn();
                        }
                        if (P_InGameUiManager.instance != null)
                            P_InGameUiManager.instance.tableInfoText.text = data["data"][tempI]["table_name"].ToString();
                        if (P_GameConstant.enableLog)
                            Debug.Log("Get game table click: " + JsonMapper.ToJson(data["data"][tempI]));

                        P_SocketController.instance.gameTableMaxPlayers = Int32.Parse(P_SocketController.instance.gameTableData["maxPlayers"].ToString());
                        // remaining: seat hide according to lobby maxPlayers
                        //if (P_SocketController.instance.gameTableMaxPlayers == 6)
                        //{
                        //    for (int i = 0; i < P_InGameManager.instance.allPlayerPos.Length; i++)
                        //    {
                        //        if (P_InGameManager.instance.allPlayerPos[i].gameObject.name == "2" || P_InGameManager.instance.allPlayerPos[i].gameObject.name == "6")
                        //            P_InGameManager.instance.allPlayerPos[i].gameObject.SetActive(false);
                        //    }
                        //}
                        //else if (P_SocketController.instance.gameTableMaxPlayers == 4)
                        //{
                        //    for (int i = 0; i < P_InGameManager.instance.allPlayerPos.Length; i++)
                        //    {
                        //        if (P_InGameManager.instance.allPlayerPos[i].gameObject.name == "1" || P_InGameManager.instance.allPlayerPos[i].gameObject.name == "7" ||
                        //            P_InGameManager.instance.allPlayerPos[i].gameObject.name == "3" || P_InGameManager.instance.allPlayerPos[i].gameObject.name == "5")
                        //            P_InGameManager.instance.allPlayerPos[i].gameObject.SetActive(false);
                        //    }
                        //}
                        //else if (P_SocketController.instance.gameTableMaxPlayers == 2)
                        //{
                        //    for (int i = 0; i < P_InGameManager.instance.allPlayerPos.Length; i++)
                        //    {
                        //        if (P_InGameManager.instance.allPlayerPos[i].gameObject.name == "1" || P_InGameManager.instance.allPlayerPos[i].gameObject.name == "2" ||
                        //            P_InGameManager.instance.allPlayerPos[i].gameObject.name == "3" || P_InGameManager.instance.allPlayerPos[i].gameObject.name == "5" ||
                        //            P_InGameManager.instance.allPlayerPos[i].gameObject.name == "6" || P_InGameManager.instance.allPlayerPos[i].gameObject.name == "7")
                        //            P_InGameManager.instance.allPlayerPos[i].gameObject.SetActive(false);
                        //    }
                        //}
                        //int counterPos = 1;
                        //for (int i = 0; i < P_InGameManager.instance.allPlayerPos.Length; i++)
                        //{
                        //    if (P_InGameManager.instance.allPlayerPos[i].gameObject.activeSelf)
                        //    {
                        //        P_InGameManager.instance.allPlayerPos[i].transform.GetChild(0).GetComponent<P_PlayerSeat>().seatNo = counterPos.ToString();
                        //        counterPos++;
                        //    }
                        //}
                    }
                });
            }
            playerCountText.text = totalPlayingUsers.ToString();
        }
        else
        {
            errorText.text = "Data not found from server";
            errorText.gameObject.SetActive(true);
        }
    }
}
