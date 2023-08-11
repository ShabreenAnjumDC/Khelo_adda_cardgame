using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassicLudoGamePlay : MonoBehaviour
{
    public static ClassicLudoGamePlay instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    public void OnClickOnButton(string buttonName)
    {
        switch (buttonName)
        {
            
        }
    }
}
