using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_HandSummaryItemControl : MonoBehaviour
{
    public static P_HandSummaryItemControl instance;

    public Text ActionOneText;
    public Text PlayerNameText;
    public Text ActionTwoText;
    public Text BetText;
    public GameObject FoldBG;

    public Image[] userCards;
    public Image p_I1, p_I2, p_I3, p_I4; //normal game

    public Image p_image0, p_image1, p_image2, p_image3, p_image4;
    public Image c_image0, c_image1, c_image2, c_image3, c_image4;

    public Color GreenColor, RedColor;
    public string GreenColorString = "#33daaf";
    public string RedColorString = "#d45055";

    private HandSummary handSummary;

    private void Awake()
    {
        instance = this;
    }

    public void Init(HandSummary _handSummary)
    {
        //ColorUtility.TryParseHtmlString(GreenColorString, out GreenColor);
        //ColorUtility.TryParseHtmlString(RedColorString, out RedColor);

        //handSummary = _handSummary;

        //ActionOneText.text = "" + handSummary.seatName;

        //PlayerNameText.text = handSummary.userName;
        //ActionTwoText.text = handSummary.handStrength;
        //Debug.Log(handSummary.userName + " HandSummary " + handSummary.winAmount + ", " + handSummary.cards.Count + ", " + handSummary.communityCard.Count);
        //if (handSummary.winAmount > 0)
        //{
        //    BetText.text = "+" + handSummary.winAmount.ToString();
        //    BetText.color = GreenColor;
        //}
        //else
        //{
        //    BetText.text = handSummary.winAmount.ToString();
        //    BetText.color = RedColor;
        //}

        ////user cards 
        //if (handSummary.handStrength != "Fold" && handSummary.cards.Count > 0)
        //{
        //    for (int i = 0; i < handSummary.cards.Count; i++)
        //    {
        //        userCards[i].SetActive(true);
        //        if (!string.IsNullOrEmpty(handSummary.cards[0].ToString()))
        //        {
        //            CardData pCard = CardsManager.instance.GetCardData(handSummary.cards[i].ToString());
        //            userCards[i].GetComponent<Image>().sprite = pCard.cardsSprite;
        //            if (handSummary.winningCards.Contains(handSummary.cards[i].ToString()))
        //                userCards[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        //        }
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < handSummary.cards.Count; i++)
        //    {
        //        userCards[i].SetActive(true);
        //    }
        //}

        ////community cards
        //if (handSummary.communityCard.Count > 0)
        //{
        //    if (!string.IsNullOrEmpty(handSummary.communityCard[0].ToString()))
        //    {
        //        CardData c1Card = CardsManager.instance.GetCardData(handSummary.communityCard[0].ToString());
        //        c_image0.sprite = c1Card.cardsSprite;
        //        if (handSummary.winningCards.Contains(handSummary.communityCard[0].ToString()))
        //            c_image0.color = new Color(1f, 1f, 1f, 1f);
        //    }
        //    if (!string.IsNullOrEmpty(handSummary.communityCard[1].ToString()))
        //    {
        //        CardData c2Card = CardsManager.instance.GetCardData(handSummary.communityCard[1].ToString());
        //        c_image1.sprite = c2Card.cardsSprite;
        //        if (handSummary.winningCards.Contains(handSummary.communityCard[1].ToString()))
        //            c_image1.color = new Color(1f, 1f, 1f, 1f);
        //    }
        //    if (!string.IsNullOrEmpty(handSummary.communityCard[2].ToString()))
        //    {
        //        CardData c3Card = CardsManager.instance.GetCardData(handSummary.communityCard[2].ToString());
        //        c_image2.sprite = c3Card.cardsSprite;
        //        if (handSummary.winningCards.Contains(handSummary.communityCard[2].ToString()))
        //            c_image2.color = new Color(1f, 1f, 1f, 1f);
        //    }
        //    if (!string.IsNullOrEmpty(handSummary.communityCard[3].ToString()))
        //    {
        //        CardData c4Card = CardsManager.instance.GetCardData(handSummary.communityCard[3].ToString());
        //        c_image3.sprite = c4Card.cardsSprite;
        //        if (handSummary.winningCards.Contains(handSummary.communityCard[3].ToString()))
        //            c_image3.color = new Color(1f, 1f, 1f, 1f);
        //    }
        //    if (!string.IsNullOrEmpty(handSummary.communityCard[4].ToString()))
        //    {
        //        CardData c5Card = CardsManager.instance.GetCardData(handSummary.communityCard[4].ToString());
        //        c_image4.sprite = c5Card.cardsSprite;
        //        if (handSummary.winningCards.Contains(handSummary.communityCard[4].ToString()))
        //            c_image4.color = new Color(1f, 1f, 1f, 1f);
        //    }
        //}


    }

}

