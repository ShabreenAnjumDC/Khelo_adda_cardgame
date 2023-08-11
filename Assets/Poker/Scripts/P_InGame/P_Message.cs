using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Message : MonoBehaviour
{
    public static P_Message instance;

    public GameObject yesButton, noButton, okButton, headingCloseButton;

    public Text okButtonText, yesButtonText, noButtonText, messageText, headingText;
    private Action defaultCallBackMethod;
    private Action okButtonCallBackMethod, noButtonCallBackMethod, yesButtonCallBackMethod;

    // private void OnEnable()
    // {
    //     Debug.Log("Messege");
    // }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        okButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
        noButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
        yesButton.GetComponent<Button>().onClick.AddListener(ClosePopup);

        okButton.GetComponent<Button>().onClick.AddListener(OnClickOnOk);
        noButton.GetComponent<Button>().onClick.AddListener(OnClickOnNo);
        yesButton.GetComponent<Button>().onClick.AddListener(OnClickOnYes);
    }

    private void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public void HideHeadingCloseButton()
    {
        headingCloseButton.SetActive(false);
    }

    public void ShowSingleButtonPopUp(string messageToShow,Action callBack = null,string buttonText = "Ok")
    {
        //Debug.Log("Show meesage :" + messageToShow);

        if (messageToShow.Equals("No record found"))
        {
            messageText.text = "";
            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Message);
        }
        else {
            messageText.text = messageToShow;
        }
       
        okButtonText.text = buttonText;
        okButtonCallBackMethod = callBack;

        defaultCallBackMethod = OnClickOnOk;


        yesButton.SetActive(false);
        noButton.SetActive(false);
        okButton.SetActive(true);
    }


    public void ShowDoubleButtonPopUp(string messageToShow,Action yesButtonCallBack = null,Action noButtonCallBack = null,string yesText = "Yes",string noText = "No")
    {
        messageText.text = messageToShow;
        yesButtonText.text = yesText;
        noButtonText.text = noText;

        yesButtonCallBackMethod = yesButtonCallBack;
        noButtonCallBackMethod = noButtonCallBack;
        defaultCallBackMethod = OnClickOnNo;



        yesButton.SetActive(true);
        noButton.SetActive(true);

        okButton.SetActive(false);
    }



    public void OnClickOnOk()
    {
        //SoundManager.instance.PlaySound(SoundType.Click);
        okButtonCallBackMethod?.Invoke();


        if (P_InGameUiManager.instance != null)
        {
            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Message);
        }
        else
        {
            //InGameUiManager.instance.DestroyScreen(InGameScreens.Message);
        }
    }

    public void OnClickOnNo()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        noButtonCallBackMethod?.Invoke();
        if (P_InGameUiManager.instance != null)
        {
            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Message);
        }
        else
        {
            //InGameUiManager.instance.DestroyScreen(InGameScreens.Message);
        }
    }

    public void OnClickOnYes()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        yesButtonCallBackMethod?.Invoke();
        if (P_InGameUiManager.instance != null)
        {
            P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.Message);
        }
        //else if(InGameManager.instance != null)
        //{
        //    InGameUiManager.instance.DestroyScreen(InGameScreens.Message);
        //}
    }

    public void ExecuteDefaultMethod()
    {
        defaultCallBackMethod?.Invoke();
    }
}
