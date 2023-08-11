using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class P_ChatManager : MonoBehaviour
{

    public static P_ChatManager instance;
    private List<P_ChatMessage> chatList = new List<P_ChatMessage>();


    private void Awake()
    {
        instance = this;
    }

    public void OnChatMessageReceived(string serverResponse)
    {
        JsonData jsonData = JsonMapper.ToObject(serverResponse);

        /*if (jsonData[0]["from"].ToString() != PlayerManager.instance.GetPlayerGameData().userId)
        {
            P_ChatMessage data = new P_ChatMessage();
            data.desc = jsonData[0]["desc"].ToString();
            data.title = jsonData[0]["title"].ToString();
            data.userId = jsonData[0]["userId"].ToString();
            data.isMe = false;
            chatList.Add(data);
        }*/

        for (int i = 0; i < jsonData.Count; i++)
        {
            if (jsonData[i]["userId"].ToString() != PlayerManager.instance.GetPlayerGameData().userId)
            {
                P_ChatMessage data = new P_ChatMessage();
                data.desc = jsonData[i]["message"].ToString();
                data.title = jsonData[i]["userName"].ToString();
                data.userId = jsonData[i]["userId"].ToString();
                data.isMe = false;
                chatList.Add(data);
            }
        }

        if (P_ChatUiManager.instance != null)
        {
            P_ChatUiManager.instance.UpdateChatList();
        }
    }

    public void SendChatMessage(string messageToSend)
    {
        P_ChatMessage chatMessage = new P_ChatMessage();
        chatMessage.desc = messageToSend;
        chatMessage.isMe = true;
        chatMessage.title = GetUserName();
        chatList.Add(chatMessage);

        if (P_InGameUiManager.instance != null)
            P_SocketController.instance.SendChatMessage(GetUserName(), messageToSend, P_SocketController.instance.TABLE_ID);
    }

    private string GetUserName()
    {
        string userName = PlayerManager.instance.GetPlayerGameData().userName;
        userName += "                                          " + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second;

        return userName;
    }

    public List<P_ChatMessage> GetChatList()
    {
        return chatList;
    }

}

public class P_ChatMessage
{
    public string userName;
    public bool isMe;
    public string desc, title;
    public string userId;
}