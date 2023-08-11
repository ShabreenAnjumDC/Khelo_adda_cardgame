using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class P_LobbySceneManager : MonoBehaviour
{
    public static P_LobbySceneManager instance;
    public GameObject[] screens; // All screens prefab
    public Transform[] screenLayers; // screen spawn parent
    private List<P_LobbyActiveScreen> mainActiveScreensList = new List<P_LobbyActiveScreen>();
    public GameObject[] activeBottomIcons;
    public GameObject[] deactiveBottomIcons;
    //public GameObject bottomMenu, drawerMenu;
    //public GameObject menuList;


    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        if (P_GameConstant.enableLog)
            Debug.Log("Saved " + PlayerManager.instance.IsLogedIn());

        //bottomMenu.SetActive(false);
        ShowScreen(P_LobbyScreens.Lobby);
    }




    public void ShowScreen(P_LobbyScreens screenName, object[] parameter = null)
    {
        int layer = (int)GetScreenLayer(screenName);
        for (int i = layer + 1; i < screenLayers.Length; i++)
        {
            DestroyScreen((P_ScreenLayer)i);
        }

        if (!IsScreenActive(screenName))
        {
            DestroyScreen(GetScreenLayer(screenName));

            P_LobbyActiveScreen mainActiveScreen = new P_LobbyActiveScreen();
            mainActiveScreen.screenName = screenName;
            mainActiveScreen.screenLayer = GetScreenLayer(screenName);

            GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainActiveScreen.screenLayer]) as GameObject;
            mainActiveScreen.screenObject = gm;
            mainActiveScreensList.Add(mainActiveScreen);
            gm.SetActive(true);
            switch (screenName)
            {
                /*case P_LobbyActiveScreen.:
                    {
                       
                    }
                    break;*/

                default:
                    break;
            }

        }
    }

    P_ScreenLayer GetScreenLayer(P_LobbyScreens screenName)
    {
        switch (screenName)
        {
            case P_LobbyScreens.Lobby:
                return P_ScreenLayer.LAYER1;
            //case P_LobbyScreens.:
            //    return P_ScreenLayer.LAYER3;
            //case P_LobbyScreens.:
            //    return P_ScreenLayer.LAYER4;
            case P_LobbyScreens.Message:
            case P_LobbyScreens.Loading:
                return P_ScreenLayer.LAYER5;
            default:
                return P_ScreenLayer.LAYER2;
        }
    }

    bool IsScreenActive(P_LobbyScreens screenName)
    {
        for (int i = 0; i < mainActiveScreensList.Count; i++)
        {
            if (mainActiveScreensList[i].screenName == screenName)
            {
                return true;
            }
        }

        return false;
    }

    public void DestroyScreen(P_LobbyScreens screenName)
    {
        for (int i = 0; i < mainActiveScreensList.Count; i++)
        {
            if (mainActiveScreensList[i].screenName == screenName)
            {
                Destroy(mainActiveScreensList[i].screenObject);
                mainActiveScreensList.RemoveAt(i);
            }
        }
    }

    public void DestroyScreen(P_ScreenLayer layerName)
    {
        for (int i = 0; i < mainActiveScreensList.Count; i++)
        {
            if (mainActiveScreensList[i].screenLayer == layerName)
            {
                Destroy(mainActiveScreensList[i].screenObject);
                mainActiveScreensList.RemoveAt(i);
            }
        }
    }
}

public class P_LobbyActiveScreen
{
    public GameObject screenObject;
    public P_LobbyScreens screenName;
    public P_ScreenLayer screenLayer;
}

public enum P_ScreenLayer
{
    LAYER1,
    LAYER2,
    LAYER3,
    LAYER4,
    LAYER5
}

public enum P_LobbyScreens
{
    Lobby,
    LobbySecond,
    Loading,
    Message,
}
