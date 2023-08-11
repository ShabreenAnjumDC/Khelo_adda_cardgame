using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_SwitchTable : MonoBehaviour
{
    public List<GameObject> PreviewImages = new List<GameObject>();
    public List<GameObject> TableImages = new List<GameObject>();
    public Button PreviousBtn, NextBtn;

    private int counter = 0;
    //public TMPro.TextMeshProUGUI CountText;

    public TMPro.TextMeshProUGUI ButtonText;
    public GameObject InUse, Use;
    public Button ConfirmButton;

    private void Start()
    {
        //P_SocketController.instance.currentScreenName = "SwitchTable";
        counter = PlayerPrefs.GetInt("TableCount");

        UpdateButtonState();

        PreviousBtn.onClick.RemoveAllListeners();
        NextBtn.onClick.RemoveAllListeners();

        PreviousBtn.onClick.AddListener(Decrement);
        NextBtn.onClick.AddListener(Increment);
        //ConfirmButton.onClick.AddListener(SwitchConfirm);
    }

    private void Increment()
    {
        counter++;
        if (counter > PreviewImages.Count - 1)
        {
            counter = 0;
        }
        UpdateButtonState();
    }

    private void Decrement()
    {
        counter--;
        if (counter < 0)
        {
            counter = PreviewImages.Count - 1;
        }
        UpdateButtonState();
    }

    public void TableIconClicked(int index)
    {
        counter = index;
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        for (int i = 0; i < PreviewImages.Count; i++)
        {
            PreviewImages[i].SetActive(false);
            TableImages[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        PreviewImages[counter].SetActive(true);
        TableImages[counter].transform.GetChild(0).gameObject.SetActive(true);
    }

    public void SwitchConfirm()
    {
        SwitchTables(counter);
        transform.parent.parent.GetComponent<P_InGameUiManager>().SwitchTables(counter);
        ClosePanel();
    }

    public void ClosePanel()
    {
        PlayerPrefs.SetInt("TableCount", counter);
        transform.parent.parent.GetComponent<P_InGameUiManager>().DestroyScreen(P_InGameScreens.SwitchTable);
    }

    private void SwitchTables(int counter)
    {
        PlayerPrefs.SetInt("TableCount", counter);

        ButtonText.text = "In Use";
        InUse.SetActive(true);
        Use.SetActive(false);
        ConfirmButton.enabled = false;

        for (int i = 0; i < GlobalGameManager.instance.table.Count; i++)
        {
            for (int j = 0; j < GlobalGameManager.instance.table.Count; j++)
            {
                GlobalGameManager.instance.table[i].transform.GetChild(0).GetComponent<P_InGameUiManager>().TableImages[j].SetActive(false);
            }
            GlobalGameManager.instance.table[i].transform.GetChild(0).GetComponent<P_InGameUiManager>().TableImages[counter].SetActive(true);
        }

        foreach (GameObject g in TableImages)
        {
            g.SetActive(false);
        }
        TableImages[counter].SetActive(true);

    }

    public void TableSelected(int index)
    {
        counter = index;
        UpdateButtonState();
    }

    public void ResetButtonClick()
    {
        counter = PlayerPrefs.GetInt("TableCount");
        UpdateButtonState();
    }

    public void BackButtonClick()
    {
        //if (P_SocketController.instance != null)
        //    P_SocketController.instance.currentScreenName = "Game";

        transform.parent.parent.GetComponent<P_InGameUiManager>().DestroyScreen(P_InGameScreens.SwitchTable);
    }
}