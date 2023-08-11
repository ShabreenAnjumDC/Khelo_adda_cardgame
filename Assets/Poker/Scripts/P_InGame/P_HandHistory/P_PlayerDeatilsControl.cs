using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class P_PlayerDeatilsControl : MonoBehaviour
{
    public static P_PlayerDeatilsControl instance;

    public TextMeshProUGUI ActionOneText;
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI ActionTwoText;
    public Image ActionTwoImage;
    public TextMeshProUGUI BetText;
    public TextMeshProUGUI TotalText;

    private Color SmallBigBlindColor, RaiseColor, FoldColor, AllInColor, CallColor, BetColor, CheckColor;

    private string SmallBigBlindColorString = "#2D67FD";
    private string RaiseColorString = "#E78224";
    private string FoldColorString = "#00023A";
    private string AllInColorString = "#7274B5";
    private string CallColorString = "#5AAA4D";
    private string BetColorString = "#E78224";
    private string CheckColorString = "#5AAA4D";


    private string greenColorString = "#1ADACE";
    private string redColorString = "#FF6969";

    private void Awake()
    {
        instance = this;
    }

    public void Init(string round, HandDetails handDetails, int place)
    {
        ColorUtility.TryParseHtmlString(SmallBigBlindColorString, out SmallBigBlindColor);
        ColorUtility.TryParseHtmlString(RaiseColorString, out RaiseColor);
        ColorUtility.TryParseHtmlString(FoldColorString, out FoldColor);
        ColorUtility.TryParseHtmlString(AllInColorString, out AllInColor);
        ColorUtility.TryParseHtmlString(CallColorString, out CallColor);
        ColorUtility.TryParseHtmlString(BetColorString, out BetColor);
        ColorUtility.TryParseHtmlString(CheckColorString, out CheckColor);

        switch (round)
        {
            case "preflop":
                PlayerNameText.text = handDetails.PREFLOP[place].userName;
                UpdateTextAndColor(handDetails.PREFLOP[place].betType, handDetails.PREFLOP[place].seatName);
                BetText.text = handDetails.PREFLOP[place].totalBet.ToString();
                TotalText.text = handDetails.PREFLOP[place].totalCoins.ToString();
                break;
            case "postflop":
                PlayerNameText.text = handDetails.POSTFLOP[place].userName;
                UpdateTextAndColor(handDetails.POSTFLOP[place].betType, handDetails.POSTFLOP[place].seatName);
                BetText.text = handDetails.POSTFLOP[place].totalBet.ToString();
                TotalText.text = handDetails.POSTFLOP[place].totalCoins.ToString();
                break;
            case "turn":
                Debug.Log(handDetails.POSTTURN.Count + " " + place);
                PlayerNameText.text = handDetails.POSTTURN[place].userName;
                UpdateTextAndColor(handDetails.POSTTURN[place].betType, handDetails.POSTTURN[place].seatName);
                BetText.text = handDetails.POSTTURN[place].totalBet.ToString();
                TotalText.text = handDetails.POSTTURN[place].totalCoins.ToString();
                break;
            case "river":
                PlayerNameText.text = handDetails.POSTRIVER[place].userName;
                UpdateTextAndColor(handDetails.POSTRIVER[place].betType, handDetails.POSTRIVER[place].seatName);
                BetText.text = handDetails.POSTRIVER[place].totalBet.ToString();
                TotalText.text = handDetails.POSTRIVER[place].totalCoins.ToString();
                break;
            case "showdown":
                PlayerNameText.text = handDetails.SHOWDOWN[place].userName;
                UpdateTextAndColor(handDetails.SHOWDOWN[place].betType, handDetails.SHOWDOWN[place].seatName);
                BetText.text = handDetails.SHOWDOWN[place].totalBet.ToString();
                TotalText.text = handDetails.SHOWDOWN[place].totalCoins.ToString();
                break;
            default:
                break;
        }
        if (PrefsManager.GetPlayerData().userName == PlayerNameText.text)
        {
            PlayerNameText.text = "<color=yellow>" + PlayerNameText.text + "</color>";
            BetText.text = "<color=yellow>" + BetText.text + "</color>";
            TotalText.text = "<color=yellow>" + TotalText.text + "</color>";
        }
    }

    void MakeMyRowColorDifferent(string uName)
    {
        if (PrefsManager.GetPlayerData().userName == uName)
        {
            PlayerNameText.text = "<color=yellow>" + PlayerNameText.text + "</color>";
            BetText.text = "<color=yellow>" + BetText.text + "</color>";
            TotalText.text = "<color=yellow>" + TotalText.text + "</color>";
        }
    }

    private void UpdateTextAndColor(string betType, string seatName)
    {
        Debug.Log("betType " + betType);
        switch (betType)
        {
            case "Call":
                ActionTwoText.text = "C";
                ActionTwoImage.color = CallColor;
                break;
            case "Check":
                ActionTwoText.text = "C";
                ActionTwoImage.color = CheckColor;
                break;
            case "Fold Card":
                ActionTwoText.text = "F";
                ActionTwoImage.color = FoldColor;
                break;
            case "Winner":
                ActionTwoText.text = "W";
                ActionTwoImage.color = CallColor;
                break;
            case "Raise":
                ActionTwoText.text = "R";
                ActionTwoImage.color = RaiseColor;
                break;
            case "All In":
                ActionTwoText.text = "A";
                ActionTwoImage.color = AllInColor;
                break;
            default:
                break;
        }
        if (betType == "Fold Card")
            ActionTwoText.text = betType.Split(' ')[0];
        else
            ActionTwoText.text = betType;
        ActionTwoText.color = Color.white;
        ActionOneText.text = seatName;
    }
}
