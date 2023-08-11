using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class P_ChatUiManager : MonoBehaviour
{
    public static P_ChatUiManager instance;
    public InputField inputFild;
    public GameObject inComingPrefab, outGoingPrefab, suggestionScreen;
    public Transform container;

    bool isSuggestionShow = false;
    public GameObject suggestionBtnArrow;
    public GameObject chatIcon;
    P_InGameUiManager inGameUiManager;

    private void Awake()
    {
        //GlobalGameManager.currentScreenName = "Chat";
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void Start()
    {
        //UpdateChatList();
        //SuggestionArrowManager();
        //inGameUiManager = transform.parent.parent.GetComponent<P_InGameUiManager>();
        OnClickOnButton("getchat");
    }


    void SuggestionArrowManager()
    {
        if (isSuggestionShow)
        {
            suggestionScreen.SetActive(true);
            suggestionBtnArrow.transform.localScale = new Vector3(suggestionBtnArrow.transform.localScale.x, suggestionBtnArrow.transform.localScale.y * (-1), suggestionBtnArrow.transform.localScale.z);
        }
        else
        {
            {
                suggestionScreen.SetActive(false);
                if (suggestionBtnArrow.transform.localScale.y < 0)
                {
                    suggestionBtnArrow.transform.localScale = new Vector3(suggestionBtnArrow.transform.localScale.x, suggestionBtnArrow.transform.localScale.y * (-1), suggestionBtnArrow.transform.localScale.z);

                }
                else
                {
                    suggestionBtnArrow.transform.localScale = new Vector3(suggestionBtnArrow.transform.localScale.x, suggestionBtnArrow.transform.localScale.y, suggestionBtnArrow.transform.localScale.z);
                }
            }
        }
    }


    public void UpdateChatList()
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        List<P_ChatMessage> chatList = P_ChatManager.instance.GetChatList();

        Debug.Log("Chat List Count: " + chatList.Count);

        if (chatList.Count > 0)
            chatIcon.SetActive(false);
        else
            chatIcon.SetActive(true);

        for (int i = 0; i < chatList.Count; i++)
        {
            GameObject gm = null;

            if (chatList[i].isMe)
            {
                gm = Instantiate(outGoingPrefab, container) as GameObject;
                gm.GetComponent<P_InOutMsgUIManager>().userId = PlayerManager.instance.GetPlayerGameData().userId;
                gm.GetComponent<P_InOutMsgUIManager>().userName.text = PlayerManager.instance.GetPlayerGameData().userName;
                gm.GetComponent<P_InOutMsgUIManager>().userNameFirstLetter.text = PlayerManager.instance.GetPlayerGameData().userName.Substring(0, 1);
            }
            else
            {
                gm = Instantiate(inComingPrefab, container) as GameObject;
                gm.GetComponent<P_InOutMsgUIManager>().userId = chatList[i].userId;
                gm.GetComponent<P_InOutMsgUIManager>().userName.text = chatList[i].title;
                gm.GetComponent<P_InOutMsgUIManager>().userNameFirstLetter.text = chatList[i].title.Substring(0, 1);
            }

            gm.GetComponent<P_InOutMsgUIManager>().SetText(chatList[i].desc);
            LayoutRebuilder.ForceRebuildLayoutImmediate(gm.GetComponent<VerticalLayoutGroup>().GetComponent<RectTransform>());
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<VerticalLayoutGroup>().GetComponent<RectTransform>());

    }


    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    if (suggestionScreen.activeInHierarchy)
                    {
                        suggestionScreen.SetActive(false);
                    }
                    //GlobalGameManager.currentScreenName = "Game";
                    //inGameUiManager.DestroyScreen(P_InGameScreens.Chat);
                    P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Chat);
                }
                break;

            case "showSuggestion":
                {
                    if (!isSuggestionShow)
                    {
                        isSuggestionShow = true;
                    }
                    else
                    {
                        isSuggestionShow = false;
                    }
                    SuggestionArrowManager();
                }
                break;

            case "send":
                {
                    if (inputFild.text.Length > 0)
                    {
                        P_ChatManager.instance.SendChatMessage(inputFild.text);

                        if (P_InGameUiManager.instance != null)
                            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Chat);
                        //else if (TournamentInGameUiManager.instance != null)
                        //TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.Chat);
                    }
                }
                break;

            case "getchat":
                {
                    P_SocketController.instance.GetChatMessage(P_SocketController.instance.TABLE_ID);
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in ChatuiManager = " + eventName);
                }
                break;
        }
    }

    public void OnClickOnSuggestions(Text suggestionText)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        P_ChatManager.instance.SendChatMessage(suggestionText.text);

        if (P_InGameUiManager.instance != null)
            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Chat);
        //else if (TournamentInGameUiManager.instance != null)
        //TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.Chat);
    }
}