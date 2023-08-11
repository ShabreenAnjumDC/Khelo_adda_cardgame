using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_EmojiUIScreenManager : MonoBehaviour
{

    public static P_EmojiUIScreenManager instance;

    public GameObject[] containerAry;

    public int containerVal;

    public GameObject hideOnDealerClick;
    public GameObject showOnDealerClick;

    public Image profileimage, frame;
    public Text userName, UserId, levletxt;

    public GameObject addBtn;
    public GameObject dealerBtn;


    // Start is called before the first frame update
    void Awake()
    {
        //GlobalGameManager.currentScreenName = "TableUserProfile";
        instance = this;
    }



    private void Start()
    {
        //ClearAll();
        if (P_InGameUiManager.instance != null && P_InGameUiManager.instance.TempUserID != null && P_InGameUiManager.instance.TempUserID != "0")
        {
            GetUserDetails(P_InGameUiManager.instance.TempUserID);
        }
    }

    private void ClearAll()
    {
        levletxt.text = "Lvl. " + ">>";
        userName.text = "";
        UserId.text = "";
    }

    public void OnDealerBtnClick()
    {
        //if (P_InGameUiManager.instance != null)
        //    P_InGameUiManager.instance.ShowScreen(P_InGameScreens.DealerImageScreen);
    }

    public void GetUserDetails(string playerid)
    {
        //StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_BASE_URL + "user/" + playerid, UserDetailsResponse));
    }

    void UserDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => UserDetails: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (bool.Parse(data["status"].ToString()))
        {
            userName.text = data["data"]["userName"].ToString();
            UserId.text = "UserID:" + data["data"]["userId"].ToString();
        }
        else
        {
            Debug.LogError(data["error"].ToString());
        }
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log(serverResponse);
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                P_InGameUiManager.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.GetUserDetails)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                for (int i = 0; i < data["getData"].Count; i++)
                {

                    loadImages(data["getData"][i]["profileImage"].ToString(), data["getData"][i]["frameURL"].ToString());
                    levletxt.text = "Lvl. " + data["getData"][i]["userLevel"].ToString() + ">>";
                    userName.text = data["getData"][i]["userName"].ToString();
                    UserId.text = "UserID:" + data["getData"][i]["userId"].ToString();
                }

            }
            else
            {
                if (P_InGameUiManager.instance != null)
                    P_InGameUiManager.instance.ShowMessage(data["message"].ToString());
            }
        }
    }
    public void loadImages(string urlAvtar, string urlframe)
    {
        StartCoroutine(loadSpriteImageFromUrl(urlAvtar, profileimage));
        StartCoroutine(loadSpriteImageFromUrl(urlframe, frame));
    }
    IEnumerator loadSpriteImageFromUrl(string URL, Image image)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            yield return null;
        }
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed" + image.gameObject.name);
        }
        else
        {
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;
        }
    }

    void ShowContainer(int val)
    {
        for (int i = 0; i < containerAry.Length; i++)
        {
            if (i == P_InGameUiManager.instance.emojiContainerVal)
            {
                containerAry[i].SetActive(true);

                if (i == 2)
                {
                    addBtn.SetActive(false);
                    dealerBtn.SetActive(true);
                    hideOnDealerClick.SetActive(false);
                    showOnDealerClick.SetActive(true);
                }
                else
                {
                    if (i == 0)
                        addBtn.SetActive(false);
                    else
                        addBtn.SetActive(true);
                    dealerBtn.SetActive(false);
                    hideOnDealerClick.SetActive(true);
                    showOnDealerClick.SetActive(false);
                }
            }
            else
            {
                containerAry[i].SetActive(false);
            }
        }
    }

    public void SelectEmojiButton(string str)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        int emojiIndex = 0;
        Debug.Log("Here Get The emoji name which show ---  " + str);

        switch (str)
        {
            case "bluffing":
                emojiIndex = 0;
                break;
            case "youRaPro":
                emojiIndex = 1;
                break;
            case "beerCheers":
                emojiIndex = 2;
                break;
            case "murgi":
                emojiIndex = 3;
                break;
            case "rocket":
                emojiIndex = 4;
                break;
            case "dung":
                emojiIndex = 5;
                break;
            case "oscar":
                emojiIndex = 6;
                break;
            case "donkey":
                emojiIndex = 7;
                break;
            case "thumbUp":
                emojiIndex = 8;
                break;
            case "cherees":
                emojiIndex = 9;
                break;
            case "kiss":
                emojiIndex = 10;
                break;
            case "fish":
                emojiIndex = 11;
                break;
            case "gun":
                emojiIndex = 12;
                break;
            case "rose":
                emojiIndex = 13;
                break;
            case "perfume":
                emojiIndex = 14;
                break;
            case "ring":
                emojiIndex = 15;
                break;
            case "car":
                emojiIndex = 16;
                break;
        }

        if (P_InGameUiManager.instance != null)
            P_InGameUiManager.instance.CallEmojiSocket(emojiIndex);

        OnClickOnButton("back");
    }


    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {

                    StopCoroutine("loadSpriteImageFromUrl");
                    //GlobalGameManager.currentScreenName = "Game";
                    //transform.parent.parent.GetComponent<P_InGameUiManager>().DestroyScreen(P_InGameScreens.EmojiScreen);
                    P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.EmojiScreen);
                    //P_SocketController.instance.JoinSimilarButtonsEnable();
                    //P_SocketController.instance.joinSimilarTblBtnContainer.gameObject.SetActive(true);
                    //P_SocketController.instance.JoinSimilarButtonsInteractable(true);
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in MissionsUiManager = " + eventName);
                }
                break;
        }
    }
}

