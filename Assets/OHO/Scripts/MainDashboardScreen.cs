using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainDashboardScreen : MonoBehaviour
{
    public class MainDashboardActiveScreen
    {
        public GameObject screenObject;
        public MainDashboardScreens screenName;
        public ScreenLayer screenLayer;
    }

    public enum ScreenLayer
    {
        LAYER1,
        LAYER2,
        LAYER3,
        LAYER4,
        LAYER5
    }

    public enum MainDashboardScreens
    {
        Registration,
        Loading,
        Message,
        AddCash,
        UserProfile,
        EditProfile,
        PrivacyPolicy,
        TermsAndConditions,
        GamePolicy
    }

    public static MainDashboardScreen instance;
    public GameObject[] screens; // All screens prefab
    public Transform[] screenLayers; // screen spawn parent
    private List<MainDashboardActiveScreen> mainDashboardActiveScreens = new List<MainDashboardActiveScreen>();
    public Text userNameText, drawerUsernameText;
    public Image profilePic, frameImage;
    public GameObject[] activeBottomIcons;
    public GameObject[] deactiveBottomIcons;
    public GameObject[] bottomScreens;
    public GameObject bottomMenu, drawerMenu;
    public GameObject menuList;

    [Header("-------webView Contact and About-------")]
    public GameObject webViewContactAboutObjParent;
    public GameObject webViewContactAboutObj;
    //public UniWebView webViewContactAbout;

    public GameObject UniWebViewObject;
    //public UniWebView UniWebView;
    private bool register, logIn;

    void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        userNameText.text = PlayerManager.instance.GetPlayerGameData().userName;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Saved " + PlayerManager.instance.IsLogedIn());
      
        if (PlayerManager.instance.IsLogedIn())
        {
            //GlobalGameManager.instance.LoadScene(Scenes.MainDashboard);
            GlobalGameManager.token = PrefsManager.GetPlayerData().token;
            GetUserDetails();
            bottomMenu.SetActive(true);
            logIn = true; //NL
            ShowScreen(MainDashboardScreens.Loading); //NL
            Invoke(nameof(GameStart), .4f); //NL
        }
        else
        {
            bottomMenu.SetActive(false);
            register = true; // NL
            ShowScreen(MainDashboardScreens.Loading);  //NL
            Invoke(nameof(GameStart), .4f);  //NL

        // OL //     ShowScreen(MainDashboardScreens.Registration);
        }

        /*UniWebView.OnMessageReceived += (view, message) => {
            Debug.Log("Message " + message.Path);
            if (message.Path.Equals("close"))
            {
                CloseWebView();
                //Destroy(UniWebView);
                //UniWebView = null;
            }
        };*/
    }
    public void GameStart()
    {
        if (logIn)
        {
            DestroyScreen(MainDashboardScreens.Loading);
            logIn = false;
        }

        if (register)
        {
            DestroyScreen(MainDashboardScreens.Loading);
            ShowScreen(MainDashboardScreens.Registration);
            register = false;
        }
    }

    public void GetUserDetails()
    {
        PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
        //WebServices.instance.SendRequest(RequestType.GetUserDetails, "{\"userId\":\"" + playerData.userId + "\"}", true, OnServerResponseFound);
        StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/profile"/* + PlayerManager.instance.GetPlayerGameData().userId*/, UserDetailsResponse));
    }

    void UserDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => UserDetails: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (data["statusCode"].ToString() == "200")
        {
            PlayerGameDetails playerData = Utility.ParseUserDetails(data);
            PlayerManager.instance.SetPlayerGameData(playerData);
            UpdateUserDetails(data);
        }
        else
        {
            Debug.LogError(data["error"].ToString());
        }
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log(requestType + " - " + errorMessage.Length + " - Response => GetUserDetails - " + serverResponse);
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                //Debug.LogError("111111111111111111111111111111");
                MainDashboardScreen.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.GetUserDetails)
        {

            //GetUserDetails userdata = JsonUtility.FromJson<GetUserDetails>(serverResponse);

            //Debug.Log("Response => GetUserDetails :" + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                UpdateUserDetails(data);
            }
            else
            {
                ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.FantasyLogin)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["ResponseCode"].ToString() == "200")
            {
                Debug.Log("Response => FantasyLogin :" + serverResponse + " - " + data["Data"]["SessionKey"].ToString());
                InitialiseWebView(data["Data"]["SessionKey"].ToString());
            }
            else
            {
                ShowMessage(data["Message"].ToString());
            }
        }
    }

    public void UpdateUserDetails(JsonData data)
    {
        PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
        playerData.userName = userNameText.text = drawerUsernameText.text = data["data"]["username"].ToString();

        /*for (int i = 0; i < data["getData"].Count; i++)
        {
            PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
            playerData.userName = userNameText.text = data["getData"][i]["userName"].ToString();
            if (data["getData"][i]["nickName"] != null)
                playerData.nickName = data["getData"][i]["nickName"].ToString();
            if (data["getData"][i]["userId"] != null)
                playerData.userId = data["getData"][i]["userId"].ToString();
            playerData.userEmail = data["getData"][i]["emailId"].ToString();
            playerData.isEmailVerified = data["getData"][i]["isEmailVerified"].ToString();
            if (data["getData"][i]["mobile"] != null)
                playerData.userMobile = data["getData"][i]["mobile"].ToString();
            playerData.userDob = data["getData"][i]["dob"].ToString();
            if (data["getData"][i]["address"] != null)
                playerData.userAddress = data["getData"][i]["address"].ToString();
            if (data["getData"][i]["gender"] != null)
                playerData.userGender = data["getData"][i]["gender"].ToString();
            Debug.Log("UserID -> " + playerData.userGender);
            if (data["getData"][i]["countryCode"] != null)
                playerData.countryCode = data["getData"][i]["countryCode"].ToString();
            if (data["getData"][i]["countryName"] != null)
                playerData.countryName = data["getData"][i]["countryName"].ToString();
            if (data["getData"][i]["profileImage"] != null)
                playerData.avatarURL = data["getData"][i]["profileImage"].ToString();
            if (data["getData"][i]["frameURL"] != null)
                playerData.FrameUrl = data["getData"][i]["frameURL"].ToString();
            if (data["getData"][i]["countryFlag"] != null)
                playerData.CountryURL = data["getData"][i]["countryFlag"].ToString();
            PlayerManager.instance.GetPlayerGameData().coins = float.Parse(data["getData"][i]["coins"].ToString());
            playerData.isMobileVerified = data["getData"][i]["isMobileVerified"].ToString();

            StartCoroutine(LoadSpriteImageFromUrl(playerData.avatarURL, profilePic));
            StartCoroutine(LoadSpriteImageFromUrl(playerData.FrameUrl, frameImage));
            if (data["getData"][i]["avatarID"] != null)
                playerData.avatarID = int.Parse(data["getData"][i]["avatarID"].ToString());
            if (data["getData"][i]["frameID"] != null)
                playerData.frameID = int.Parse(data["getData"][i]["frameID"].ToString());
        }*/
    }

    IEnumerator LoadSpriteImageFromUrl(string URL, Image image)
    {
        Debug.Log("Pic url " + URL);
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.LogError("Download failed");
        }
        else
        {
            var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
            Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);
            image.sprite = sprite;
            PlayerManager.instance.GetPlayerGameData().userPic = sprite;
            Debug.Log("Successfully Set Player Profile");
        }
    }

    public void ShowScreen(MainDashboardScreens screenName, object[] parameter = null)
    {
        int layer = (int)GetScreenLayer(screenName);
        for (int i = layer + 1; i < screenLayers.Length; i++)
        {
            DestroyScreen((ScreenLayer)i);
        }

        if (!IsScreenActive(screenName))
        {
            DestroyScreen(GetScreenLayer(screenName));

            MainDashboardActiveScreen mainDashboardScreen = new MainDashboardActiveScreen();
            mainDashboardScreen.screenName = screenName;
            mainDashboardScreen.screenLayer = GetScreenLayer(screenName);

            GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainDashboardScreen.screenLayer]) as GameObject;
            mainDashboardScreen.screenObject = gm;
            mainDashboardActiveScreens.Add(mainDashboardScreen);
            gm.SetActive(true);
            switch (screenName)
            {
                /*case MainMenuScreens.GlobalTournament:
                    {
                        if (parameter != null)
                        {
                            gm.GetComponent<GlobalTournamentListUiManager>().ShowScreen((string)parameter[0]);
                        }
                        else
                        {
                            gm.GetComponent<GlobalTournamentListUiManager>().ShowScreen();
                        }
                    }
                    break;*/

                default:
                    break;
            }

        }
    }

    ScreenLayer GetScreenLayer(MainDashboardScreens screenName)
    {
        switch (screenName)
        {
            case MainDashboardScreens.Registration:
            //case MainDashboardScreens.ProfileModification:
                return ScreenLayer.LAYER1;
            //case MainDashboardScreens.SelectFrom:
            //case MainDashboardScreens.WebView_FeedBackAboutUs:
            //    return ScreenLayer.LAYER3;
            //case MainDashboardScreens.ChangeProfileIcon:           
            //case MainDashboardScreens.SelectRegion:
            //    return ScreenLayer.LAYER4;
            case MainDashboardScreens.Message:
            case MainDashboardScreens.Loading:
                return ScreenLayer.LAYER5;
            default:
                return ScreenLayer.LAYER2;
        }
    }

    bool IsScreenActive(MainDashboardScreens screenName)
    {
        for (int i = 0; i < mainDashboardActiveScreens.Count; i++)
        {
            if (mainDashboardActiveScreens[i].screenName == screenName)
            {
                return true;
            }
        }

        return false;
    }

    public void DestroyScreen(MainDashboardScreens screenName)
    {
        for (int i = 0; i < mainDashboardActiveScreens.Count; i++)
        {
            if (mainDashboardActiveScreens[i].screenName == screenName)
            {
                Destroy(mainDashboardActiveScreens[i].screenObject);
                mainDashboardActiveScreens.RemoveAt(i);
            }
        }
    }

    public void DestroyScreen(ScreenLayer layerName)
    {
        for (int i = 0; i < mainDashboardActiveScreens.Count; i++)
        {
            if (mainDashboardActiveScreens[i].screenLayer == layerName)
            {
                Destroy(mainDashboardActiveScreens[i].screenObject);
                mainDashboardActiveScreens.RemoveAt(i);
            }
        }
    }

    public void ShowMessage(string messageToShow, Action callBackMethod = null, string okButtonText = "Ok")
    {
        if (!IsScreenActive(MainDashboardScreens.Message))
        {
            MainDashboardActiveScreen mainDashboardScreen = new MainDashboardActiveScreen();
            mainDashboardScreen.screenName = MainDashboardScreens.Message;
            mainDashboardScreen.screenLayer = GetScreenLayer(MainDashboardScreens.Message);

            GameObject gm = Instantiate(screens[(int)MainDashboardScreens.Message], screenLayers[(int)mainDashboardScreen.screenLayer]) as GameObject;
            mainDashboardScreen.screenObject = gm;

            mainDashboardActiveScreens.Add(mainDashboardScreen);

            gm.GetComponent<MessageScript>().ShowSingleButtonPopUp(messageToShow, callBackMethod, okButtonText);
        }
    }

    public void OpenGame(string gameName)
    {
        switch (gameName)
        {
            case "Poker":
                GlobalGameManager.instance.LoadScene(Scenes.PokerController);
                break;
            case "Trivia":
                //GlobalGameManager.instance.LoadScene(Scenes.TriviaController);
                break;
            case "Ludo":
                //GlobalGameManager.instance.LoadScene(Scenes.LudoMainMenu);
                SceneManager.LoadScene("MainScene");
                break;
            case "Rummy":
                //MainDashboardScreen.instance.ShowScreen(MainDashboardScreens.Loading);
                //gameObject.transform.Find("Canvas/LAYER_5/Loading(Clone)/Image").GetComponent<Image>().enabled = false;
                //GlobalGameManager.instance.LoadScene(Scenes.RummyController);
                ////SceneManager.LoadScene("Rummy");
                break;
            case "Fantasy":
                LoginIntoFantasyGame();
                break;
        }
    }

    public void Logout()
    {
        drawerMenu.SetActive(false);
        PlayerPrefs.DeleteAll();
        PlayerManager.instance.DeletePlayerGameData();
        PrefsManager.DeletePlayerData();
        GlobalGameManager.instance.isLoginShow = false;
        bottomMenu.SetActive(false);
        ShowScreen(MainDashboardScreens.Registration);
    }

    public void MenuSelection(int ind)
    {
        for (int i = 0; i < 2; i++)
        {
            //activeBottomIcons[i].SetActive(false);
            //deactiveBottomIcons[i].SetActive(true);
            bottomScreens[i].SetActive(false);
        }
        //activeBottomIcons[ind - 1].SetActive(true);
        //deactiveBottomIcons[ind - 1].SetActive(false);
        bottomScreens[ind].SetActive(true);

        if (ind == 0)
            bottomMenu.SetActive(true);
        else
            bottomMenu.SetActive(false);
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);


        switch (eventName)
        {

            case "menu":
                {
                    MenuSelection(1);
                    //DestroyScreen(MainDashboardScreens.wallet);
                    //DestroyScreen(MainDashboardScreens.ProfileModification);
                }
                break;

            case "wallet":
                {
                    if (PlayerPrefs.GetString("GuestLogin") == "Yes")
                    {
                        ShowMessage("Create an account to update wallet data!");
                    }
                    else
                    {
                        MenuSelection(2);
                        //ShowScreen(MainDashboardScreens.wallet);
                        //DestroyScreen(MainDashboardScreens.ProfileModification);
                    }
                }
                break;

            case "profilemodification":
                {
                    //menuList.SetActive(false);
                    if (PlayerPrefs.GetString("GuestLogin") == "Yes")
                    {
                        ShowMessage("Create an account to update profile data!");
                    }
                    else
                    {
                        MenuSelection(3);
                        //ShowScreen(MainDashboardScreens.ProfileModification);
                    }
                }
                break;

            case "feedback":
                /*#if UNITY_ANDROID || UNITY_IOS
                    ShowScreen(MainDashboardScreens.WebView_FeedBackAboutUs);
                    if (WebView_FeedBackAboutUs.instance!=null)
                    {
                        WebView_FeedBackAboutUs.instance.url = "https://funasia.net/contactus";
                    }
                #else
                    Application.OpenURL("https://funasia.net/contactus");
                #endif*/
                break;

            case "aboutus":
                /*#if UNITY_ANDROID || UNITY_IOS
                    ShowScreen(MainDashboardScreens.WebView_FeedBackAboutUs);
                    if (WebView_FeedBackAboutUs.instance!=null)
                    {
                        WebView_FeedBackAboutUs.instance.url = "https://funasia.net/contactus";
                    }
                #else
                    Application.OpenURL("https://funasia.net/contactus");
                #endif*/
                break;

            case "shop":
                {
                    //MenuSelection(0);
                    //ShowScreen(MainMenuScreens.Shop);
                }
                break;
            case "Forum":
                {
                    //MenuSelection(1);
                    //ShowScreen(MainMenuScreens.Forum);
                }
                break;
            case "AddCash":
                {
                    ShowScreen(MainDashboardScreens.AddCash);
                }
                break;
            case "Withdraw":
                {
                    WalletScreen.instance.WithdrawUserCash();
                }
                break;
            case "UserProfile":
                {
                    ShowScreen(MainDashboardScreens.UserProfile);
                    bottomMenu.SetActive(false);
                    drawerMenu.SetActive(false);
                }
                break;

            default:
#if ERROR_LOG
                Debug.LogError("unhdnled eventName found in MainMenuController = " + eventName);
#endif
                break;
        }

    }

    public void LoginIntoFantasyGame()
    {
        string requestData = "{\"Keyword\":\"" + PlayerManager.instance.GetPlayerGameData().userEmail + "\"," +
                             "\"Password\":\"" + PlayerPrefs.GetString("Password") + "\"," +
                              "\"Source\":\"" + "Direct" + "\"," +
                              "\"DeviceType\":\"" + "Native" + "\"}";

        WebServices.instance.SendRequest(RequestType.FantasyLogin, requestData, true, OnServerResponseFound);
    }

    public void InitialiseWebView(string payLink)
    {
        string url = "http://65.1.233.18/lobby?SessionKey=" + payLink;
        Debug.Log("url = " + url);
        UniWebViewObject.SetActive(true);
        //UniWebView.CleanCache();
        //UniWebView.SetShowSpinnerWhileLoading(true);
        //UniWebView.SetSpinnerText("Loading. Please do not close or go back.");
        //UniWebView.Load(url);
        //UniWebView.Show(true);
    }

    public void CloseWebView()
    {
        //DestroyScreen(MainDashboardScreens.WebView_FeedBackAboutUs);
    }
 
    public void CloseWebViewAfterWait()
    {
        //StartCoroutine(GlobalGameManager.instance.RunAfterDelay(1f, () => {
        //    DestroyScreen(MainDashboardScreens.WebView_FeedBackAboutUs);
        //}));
    }

    // Drawer Menu

    public void OpenTermsAndConditionsScreen()
    {
        ShowScreen(MainDashboardScreens.TermsAndConditions);
    }

    public void OpenGamePolicyScreen()
    {
        ShowScreen(MainDashboardScreens.GamePolicy);
    }

    public void OpenPrivacyPolicyScreen()
    {
        ShowScreen(MainDashboardScreens.PrivacyPolicy);
    }
}
