using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class QuickLudoSelection : MonoBehaviour
{
    public static QuickLudoSelection instance;

    [Header("-----classic or quick-----")]
    public bool isClassicLudo, isQuickLudo;
    public Text titleText;
    public Text info1Text;
    public Text info2Text;

    [Header("-----selection amount-----")]
    public GameObject amountItemPrefab;
    public Transform amountItemContent;
    public ToggleGroup amountItemToggleGroup;
    public ToggleGroup gotiColorToggleGroup;

    [Space(10)]
    public Button playButton;

    public string gameTypeId;
    public string gameVarientId;
    public string gameAmount;
    public InputField userIdInput;
    public Text winAmountText;
    public TMP_Text errorTMP;

    void Awake()
    {
        instance = this;
    }


    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "close":
                if (L_SocketController.instance != null)
                {
                    if (L_SocketController.instance.IsSocketOpen())
                    {
                        if (L_SocketController.instance.isRegisterSend)
                        {
                            L_SocketController.instance.RemovePlayerFromGame();
                        }
                    }
                }
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                if (L_GlobalGameManager.instance.currentScreenPathList.Contains("ClassicLudoSelection") || L_GlobalGameManager.instance.currentScreenPathList.Contains("QuickLudoSelection"))
                    L_GlobalGameManager.instance.currentScreenPathList.Clear();
                Debug.Log("list clear count=" + L_GlobalGameManager.instance.currentScreenPathList.Count);
                break;

            case "play":
                //if (string.IsNullOrEmpty(userIdInput.text))
                //{
                //    StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter user id", "red", 2f));
                //}
                //else
                //{
                    playButton.enabled = false;
                    Toggle toggleAmount = GetSelectedToggle(amountItemToggleGroup);
                    gameAmount = toggleAmount.transform.GetChild(0).GetChild(1).GetComponent<Text>().text.Substring(1);
                    Debug.Log("selected amount: " + gameAmount);
                    gameVarientId = toggleAmount.transform.GetChild(0).GetChild(2).GetComponent<Text>().text;
                    Debug.Log("selected goti id: " + toggleAmount.transform.GetChild(0).GetChild(2).GetComponent<Text>().text);

                    Toggle toggleGotiColor = GetSelectedToggle(gotiColorToggleGroup);
                    Debug.Log("selected goti color: " + toggleGotiColor.name);


                    if (isClassicLudo)
                    {
                        //StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameVarientForGameId] + "?type="+ gameTypeId +"&varient="+ gameVarientId, (serverResponse, errorBool, error) => {
                        StartCoroutine(WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameVarientForGameId] + "?type="+ gameTypeId +"&varient="+ gameVarientId, (serverResponse, errorBool, error) => {
                            if (errorBool)
                            {
                                Debug.Log("Error in Game Varient: " + error);
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response", "red", 2f));
                                playButton.enabled = true;
                            }
                            else
                            {
                                Debug.Log("Game Id response: " + serverResponse);
                                JsonData data = JsonMapper.ToObject(serverResponse);

                                IDictionary iData1 = data as IDictionary;

                                if (iData1.Contains("code"))
                                {
                                    if (data["code"].ToString() == "200")
                                    {
                                        if (data["data"].Count == 1)
                                        {
                                            string gameId = data["data"][0]["id"].ToString();
                                            if (!string.IsNullOrEmpty(gameId))
                                            {
                                                L_GlobalGameManager.instance.socketController.enabled = true;
                                                L_GlobalGameManager.instance.socketController.gameObject.SetActive(true);

                                                L_SocketController.instance.selectedGotiColor = toggleGotiColor.name;
                                                L_SocketController.instance.isClassicLudo = true;
                                                L_SocketController.instance.isQuickLudo = false;
                                                L_SocketController.instance.gameAmount = gameAmount;
                                                L_SocketController.instance.gameTypeId = gameTypeId;
                                                L_SocketController.instance.gameVarientId = gameVarientId;

                                                L_GlobalGameManager.instance.socketController.RequestRegisterForGame(gameId, gameVarientId); //userIdInput.text
                                                L_GlobalGameManager.instance.currentScreenPathList.Add("ClassicLudoSelection");
                                            }
                                            else
                                            {
                                                Debug.Log("Enter in game response...");
                                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response...", "red", 2f));
                                                playButton.enabled = true;
                                            }
                                        }
                                        else
                                        {
                                            Debug.Log("Enter in game response!");
                                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response!", "red", 2f));
                                            playButton.enabled = true;
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("Enter in game response!!");
                                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response!!", "red", 2f));
                                        playButton.enabled = true;
                                    }
                                }
                                else
                                {
                                    Debug.Log("Enter in game response!!!");
                                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response!!!", "red", 2f));
                                    playButton.enabled = true;
                                }
                            }
                        }));
                    }
                    else
                    {
                        //StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameVarientForGameId] + "?type=" + gameTypeId + "&varient=" + gameVarientId, (serverResponse, errorBool, error) => {
                        StartCoroutine(WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameVarientForGameId] + "?type=" + gameTypeId + "&varient=" + gameVarientId, (serverResponse, errorBool, error) => {
                        if (errorBool)
                        {
                            Debug.Log("Error in Game Varient: " + error);
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response", "red", 2f));
                            playButton.enabled = true;
                        }
                        else
                        {
                            Debug.Log("Game Id response: " + serverResponse);
                            JsonData data = JsonMapper.ToObject(serverResponse);

                            IDictionary iData1 = data as IDictionary;

                            if (iData1.Contains("code"))
                            {
                                if (data["code"].ToString() == "200")
                                {
                                    if (data["data"].Count == 1)
                                    {
                                        string gameId = data["data"][0]["id"].ToString();
                                        if (!string.IsNullOrEmpty(gameId))
                                        {
                                            L_GlobalGameManager.instance.socketController.enabled = true;
                                            L_GlobalGameManager.instance.socketController.gameObject.SetActive(true);

                                            L_SocketController.instance.selectedGotiColor = toggleGotiColor.name;
                                            L_SocketController.instance.isQuickLudo = true;
                                            L_SocketController.instance.isClassicLudo = false;
                                            L_SocketController.instance.gameAmount = gameAmount;
                                            L_SocketController.instance.gameTypeId = gameTypeId;
                                            L_SocketController.instance.gameVarientId = gameVarientId;

                                            L_GlobalGameManager.instance.socketController.RequestRegisterForGame(gameId, gameVarientId); //userIdInput.text
                                            L_GlobalGameManager.instance.currentScreenPathList.Add("QuickLudoSelection");
                                        }
                                        else
                                        {
                                            Debug.Log("Enter in game response...");
                                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response...", "red", 2f));
                                            playButton.enabled = true;
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("Enter in game response!");
                                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response!", "red", 2f));
                                        playButton.enabled = true;
                                    }
                                }
                                else
                                {
                                    Debug.Log("Enter in game response!!");
                                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response!!", "red", 2f));
                                    playButton.enabled = true;
                                }
                            }
                            else
                            {
                                Debug.Log("Enter in game response!!!");
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorTMP, "Enter in game response!!!", "red", 2f));
                                playButton.enabled = true;
                            }
                        }
                    }));
                    //GlobalGameManager.instance.currentScreenPathList.Add("QuickLudoSelection");
                    //    MainMenuController.instance.ShowScreen(MainMenuScreens.PlayerFinding);
                    }
                //}
                break;
        }
    }


    public void OnPlayerAddedReceived()
    {
        L_MainMenuController.instance.ShowScreen(MainMenuScreens.PlayerFinding);
        PlayerFinding.instance.enterAmount.text = gameAmount;
        //int amountInt = 0;
        //if (int.TryParse(gameAmount, out amountInt))
        //    PlayerFinding.instance.priceMoney.text = "$"+amountInt.ToString();
        PlayerFinding.instance.priceMoney.text = "₹" + gameAmount;  //now priceMoney set as enter amount (entry fees)
    }

    void Start()
    {
        if (isClassicLudo)
        {
            titleText.text = "CLASSIC LUDO";
            info1Text.text = "IN CLASSIC LUDO, TAKE YOUR 4 GOTIS/PAWNS HOME";
            //info1Text.text = "IN CLASSIC LUDO, TAKE YOUR 4 GOTIS/\nPAWNS HOME";

            //GlobalGameManager.instance.socketController.enabled = true;
            //GlobalGameManager.instance.socketController.gameObject.SetActive(true);
        }
        else
        {
            titleText.text = "QUICK LUDO";
            info1Text.text = "IN QUICK LUDO, TAKE ONLY 2 GOTIS/PAWNS HOME";
            //info1Text.text = "IN QUICK LUDO, TAKE ONLY 2 GOTIS/\nPAWNS HOME";

            //GlobalGameManager.instance.socketController.enabled = true;
            //GlobalGameManager.instance.socketController.gameObject.SetActive(true);
        }

        //StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameVarient], (serverResponse, errorBool, error) =>
        StartCoroutine(WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameVarient], (serverResponse, errorBool, error) =>
        {
            if (errorBool)
            {
                Debug.Log("Error in Game Varient: " + error);
            }
            else
            {
                Debug.Log("Game Varient response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("data"))
                {
                    for (int i = 0; i < amountItemContent.childCount; i++)
                    {
                        Destroy(amountItemContent.GetChild(i));
                    }

                    for (int i = 0; i < data["data"].Count; i++)
                    {
                        if (data["data"][i]["status"].ToString() == "1")
                        {
                            int tempI = i;
                            GameObject obj = Instantiate(amountItemPrefab, amountItemContent);
                            obj.name = tempI.ToString();
                            obj.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "₹" + data["data"][i]["value"].ToString();
                            obj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = data["data"][i]["id"].ToString();
                            obj.GetComponent<Toggle>().group = amountItemToggleGroup;
                            obj.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                                OnAmountSelection(obj.GetComponent<Toggle>(), data["data"][tempI]["value"].ToString());
                            });
                            if (tempI == 0)
                                obj.GetComponent<Toggle>().isOn = true;
                        }
                    }
                    //winAmountText.text = "$" + AmountDoubleFunc(data["data"][0]["value"].ToString()).ToString();
                }
            }
        }));
    }

    Toggle GetSelectedToggle(ToggleGroup tg)
    {
        Toggle[] toggles = tg.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles)
            if (t.isOn) return t;  //returns selected toggle
        return null;           // if nothing is selected return null
    }

    void OnAmountSelection(Toggle change, string value)
    {
        if (change.isOn)
        {
            //Debug.Log("New Value : " + change.transform.GetChild(0).GetChild(1).GetComponent<Text>().text);
            //Debug.Log("Value : " + value);
            //winAmountText.text = "$" + AmountDoubleFunc(value).ToString();
        }
    }

    int AmountDoubleFunc(string amount)
    {
        int amountInt = 0;
        if (int.TryParse(amount, out amountInt))
            return (amountInt + amountInt);
        else
            return amountInt;
    }
}
