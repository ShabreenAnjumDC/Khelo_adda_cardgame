using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;

public class P_BuyinPopup : MonoBehaviour
{
    public static P_BuyinPopup instance;

    public Slider buyInSlider;
    public Text buyInSliderMinText; //minText
    public Text buyInSliderMaxText; //maxText
    public Text buyInSliderSelectedText; //sliderAmountText
    public Text buyInSelectedTitle; //blinds
    public Text buyInTitle;
    public Text buyInBalance; //balanceText
    public Text buyInErrorText;
    public Button buyInButton; //TopUpButton
    public Button buyInCloseButton;

    public P_InGameUiManager p_InGameUiManager;

    [SerializeField]
    Text timerText;
    [SerializeField]
    Image timerImage;

    float initialBalance = 0;
    float timer, displayValue, increaseTimer = 0;


    void Awake()
    {
        instance = this;
    }


    void OnEnable()
    {
        timer = 0;
        increaseTimer = 0;
        displayValue = 0;
    }


    void FixedUpdate()
    {
        if (p_InGameUiManager.isTopUp == false && p_InGameUiManager.isCallFromMenu == false)
        {
            if (timer > 1)
            {
                timer -= Time.deltaTime;
                increaseTimer += Time.deltaTime;
                timerText.text = string.Format("{0:00}:{1:00}", 0, timer);
                timerImage.fillAmount = increaseTimer / displayValue;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }


    public void ShowBuyInPopup(bool isShow) //, float userInitialBalance = 0f)
    {
        if (isShow)
        {
            if (P_GameConstant.enableLog)
                Debug.Log("tableData: " + JsonMapper.ToJson(P_SocketController.instance.tableData));

            gameObject.SetActive(true);
            gameObject.GetComponent<RectTransform>().DOLocalMove(new Vector3(0, 0, 0), 0.3f);

            //.OnComplete( () => {
            //    buyInPopUp.GetComponent<RectTransform>().DOLocalMove(new Vector3(0, -1170, 0), 1f);
            //    buyInPopUp.SetActive(false);
            //});

            //buyInTitle.text = P_InGameUiManager.instance.tableInfoText.text;
            //buyInTitle.text = P_SocketController.instance.smallBlindTableData + "/" + P_SocketController.instance.bigBlindTableData + " (" + P_InGameUiManager.instance.tableInfoText.text + ")";
            buyInTitle.text = P_SocketController.instance.tableData["game_json_data"]["small_blind"].ToString() + "/" + P_SocketController.instance.tableData["game_json_data"]["big_blind"].ToString()
                            + " (" + P_InGameUiManager.instance.tableInfoText.text + ")";

            //buyInSlider.minValue = float.Parse(P_SocketController.instance.bigBlindTableData);
            //buyInSlider.maxValue = buyInSlider.minValue * (float.Parse(P_SocketController.instance.minimumBuyinTableData));
            buyInSlider.minValue = float.Parse(P_SocketController.instance.tableData["game_json_data"]["minimum_buyin"].ToString());  //bigBlind
            buyInSlider.maxValue = float.Parse(P_SocketController.instance.tableData["game_json_data"]["maximum_buyin"].ToString());  //maxBuyIn // parameter not found

            buyInSliderMinText.text = buyInSlider.minValue.ToString(); //bigBlind
            buyInSliderMaxText.text = buyInSlider.maxValue.ToString();

            buyInSlider.value = buyInSlider.maxValue;
            buyInBalance.text = buyInSlider.maxValue.ToString();

            if (p_InGameUiManager.isTopUp || p_InGameUiManager.isCallFromMenu)
            {
                buyInSelectedTitle.text = "Top-up";
                buyInButton.transform.GetChild(0).GetComponent<Text>().text = "TOP-UP";
            }
            else
            {
                buyInSelectedTitle.text = "Buy-in";
                buyInButton.transform.GetChild(0).GetComponent<Text>().text = "BUY-IN";
            }

            P_InGameManager.instance.realTimeResultBtn.interactable = false;
            P_InGameManager.instance.walletBtn.interactable = false;

            if (p_InGameUiManager.isTopUp == false && p_InGameUiManager.isCallFromMenu == false)
            {
                increaseTimer = 0;
                displayValue = timer = 20;
                timerText.gameObject.SetActive(true);
                timerImage.gameObject.SetActive(true);
            }
            else
            {
                timerText.gameObject.SetActive(false);
                timerImage.gameObject.SetActive(false);
            }
            //initialBalance = userInitialBalance;
            //balanceText.text = "Balance : " + Utility.GetTrimmedAmount("" + PlayerManager.instance.GetPlayerGameData().cashAmount.ToString());
            //string str = "";
            //switch (GlobalGameManager.instance.GetRoomData().gameMode)
            //{
            //    case GameMode.NLH:
            //        str = "(HOLD’EM)";
            //        break;
            //}
            //blinds.text = GlobalGameManager.instance.GetRoomData().smallBlind + "/" + GlobalGameManager.instance.GetRoomData().bigBlind + " " + str;
        }
        else
        {

        }
    }


    public void OnBuyInSliderValueChanged()
    {
        buyInSliderSelectedText.text = ((int)buyInSlider.value).ToString();
    }


    public void OnClickOnBuyInPopUp(string buttonName)
    {
        switch (buttonName)
        {
            case "CloseBuyIn":
                gameObject.SetActive(false);
                p_InGameUiManager.isCallFromMenu = false;
                //if (isTopUp)
                //{
                //    bool myPlayerFind = false;
                //    for (int i = 0; i < P_InGameManager.instance.playersScript.Length; i++)
                //    {
                //        if (P_InGameManager.instance.playersScript[i].GetPlayerData().userId == PlayerManager.instance.GetPlayerGameData().userId)
                //        {
                //            myPlayerFind = true;
                //        }
                //    }
                //    if (myPlayerFind == false)
                //    {
                //        P_SocketController.instance.SendJoinViewer();
                //        AllPlayerPosPlusOn();
                //    }
                //}
                break;

            case "SendTopUp":
                if (p_InGameUiManager.isTopUp || p_InGameUiManager.isCallFromMenu)
                {
                    // top up code
                    float selectedText = 0;

                    if (float.TryParse(buyInSliderSelectedText.text, out selectedText))
                    {
                        if (selectedText > 0)
                        {
                            P_SocketController.instance.SendTopUp(selectedText);
                            //P_SocketController.instance.isTopUpSended = true;
                        }
                    }
                }
                else
                {
                    P_SocketController.instance.SendJoin(P_SocketController.instance.TABLE_ID, buyInSliderSelectedText.text);
                    //buyInButton.interactable = false;
                    //buyInCloseButton.interactable = false;
                }
                break;
        }
    }
}
