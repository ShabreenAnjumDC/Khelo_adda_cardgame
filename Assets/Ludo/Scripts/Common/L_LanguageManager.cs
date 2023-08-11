using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_LanguageManager : MonoBehaviour
{
    public static L_LanguageManager instance;

    public int languageId;
    // languageId = 0 for english

    void Awake()
    {
        instance = this;
    }
}
