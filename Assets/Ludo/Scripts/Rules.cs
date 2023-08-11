using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rules : MonoBehaviour
{
    [Header("-----Parents-----")]
    public GameObject tournmentParent;
    public GameObject gameplayParent;
    public GameObject pointsParent;

    [Header("-----Option Toggle-----")]
    public Toggle tournmentOptionToggle;
    public Toggle gameplayOptionToggle;
    public Toggle pointsOptionToggle;

    [Header("-----Option Text-----")]
    public Text tournmentOptionText;
    public Text gameplayOptionText;
    public Text pointsOptionText;

    [Header("-----Option On Off Sprites & Color-----")]
    public Sprite optionOnSprite;
    public Sprite optionOffSprite;
    public Color optionOnTextColor;
    public Color optionOffTextColor;

    public void OnOptionToggleValueChanged(string toggleName)
    {
        switch(toggleName)
        {
            case "tournament":
                ShowTournamentPanel();
                break;

            case "gameplay":
                ShowGameplayPanel();
                break;

            case "points":
                ShowPointsPanel();
                break;
        }
    }

    public void OnClickCloseButton()
    {
        L_MainMenuController.instance.PlayButtonSound();

        L_MainMenuController.instance.DestroyScreen(MainMenuScreens.Rules);
    }

    void ShowTournamentPanel()
    {
        if (tournmentOptionToggle.isOn)
            L_MainMenuController.instance.PlayButtonSound();

        tournmentParent.SetActive(true);
        gameplayParent.SetActive(false);
        pointsParent.SetActive(false);

        tournmentOptionText.color = optionOnTextColor;
        gameplayOptionText.color = optionOffTextColor;
        pointsOptionText.color = optionOffTextColor;
    }

    void ShowGameplayPanel()
    {
        if (gameplayOptionToggle.isOn)
            L_MainMenuController.instance.PlayButtonSound();

        gameplayOptionText.color = optionOnTextColor;
        tournmentOptionText.color = optionOffTextColor;
        pointsOptionText.color = optionOffTextColor;

        gameplayParent.SetActive(true);
        tournmentParent.SetActive(false);
        pointsParent.SetActive(false);
    }

    void ShowPointsPanel()
    {
        if (pointsOptionToggle.isOn)
            L_MainMenuController.instance.PlayButtonSound();

        pointsOptionText.color = optionOnTextColor;
        gameplayOptionText.color = optionOffTextColor;
        tournmentOptionText.color = optionOffTextColor;

        pointsParent.SetActive(true);
        tournmentParent.SetActive(false);
        gameplayParent.SetActive(false);
    }
}
