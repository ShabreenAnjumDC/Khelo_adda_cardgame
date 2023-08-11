using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_TopUp : MonoBehaviour
{
    public static P_TopUp instance;

    public Slider slider;
    public Text sliderMinText;
    public Text sliderMaxText;
    public Text sliderSelectedText;
    public Text title;
    public Text balance;
    public Text errorText;

    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        title.text = P_SocketController.instance.tableData["game_json_data"]["small_blind"].ToString() + "/" + P_SocketController.instance.tableData["game_json_data"]["big_blind"].ToString() + " (" + P_InGameUiManager.instance.tableInfoText.text + ")";
        balance.text = "0";
        slider.minValue = float.Parse(P_SocketController.instance.tableData["game_json_data"]["minimum_buyin"].ToString());
        slider.maxValue = float.Parse(P_SocketController.instance.tableData["game_json_data"]["maximum_buyin"].ToString());
        sliderMinText.text = slider.minValue.ToString();
        sliderMaxText.text = slider.maxValue.ToString();
    }

    public void OnClickOnButton(string eventName)
    {
        switch (eventName)
        {
            case "close":
                P_InGameUiManager.instance.DestroyScreen(P_InGameScreens.TopUp);
                break;

            case "confirm":
                break;
        }
    }

    public void OnBuyInSliderValueChanged()
    {
        balance.text = ((int)slider.value).ToString();
    }
}