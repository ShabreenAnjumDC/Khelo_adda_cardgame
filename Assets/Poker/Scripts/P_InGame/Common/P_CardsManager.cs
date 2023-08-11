using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

public class P_CardsManager : MonoBehaviour
{
    public static P_CardsManager instance;
    [SerializeField]
    public Sprite[] cardSprites;
    public Sprite cardBackSprite;    //private

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //cardSprites = Resources.LoadAll<Sprite>("cards");
        //cardSprites = Resources.LoadAll<Sprite>("cardsOld");
        //cardBackSprite = cardSprites[52];
    }

    public Sprite GetCardBackSideSprite()
    {
        return cardBackSprite;
    }

    #region Card_Prioirt_Calculation
    public P_CardSequence GetCardSequence(List<P_CardData> availableCards)
    {
        availableCards = SortCard(availableCards);

        bool isAllInSequence = true, isAllIconSame = true;
        P_CardIcon firstCardIcon = availableCards[0].cardIcon;
        bool isAceAtFront = false;
        List<P_CardOccurrenceData> cardOccurrance = new List<P_CardOccurrenceData>();

        if (availableCards[0].cardNumber == P_CardNumber.ACE)
        {
            isAceAtFront = true;
        }

        for (int i = 0; i < availableCards.Count - 1; i++)
        {
            bool isMatchFound = false;
            for (int j = 0; j < cardOccurrance.Count; j++)
            {
                if (cardOccurrance[j].cardNumber == availableCards[i].cardNumber)
                {
                    ++cardOccurrance[j].occurranceCount;
                    isMatchFound = true;
                    j = 100;
                }
            }


            if (!isMatchFound)
            {
                P_CardOccurrenceData cardOccurrenceData = new P_CardOccurrenceData();
                cardOccurrenceData.cardNumber = availableCards[i].cardNumber;
                cardOccurrenceData.occurranceCount = 1;
                cardOccurrance.Add(cardOccurrenceData);
            }

            if (availableCards[i].cardNumber == P_CardNumber.ACE)
            {
                if (isAceAtFront)
                {
                    if (availableCards[i + 1].cardNumber != P_CardNumber.TWO)
                    {
                        isAllInSequence = false;
                    }
                }
                else // more than 1 Ace at the end
                {
                    isAllInSequence = false;
                }
            }
            else
            {
                if ((int)availableCards[i].cardNumber + 1 != (int)availableCards[i + 1].cardNumber)
                {
                    isAllInSequence = false;
                }
            }


            if (availableCards[i].cardIcon != firstCardIcon)
            {
                isAllIconSame = false;
            }
        }


        bool isMatchFound2 = false;
        for (int j = 0; j < cardOccurrance.Count; j++)
        {
            if (cardOccurrance[j].cardNumber == availableCards[availableCards.Count - 1].cardNumber)
            {
                ++cardOccurrance[j].occurranceCount;
                isMatchFound2 = true;
                break;
            }
        }


        if (!isMatchFound2)
        {
            P_CardOccurrenceData cardOccurrenceData = new P_CardOccurrenceData();
            cardOccurrenceData.cardNumber = availableCards[availableCards.Count - 1].cardNumber;
            cardOccurrenceData.occurranceCount = 1;
            cardOccurrance.Add(cardOccurrenceData);
        }

        if (availableCards[availableCards.Count - 1].cardIcon != firstCardIcon)
        {
            isAllIconSame = false;
        }


        if (isAllInSequence)
        {
            if (isAllIconSame)
            {
                if (IsContainsCard(availableCards, P_CardNumber.KING) && IsContainsCard(availableCards, P_CardNumber.ACE))
                {
                    return P_CardSequence.Royal_Flush; // All Card in Sequence and cardIcon is also same with highest Cards
                }
                else
                {
                    return P_CardSequence.Straight_Flush; // All Card in Sequence and cardIcon is also same without highest cards
                }
            }
        }


        int twoPairCount = 0;
        bool isThreePairAvailable = false;

        for (int i = 0; i < cardOccurrance.Count; i++)
        {
            if (cardOccurrance[i].occurranceCount >= 4)
            {
                return P_CardSequence.Four_of_a_Kind; // Four cards of same kind
            }

            if (cardOccurrance[i].occurranceCount >= 3)
            {
                isThreePairAvailable = true;
            }
            else if (cardOccurrance[i].occurranceCount >= 2)
            {
                ++twoPairCount;
            }
        }

        if (isThreePairAvailable && twoPairCount > 0)
        {
            return P_CardSequence.Full_House; // Three of a kind and one pair
        }

        if (isAllIconSame)
        {
            return P_CardSequence.Flush; // five cards of same Icon
        }

        if (isAllInSequence)
        {
            return P_CardSequence.Straight; // all cards in sequence but icon is not in sequence
        }

        if (isThreePairAvailable)
        {
            return P_CardSequence.Three_of_a_Kind; // Three of a kind without pair
        }

        if (twoPairCount > 1)
        {
            return P_CardSequence.Two_Pairs; // pair of two cards with same number 
        }

        if (twoPairCount > 0)
        {
            return P_CardSequence.Pair; // two cards with same number
        }

        return P_CardSequence.High_Card;
    }

    private List<P_CardData> SortCard(List<P_CardData> cards)
    {
        List<P_CardData> availableCards = new List<P_CardData>(cards);
        List<P_CardData> aceCards = new List<P_CardData>();

        for (int i = 0; i < availableCards.Count; i++)
        {
            if (availableCards[i].cardNumber == P_CardNumber.ACE)
            {
                aceCards.Add(availableCards[i]);
                availableCards.RemoveAt(i);
                --i;
            }
        }


        for (int counter = 0; counter < availableCards.Count; counter++)
        {
            for (int j = 0; j < availableCards.Count - 1; j++)
            {
                if (availableCards[j].cardNumber > availableCards[j + 1].cardNumber)
                {
                    P_CardData temp = availableCards[j];
                    availableCards[j] = availableCards[j + 1];
                    availableCards[j + 1] = temp;
                }
            }
        }


        if (IsContainsCard(availableCards, P_CardNumber.KING))
        {
            availableCards.AddRange(aceCards);
            return availableCards;
        }
        else
        {
            aceCards.AddRange(availableCards);
            return aceCards;
        }
    }


    private bool IsContainsCard(List<P_CardData> availableCards, P_CardNumber cardNumber)
    {
        for (int i = 0; i < availableCards.Count; i++)
        {
            if (availableCards[i].cardNumber == cardNumber)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    public P_CardData GetEmptyCardData()
    {
        P_CardData data = new P_CardData();
        data.cardNumber = P_CardNumber.NONE;
        data.cardIcon = P_CardIcon.NONE;
        data.cardsSprite = null;

        return data;
    }

    public P_CardData GetCardData(string serverGivenCardName)
    {
        P_CardData data = new P_CardData();

        switch (serverGivenCardName[0])
        {
            case 'T':
                data.cardNumber = P_CardNumber.TEN;
                break;

            case 'J':
                data.cardNumber = P_CardNumber.JACK;
                break;

            case 'Q':
                data.cardNumber = P_CardNumber.QUEEN;
                break;

            case 'K':
                data.cardNumber = P_CardNumber.KING;
                break;

            case 'A':
                data.cardNumber = P_CardNumber.ACE;
                break;

            default:
                int numberIndex = int.Parse(serverGivenCardName[0].ToString());
                data.cardNumber = (P_CardNumber)(numberIndex - 2);
                break;
        }


        switch (serverGivenCardName[1])
        {
            case 'c':
                data.cardIcon = P_CardIcon.CLUB;
                break;

            case 'd':
                data.cardIcon = P_CardIcon.DIAMOND;
                break;

            case 'h':
                data.cardIcon = P_CardIcon.HEART;
                break;

            case 's':
                data.cardIcon = P_CardIcon.SPADES;
                break;

            default:
                int numberIndex = int.Parse(serverGivenCardName[0].ToString());
                data.cardNumber = (P_CardNumber)(numberIndex - 2);
                break;
        }

        int totalCardNumbers = Enum.GetNames(typeof(P_CardNumber)).Length - 1;
        int totalCardIcons = Enum.GetNames(typeof(P_CardIcon)).Length - 1;

        int cardNumber = totalCardNumbers - (int)data.cardNumber; // reverse order
        int cardIcon = totalCardIcons - (int)data.cardIcon; // reverse order

        if (serverGivenCardName[0] == 'J' || serverGivenCardName[0] == 'K' || serverGivenCardName[0] == 'Q' ||
            serverGivenCardName[0] == 'A' || serverGivenCardName[0] == 'T')
        {
            data.cardsSprite = cardSprites[((cardIcon * 13) + cardNumber)];
        }
        else
        {
            int val = (((cardIcon * 13) + cardNumber) - 1);
            data.cardsSprite = cardSprites[val];
        }
        return data;
    }
}


[System.Serializable]
public class P_CardData
{
    public P_CardIcon cardIcon;
    public P_CardNumber cardNumber;
    public Sprite cardsSprite;
}

[System.Serializable]
public class P_CardOccurrenceData
{
    public int occurranceCount;
    public P_CardNumber cardNumber;
}

[System.Serializable]
public enum P_CardIcon
{
    NONE,
    CLUB,
    SPADES,
    DIAMOND,
    HEART
}


[System.Serializable]
public enum P_CardNumber
{
    NONE,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    SEVEN,
    EIGHT,
    NINE,
    TEN,
    JACK,
    QUEEN,
    KING,
    ACE
}


[System.Serializable]
public enum P_CardSequence
{
    NONE,
    Royal_Flush,
    Straight_Flush,
    Four_of_a_Kind,
    Full_House,
    Flush,
    Straight,
    Three_of_a_Kind,
    Two_Pairs,
    Pair,
    High_Card
}
