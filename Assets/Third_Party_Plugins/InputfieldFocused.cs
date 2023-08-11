using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputfieldFocused : MonoBehaviour
{
    InputField inputField;

    void Start()
    {
        inputField = transform.GetComponent<InputField>();
        //#if UNITY_IOS
        inputField.shouldHideMobileInput = true;
        //#endif
    }
}