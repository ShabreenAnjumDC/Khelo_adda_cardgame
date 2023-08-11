using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayInstruction : MonoBehaviour
{
    [Header("-----Parents-----")]
    public GameObject classicLudoParent;
    public GameObject quickLudoParent;
    public GameObject tipsNTricksParent;

    [Header("-----Option Buttons-----")]
    public Button classicLudoBtn;
    public Button quickLudoBtn;
    public Button tipsNTricksLudoBtn;

    [Header("-----Option On Off Sprites & Color-----")]
    public Sprite optionOnSprite;
    public Sprite optionOffSprite;
    public Color optionOnTextColor;
    public Color optionOffTextColor;

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "classic_ludo":
                ShowClassicPanel();
                break;

            case "quick_ludo":
                ShowQuickPanel();
                break;

            case "tips_n_tricks":
                ShowTipsNTricksPanel();
                break;

            case "classic_prev_btn":
                // if arrow is for slider
                break;

            case "classic_next_btn":
                ShowQuickPanel();
                break;

            case "quick_prev_btn":
                ShowClassicPanel();
                break;

            case "quick_next_btn":
                ShowTipsNTricksPanel();
                break;

            case "tips_prev_btn":
                ShowQuickPanel();
                break;

            case "tips_next_btn":
                // if arrow is for slider
                break;

            case "close":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                break;
        }
    }

    void ShowClassicPanel()
    {
        classicLudoParent.SetActive(true);
        quickLudoParent.SetActive(false);
        tipsNTricksParent.SetActive(false);

        classicLudoBtn.transform.GetChild(0).GetComponent<Text>().color = optionOnTextColor;
        quickLudoBtn.transform.GetChild(0).GetComponent<Text>().color = optionOffTextColor;
        tipsNTricksLudoBtn.transform.GetChild(0).GetComponent<Text>().color = optionOffTextColor;

        classicLudoBtn.GetComponent<Image>().sprite = optionOnSprite;
        quickLudoBtn.GetComponent<Image>().sprite = optionOffSprite;
        tipsNTricksLudoBtn.GetComponent<Image>().sprite = optionOffSprite;
    }

    void ShowQuickPanel()
    {
        quickLudoParent.SetActive(true);
        classicLudoParent.SetActive(false);
        tipsNTricksParent.SetActive(false);

        quickLudoBtn.transform.GetChild(0).GetComponent<Text>().color = optionOnTextColor;
        classicLudoBtn.transform.GetChild(0).GetComponent<Text>().color = optionOffTextColor;
        tipsNTricksLudoBtn.transform.GetChild(0).GetComponent<Text>().color = optionOffTextColor;

        quickLudoBtn.GetComponent<Image>().sprite = optionOnSprite;
        classicLudoBtn.GetComponent<Image>().sprite = optionOffSprite;
        tipsNTricksLudoBtn.GetComponent<Image>().sprite = optionOffSprite;
    }

    void ShowTipsNTricksPanel()
    {
        tipsNTricksParent.SetActive(true);
        classicLudoParent.SetActive(false);
        quickLudoParent.SetActive(false);

        tipsNTricksLudoBtn.transform.GetChild(0).GetComponent<Text>().color = optionOnTextColor;
        quickLudoBtn.transform.GetChild(0).GetComponent<Text>().color = optionOffTextColor;
        classicLudoBtn.transform.GetChild(0).GetComponent<Text>().color = optionOffTextColor;

        tipsNTricksLudoBtn.GetComponent<Image>().sprite = optionOnSprite;
        quickLudoBtn.GetComponent<Image>().sprite = optionOffSprite;
        classicLudoBtn.GetComponent<Image>().sprite = optionOffSprite;
    }
}
