using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions;

public class L_MainMenuController : MonoBehaviour
{
    public static L_MainMenuController instance;

    public GameObject bottomPanel/*, bottomPanelTeen*/;

    public GameObject[] screens; // All screens prefab
    public Transform[] screenLayers; // screen spawn parent
    public GameObject[] bottomMenus;

    //public HorizontalScrollSnap mainScrollSnap;

    [SerializeField] public List<MainMenuActiveScreen> mainMenuActiveScreens = new List<MainMenuActiveScreen>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //ShowScreen(MainMenuScreens.Dashboard);

        
        if (PlayerManager.instance.IsLogedIn())
        {
            L_GlobalGameManager.playerToken = PlayerManager.instance.GetPlayerGameData().token;
            ShowScreen(MainMenuScreens.Dashboard);
        }
        else
        {
            ShowScreen(MainMenuScreens.Loading);
            StartCoroutine(L_GlobalGameManager.instance.RunAfterDelay(2f, () =>
            {
                DestroyScreen(MainMenuScreens.Loading);
                ShowScreen(MainMenuScreens.Login_SignUp_Otp);
            }));
        }
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            ShowScreen(MainMenuScreens.QuitConfirm);
        }
    }

    public bool IsScreenActive(MainMenuScreens screenName)
    {
        for (int i = 0; i < mainMenuActiveScreens.Count; i++)
        {
            if (mainMenuActiveScreens[i].screenName == screenName)
            {
                return true;
            }
        }

        return false;
    }

    public void DestroyScreen(MainMenuScreens screenName)
    {
        for (int i = 0; i < mainMenuActiveScreens.Count; i++)
        {
            if (mainMenuActiveScreens[i].screenName == screenName)
            {
                Destroy(mainMenuActiveScreens[i].screenObject);
                mainMenuActiveScreens.RemoveAt(i);
            }
        }
    }

    public void DestroyScreen(ScreenLayer layerName)
    {
        for (int i = 0; i < mainMenuActiveScreens.Count; i++)
        {
            if (mainMenuActiveScreens[i].screenLayer == layerName)
            {
                Destroy(mainMenuActiveScreens[i].screenObject);
                mainMenuActiveScreens.RemoveAt(i);
            }
        }
    }

    public void ShowScreen(MainMenuScreens screenName, object[] parameter = null)
    {
        if (screenName == MainMenuScreens.Dashboard || screenName == MainMenuScreens.Tournaments)
        {
            if (L_SoundManager.instance.isSound)
            {
                if (!L_SoundManager.instance.IsSoundPlaying(L_SoundType.MainAppSound, transform))
                    L_SoundManager.instance.PlayLoopSound(L_SoundType.MainAppSound, transform);
            }

            //    SwitchToMainMenu(true);
            //    return;
        }
        else if (screenName == MainMenuScreens.PlayerFinding || screenName == MainMenuScreens.Login_SignUp_Otp || screenName == MainMenuScreens.TournamentPlayerFinding)
        {
            if (L_SoundManager.instance.isSound)
                L_SoundManager.instance.StopLoopSound(L_SoundType.MainAppSound, transform);
        }

        int layer = (int)GetScreenLayer(screenName);
        for (int i = layer /*+ 1*/; i < screenLayers.Length; i++)
        {
            DestroyScreen((ScreenLayer)i);
        }

        if (!IsScreenActive(screenName))
        {
            DestroyScreen(GetScreenLayer(screenName));

            MainMenuActiveScreen mainMenuScreen = new MainMenuActiveScreen();
            mainMenuScreen.screenName = screenName;
            mainMenuScreen.screenLayer = GetScreenLayer(screenName);

            GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;
            mainMenuActiveScreens.Add(mainMenuScreen);
            gm.SetActive(true);
            switch (screenName)
            {

            }

        }
    }

    public ScreenLayer GetScreenLayer(MainMenuScreens screenName)
    {
        switch (screenName)
        {
            //case MainMenuScreens.HomePanel:
            //case MainMenuScreens.ProfileFeedPanel:
            case MainMenuScreens.Dashboard:
            case MainMenuScreens.ClassicLudoGamePlay:
            case MainMenuScreens.QuickLudoSelection:
            case MainMenuScreens.PlayerFinding:
            case MainMenuScreens.Tournaments:
            case MainMenuScreens.TournamentJoin:
            case MainMenuScreens.TournamentGamePlay:
                return ScreenLayer.LAYER1;

            //case MainMenuScreens.SigningPanel:
            //case MainMenuScreens.Message:
            //    return ScreenLayer.LAYER3;

            //case MainMenuScreens.ProductAddAddressPage:
            //    return ScreenLayer.LAYER4;

            case MainMenuScreens.Login_SignUp_Otp:
            case MainMenuScreens.ReferralConfirm:
            case MainMenuScreens.ReferralInput:
            case MainMenuScreens.Profile:
            case MainMenuScreens.HelpDesk:
            case MainMenuScreens.Wallet:
            case MainMenuScreens.Settings:
            case MainMenuScreens.NameUpdate:
            case MainMenuScreens.PrivacyPolicy:
            case MainMenuScreens.AboutUs:
            case MainMenuScreens.RefundNCancellations:
            case MainMenuScreens.GamePlayInstruction:
            case MainMenuScreens.Leaderboard:
            case MainMenuScreens.ReferAndEarn:
            case MainMenuScreens.Winner:
            case MainMenuScreens.TournamentPlayerFinding:
            case MainMenuScreens.TournamentNextRound:
            case MainMenuScreens.TournamentWinning:
                return ScreenLayer.LAYER3;

            case MainMenuScreens.KYC:
            case MainMenuScreens.ReferralHistory:
            case MainMenuScreens.GameHistory:
            case MainMenuScreens.TermsNCondition:
            case MainMenuScreens.Rules:
            case MainMenuScreens.Loading:
                return ScreenLayer.LAYER4;

            case MainMenuScreens.QuitConfirm:
                return ScreenLayer.LAYER5;

            default:
                return ScreenLayer.LAYER2;
        }
    }






    // for common use
    public void GetProfilePictureLink(string keyStr, Image profileImg)
    {
        StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.UploadProfilePicture] + "?key=" + keyStr, (serverResponse, errorBool, error) =>
        {
            if (errorBool)
            {
                Debug.Log("Error in Profile Picture Link: " + error);
            }
            else
            {
                Debug.Log("Profile Picture Link response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("code"))
                {
                    if (data["code"].ToString() == "200")
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(data["data"]["url"].ToString()))
                            {
                                if (
                                (string.IsNullOrEmpty(PlayerManager.instance.GetPlayerGameData().avatarURL)) ||
                                (!PlayerManager.instance.GetPlayerGameData().avatarURL.Equals(data["data"]["url"].ToString()))
                                )
                                {
                                    Debug.Log("PIC way 1");
                                    // set new url into Player....avatarURL & load image

                                    try
                                    {
                                        SetImageFromURL(data["data"]["url"].ToString(), profileImg);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.Log("Error in Load Profile Picture: " + e.Message);
                                    }

                                    Debug.Log("PIC way 1 SET " + PlayerManager.instance.GetPlayerGameData().avatarURL);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log("error in Profile Picture Link " + e.Message);
                        }
                    }
                    else
                    {
                        Debug.Log("Error in Profile Picture Link 2: " + error);
                    }
                }
                else
                {
                    Debug.Log("Error in Profile Picture Link 3: " + error);
                }
            }
        }));
    }

    public void SetImageFromURL(string URL, Image img)
    {
        PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
        playerData.avatarURL = URL;
        PlayerManager.instance.SetPlayerGameData(playerData);
        Davinci.get().setFadeTime(0).load(playerData.avatarURL).into(img).start();
    }

    public void PlayButtonSound()
    {
        if (L_SoundManager.instance.isSound)
            L_SoundManager.instance.PlaySound(L_SoundType.ButtonSound, L_MainMenuController.instance.transform);
    }
}

[Serializable]
public class MainMenuActiveScreen
{
    public GameObject screenObject;
    public MainMenuScreens screenName;
    public ScreenLayer screenLayer;
}

public enum MainMenuScreens
{
    Loading,
    Login_SignUp_Otp,
    Dashboard,
    ReferralConfirm,
    ReferralInput,
    ReferralHistory,
    Profile,
    KYC,
    GameHistory,
    HelpDesk,
    Wallet,
    Settings,
    NameUpdate,
    PrivacyPolicy,
    AboutUs,
    RefundNCancellations,
    TermsNCondition,
    GamePlayInstruction,
    QuitConfirm,
    Rules,
    QuickLudoSelection,
    Leaderboard,
    ReferAndEarn,
    PlayerFinding,
    Winner,
    ClassicLudoGamePlay,
    Tournaments,
    TournamentJoin,
    TournamentPlayerFinding,
    TournamentNextRound,
    TournamentWinning,
    TournamentGamePlay,
}

public enum ScreenLayer
{
    LAYER1,
    LAYER2,
    LAYER3,
    LAYER4,
    LAYER5,
}
