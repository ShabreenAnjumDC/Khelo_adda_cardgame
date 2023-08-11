using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpDesk : MonoBehaviour
{
    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "close":
                //MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                L_MainMenuController.instance.DestroyScreen(MainMenuScreens.HelpDesk);
                break;

            case "chat":
                SendWhatsAppMsg();
                break;

            case "email":
                SendEmail();
                break;
        }
    }

    void SendEmail()
    {
        string email = "help-tujezeludo@gmail.com";
        string subject = MyEscapeURL("My Subject");
        string body = MyEscapeURL("Hi,\r\nPlease provide ...");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    void SendWhatsAppMsg()
    {
        string number = "+99000000001";
        string message = "Please help regarding ...";
        string url = "https://api.whatsapp.com/send?phone=" + number + "&text=" + message;
        //Number variable needs to include the country code. If you want to send a message to Guatemala
        // and the mobile number is: 12345678, your final string will be:
        // https://api.whatsapp.com/send?phone=50212345678&text=hello

        Application.OpenURL(url);
    }
}
