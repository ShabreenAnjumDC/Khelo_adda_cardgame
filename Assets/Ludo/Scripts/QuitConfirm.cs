using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitConfirm : MonoBehaviour
{
    public static QuitConfirm instance;
    public bool isQuitApp = true;

    void Awake()
    {
        instance = this;
    }

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "yes":
                if (
                    L_MainMenuController.instance.IsScreenActive(MainMenuScreens.PlayerFinding) ||
                    L_MainMenuController.instance.IsScreenActive(MainMenuScreens.ClassicLudoGamePlay) || 
                    L_MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentPlayerFinding) ||
                    L_MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentGamePlay)
                )
                {
                    if (L_SocketController.instance != null)
                    {
                        if (L_SocketController.instance.GetSocketState() == L_SocketState.Connected)
                        {
                            L_SocketController.instance.RemovePlayerFromGame();
                        }
                    }
                    if (T_SocketController.instance != null)
                    {
                        if (T_SocketController.instance.GetSocketState() == T_SocketState.Connected)
                        {
                            T_SocketController.instance.SendRemovePlayer();
                        }
                    }
                }

                if (isQuitApp)
                {
                    Application.Quit();
                }
                else if (
                    L_MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentPlayerFinding) ||
                    L_MainMenuController.instance.IsScreenActive(MainMenuScreens.TournamentGamePlay)
                )
                {
                    L_MainMenuController.instance.ShowScreen(MainMenuScreens.Tournaments);
                }
                break;

            case "no":
                if (Panel_Controller.instance != null)
                {
                    Panel_Controller.instance.ShowBackButtonAndPanel();
                }
                if (T_Panel_Controller.instance != null)
                {
                    T_Panel_Controller.instance.ShowBackButtonAndPanel();
                }
                L_MainMenuController.instance.DestroyScreen(MainMenuScreens.QuitConfirm);
                break;
        }
    }
}
