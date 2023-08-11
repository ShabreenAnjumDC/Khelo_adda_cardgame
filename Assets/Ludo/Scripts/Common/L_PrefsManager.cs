using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class L_PrefsManager : MonoBehaviour
{
    public static void SetData(L_PrefsKey prefsKey, string dataToAssign)
    {
        PlayerPrefs.SetString("" + prefsKey, dataToAssign);
    }

    public static void SetPlayerGameData(L_PlayerGameDetails dataToAssign)
    {
        PlayerPrefs.SetString("" + L_PrefsKey.PlayerGameData, JsonUtility.ToJson(dataToAssign));
    }

    public static void DeletePlayerData()
    {
        PlayerPrefs.DeleteKey("" + L_PrefsKey.PlayerGameData);
    }

    public static L_PlayerGameDetails GetPlayerData()
    {
        L_PlayerGameDetails playerData = new L_PlayerGameDetails();

        string prefsData = PlayerPrefs.GetString("" + L_PrefsKey.PlayerGameData, "");

        if (!string.IsNullOrEmpty(prefsData))
        {
            JsonData data = JsonMapper.ToObject(prefsData);

            playerData.userId = data["userId"].ToString();
            //playerData.password = data["password"].ToString();
            if (data["userName"] != null)
                playerData.userName = data["userName"].ToString();
            playerData.mobileNo = data["mobileNo"].ToString();
            playerData.emailId = data["emailId"].ToString();
            playerData.name = data["name"].ToString();
            playerData.token = data["token"].ToString();
            playerData.avatarURL = data["avatarURL"].ToString();

            playerData.registrationType = data["registrationType"].ToString();

            //if (data.Keys.Contains("coins") && data["coins"].ToString() != null && data["coins"].ToString().Length > 0)
            //{
            //    float.TryParse(data["coins"].ToString(), out playerData.coins);
            //    // playerData.coins = float.Parse(data["coins"].ToString());
            //}
            //else
            //{
            //    playerData.coins = 0f;
            //}


            //if (data.Keys.Contains("diamonds") && data["diamonds"].ToString() != null && data["diamonds"].ToString().Length > 0)
            //{
            //    playerData.diamonds = float.Parse(data["diamonds"].ToString());
            //}
            //else
            //{
            //    playerData.diamonds = 0;
            //}

            //if (data.Keys.Contains("points") && data["points"].ToString() != null && data["points"].ToString().Length > 0)
            //{
            //    playerData.points = float.Parse(data["points"].ToString());
            //}
            //else
            //{
            //    playerData.points = 0;
            //}

            //if (data.Keys.Contains("emoji") && data["emoji"].ToString() != null && data["emoji"].ToString().Length > 0)
            //{
            //    playerData.emoji = int.Parse(data["emoji"].ToString());
            //}
            //else
            //{
            //    playerData.emoji = 0;
            //}

            //if (data.Keys.Contains("time") && data["time"].ToString() != null && data["time"].ToString().Length > 0)
            //{
            //    playerData.time = int.Parse(data["time"].ToString());
            //}
            //else
            //{
            //    playerData.time = 0;
            //}

        }
        else
        {
            playerData.userName = playerData.password = playerData.userId = "";

            //playerData.coins = playerData.diamonds = playerData.points = 0f;
            //playerData.rabit = playerData.emoji = playerData.time = 0;
        }

        return playerData;
    }
}

public enum L_PrefsKey
{
    PlayerGameData,
    RoomData
}
