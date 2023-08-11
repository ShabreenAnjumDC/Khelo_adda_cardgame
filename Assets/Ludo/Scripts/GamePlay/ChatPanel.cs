using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatPanel : MonoBehaviour
{
    public static ChatPanel instance;

    public TMP_InputField chatInputTMP;
    public Transform chatContent;
    public GameObject chatObject;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //for (int i = 0; i < chatContent.childCount; i++)
        //{
        //    Destroy(chatContent.GetChild(i).gameObject);
        //}
    }

    void Update()
    {

    }

    public void OnSendChatClick()
    {
        GameObject chatObj = Instantiate(chatObject, chatContent);
        chatObj.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = chatInputTMP.text;
        chatObj.transform.GetChild(1).gameObject.SetActive(true);
        chatInputTMP.text = "";
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<VerticalLayoutGroup>().GetComponent<RectTransform>());
        //Canvas.ForceUpdateCanvases();
    }

   
}
