using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoHomes : MonoBehaviour
{
    public static LudoHomes instance;

    public DiceManager rollingDice;
    public Players[] players;

    private void Start()
    {
        instance = this;
    }
}
