using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;

    public Sprite[] Dice;
    public Image rend;
    public int randomDiceSide;
    public int finalSide;
    public AudioSource AudioClip;

    Coroutine DiceRoll;

    void Start()
    {
        instance = this;
    }

    // when click on dice
    public void OnClickDice()
    {
        if (L_SocketController.instance.playerTurnIndex == L_SocketController.instance.myPlayerIndex)
        {
            if (L_SocketController.instance.waitingForMove == false)
            {
                L_SocketController.instance.RollDice();
                GameManager.instance.manageRollingDice[L_SocketController.instance.swapIndex].GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            if (L_SoundManager.instance.isSound)
                L_SoundManager.instance.PlaySound(L_SoundType.NotYourTurnSound, L_SocketController.instance.transform);
        }
    }

    // coroutine for roll dice animation
    public IEnumerator DiceTime(int diceValue, Action<int> action = null)
    {
        if (L_SoundManager.instance.isSound)
            AudioClip.Play();
        
        GameManager.instance.rollingDice = this;

        for (int i = 0; i < Dice.Length; i++)
        {
            rend.sprite = Dice[i];
            yield return new WaitForSeconds(0.1f);
            randomDiceSide = diceValue - 1; //Random.Range(0, maxNumOnDice);  //0 default  //for testing 4 thi 5 k 6 avse 
            rend.sprite = Dice[randomDiceSide];
        }

        finalSide = randomDiceSide;

        if (DiceRoll != null)
            StopCoroutine(DiceRoll);

        if (action != null)
            action(diceValue);
    }
}