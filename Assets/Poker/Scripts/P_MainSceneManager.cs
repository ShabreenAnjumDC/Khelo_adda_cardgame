using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;
using System;

public class P_MainSceneManager : MonoBehaviour
{
    public static P_MainSceneManager instance;

    GameObject previousScene;
    public List<GameObject> gameScens;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadScene(P_MainScenes.LobbyScene);
    }

    public void ScreenDestroy()
    {
        Destroy(previousScene);
    }

    public void LoadScene(P_MainScenes gameScene)
    {
        if (previousScene != null)
        {
            StartCoroutine(WaitAndDestroyOldScreen(previousScene));
        }
        previousScene = Instantiate(gameScens[(int)gameScene], Vector3.zero, Quaternion.identity) as GameObject;
    }

    IEnumerator WaitAndDestroyOldScreen(GameObject gm)
    {
        yield return new WaitForSeconds(1);

        if (gm != null)
        {
            //if (gm.name == "MainDashboardScene(Clone)" || gm.name == "MainMenuScene(Clone)" || gm.name == "InGame(Clone)")
            //{
            Destroy(gm);
            //}
            //else
            //{
            //    gm.SetActive(false);    //DEV_CODE This line is added
            //}
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Null reference exception found gm is null in GlobalGameManager.WaitAndDestroyOldScreen ");
#endif
        }

        yield return new WaitForSeconds(0.01f);
        System.GC.Collect();
    }

    //public bool IsScreenActive(MainMenuScreens screenName)
    //{
    //    for (int i = 0; i < gameScens.Count; i++)
    //    {
    //        if (gameScens[i].screenName == screenName)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //public void DestroyScreen(MainMenuScreens screenName)
    //{
    //    for (int i = 0; i < gameScens.Count; i++)
    //    {
    //        if (gameScens[i].screenName == screenName)
    //        {
    //            Destroy(gameScens[i].gameObject);
    //            gameScens.RemoveAt(i);
    //        }
    //    }
    //}

    public IEnumerator RunAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}

public enum P_MainScenes
{
    //LoginScreen,
    LobbyScene,
    InGame
}
