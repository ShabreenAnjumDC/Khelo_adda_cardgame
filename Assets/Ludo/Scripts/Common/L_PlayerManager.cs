using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_PlayerManager : MonoBehaviour
{
    public static L_PlayerManager instance;

    [SerializeField]
    private L_PlayerGameDetails playerGameData = null; // contains user details


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerGameData = L_PrefsManager.GetPlayerData();
    }

    public void DeletePlayerGameData()
    {
        playerGameData = null;
        L_PrefsManager.DeletePlayerData();
    }

    public L_PlayerGameDetails GetPlayerGameData()
    {
        return playerGameData;
    }

    public void SetPlayerGameData(L_PlayerGameDetails dataToAssign)
    {
        playerGameData = dataToAssign;
        L_PrefsManager.SetPlayerGameData(playerGameData);
    }

    public bool IsLogedIn()
    {
        //if (playerGameData != null && playerGameData.userId.Length > 0)
        if (playerGameData != null && (playerGameData.mobileNo !=null && playerGameData.mobileNo.Length > 0) && (playerGameData.emailId != null && playerGameData.emailId.Length > 0))
        {
            return true;
        }

        return false;
    }
}

[System.Serializable]
public class L_PlayerGameDetails
{
    public string userId;
    public string userName;
    public string password;
    public string name;
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
    public string gender;
    public string dob;
    public string panNo;
    public string address;
    public string mobileNo;
    public string emailId;
    public string token;
}
