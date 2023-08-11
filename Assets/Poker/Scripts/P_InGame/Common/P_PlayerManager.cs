using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_PlayerManager : MonoBehaviour
{

    public static P_PlayerManager instance;

    [SerializeField]
    private P_PlayerGameDetails playerGameData = null; // contains user details


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerGameData = P_PrefsManager.GetPlayerData();
    }

    public void DeletePlayerGameData()
    {
        playerGameData = null;
    }

    public P_PlayerGameDetails GetPlayerGameData()
    {
        return playerGameData;
    }

    public void SetPlayerGameData(P_PlayerGameDetails dataToAssign)
    {
        playerGameData = dataToAssign;
        P_PrefsManager.SetPlayerGameData(playerGameData);
        //Debug.Log("Saving data " + PrefsManager.GetPlayerData().userToken);
    }

    public bool IsLogedIn()
    {
        if (playerGameData.userId.Length > 0)
        {
            //UnityEngine.Debug.Log("playerGameData.userId :" + playerGameData.userId);
            return true;
        }

        return false;
    }

}

[System.Serializable]
public class P_PlayerGameDetails
{
    public string userId;
    public string userName;
    public string userMobile;
    public string userEmail;
    public string password;
    public string avatarURL, FrameUrl, CountryURL;
    public string referralCode;
    public string userLevel;
    public string countryName;
    public string countryCode;
    public float coins, diamonds, points;
    public int rabit, emoji, time;
    public bool isSilverCardPurchased;
    public bool isBronzeCardPurchased;
    public bool isPlatinumCardPurchased;
    public string registrationType;
    public string userToken;
    public string cashAmount;
}

