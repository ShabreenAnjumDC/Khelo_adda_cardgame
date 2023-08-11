using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CloseScreen(string screenName)
    {
        switch (screenName)
        {
            case "GamePolicyScreen":
                MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.GamePolicy);
                break;
            
            case "PrivacyPolicyScreen":
                MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.PrivacyPolicy);
                break;

            case "TermsAndConditionsScreen":
                MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.TermsAndConditions);
                break;

            default:
                break;
        }

        //MainDashboardScreen.instance.DestroyScreen(screen);
    }
}
