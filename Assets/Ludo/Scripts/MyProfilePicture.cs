using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.Networking;
using System.IO;
using System;

public class MyProfilePicture : MonoBehaviour
{
    public static MyProfilePicture instance;

    [Header("-----for image-----")]
    public Image profileImage;
    public Button profileImageButton;
    string profileImagePath = "";
    bool isChangingProfile = false;
    //byte[] newProfilePic;
    int stepSuccessCountForMsg = 0;

    void Awake()
    {
        instance = this;
    }

    public void ChangePicture()
    {
        isChangingProfile = true;
        OpenGallery(profileImage);
        MyProfile.instance.ShowMaskForWaitingMyProfile();

        L_MainMenuController.instance.PlayButtonSound();
    }

    private void OpenGallery(Image image)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);

            if (isChangingProfile)
                profileImagePath = path;

            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path/*, maxSize*/);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    MyProfile.instance.ShowMyProfile();
                    return;
                }

                image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero);
                stepSuccessCountForMsg++;
                StartCoroutine(SaveProfileData());
                isChangingProfile = false;
            }
            else
            {
                MyProfile.instance.ShowMyProfile();
            }
        }, "Select a PNG image", "image/png");
    }


    IEnumerator SaveProfileData()
    {
        Debug.Log("Start Coroutine To Upload Image");

        Dictionary<string, string> headerData = new Dictionary<string, string>
        {
            { "token", (L_GlobalGameManager.playerToken.Length > 0 ? L_GlobalGameManager.playerToken : "") }
        };

        Debug.Log("profileImagePath=" + profileImagePath);

        byte[] profileImg = (profileImagePath.Length > 0 ? File.ReadAllBytes(profileImagePath) : null);

        //newProfilePic = profileImg;

        List<IMultipartFormSection> requestData = new List<IMultipartFormSection>();

        if (profileImagePath.Length > 0)
            requestData.Add(new MultipartFormFileSection("image", profileImg, profileImagePath, "image/png"));

        //if (fullName.text.Length > 0)
        //    requestData.Add(new MultipartFormDataSection("name", fullName.text));

        //if (dob.text.Length > 0)
        //{
        //    string dobDateStr = dob.text;
        //    string[] dtStr = dobDateStr.Split('-');
        //    string utcstr = dtStr[2] + "-" + dtStr[0] + "-" + dtStr[1];
        //    DateTime dt = DateTimeOffset.Parse(utcstr).DateTime;
        //    string dtText = dt.ToString(@"yyyy'-'MM'-'dd");

        //    if (dtText.Length > 0)
        //        requestData.Add(new MultipartFormDataSection("dob", dtText));
        //}

        Debug.Log("Request => UpdateProfile: " + requestData);

        UnityWebRequest www = UnityWebRequest.Post(L_GameConstant.GAME_URLS[(int)L_RequestType.UploadProfilePicture], requestData);

        if (headerData.Count > 0)
        {
            foreach (var param in headerData)
            {
                www.SetRequestHeader(param.Key, param.Value);
            }
        }

        yield return www.SendWebRequest();

        Debug.Log("Upload request Success....");


        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error.ToString());
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Error in Upload", "red", 2f));
            //saveProfileBtn.interactable = true;
            MyProfile.instance.ShowMyProfile();
        }
        else
        {
            //pathText.text = www.downloadHandler.text;
            JsonData jsonData = JsonMapper.ToObject(www.downloadHandler.text);
            Debug.Log("Response => UpdateProfilePic: " + jsonData.ToJson());

            IDictionary iData1 = jsonData as IDictionary;
            Debug.Log("iData1");
            bool isError = false;

            if (iData1.Contains("code"))
            {
                Debug.Log("iData1 code");
                if (jsonData["code"].ToString() == "200")
                {
                    Debug.Log("url: " + jsonData["data"]["key"].ToString());
                    //StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Picture Uploaded Successfully", "green", 10f));
                    stepSuccessCountForMsg++;
                    UpdateUserProfilePic(jsonData["data"]["key"].ToString());
                    //UpdateUserProfilePic("{\"profilePic\": \"" + jsonData["data"]["url"].ToString() + "\"}", "Profile Picture");
                }
                else
                {
                    isError = true;
                }
            }
            else
            {
                isError = true;
            }

            if (isError)
            {
                MyProfile.instance.ShowMyProfile();
                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Error in UpdateProfilePic!", "red", 2f));
                //saveProfileBtn.interactable = true;
            }
        }
    }



    void UpdateUserProfilePic(string keyStr)
    {
        MyProfile.instance.inputMaskImage.enabled = true;
        MyProfile.instance.updateButton.interactable = false;

        if (string.IsNullOrEmpty(keyStr))
        {
            MyProfile.instance.ShowMyProfile();
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Error in Profile Picture", "red", 2f));
        }
        else
        {
            string jsonData = "{\"profilePic\": \"" + keyStr + "\"}";
            Debug.Log("User Update json: " + jsonData);

            StartCoroutine(L_WebServices.instance.PUTRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.UpdateUser] + L_PlayerManager.instance.GetPlayerGameData().userId, jsonData, (serverResponse, errorBool, error) =>
            {
                if (errorBool)
                {
                    MyProfile.instance.ShowMyProfile();
                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Error in Profile Picture!", "red", 2f));
                    Debug.Log("Error in Profile Picture: " + error);
                }
                else
                {
                    Debug.Log("Profile Picture response: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    IDictionary iData1 = data as IDictionary;

                    if (iData1.Contains("code"))
                    {
                        if (data["code"].ToString() == "200")
                        {
                            //StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(errorText, data["data"]["message"].ToString(), "green", 2f));
                            //MainMenuController.instance.ShowScreen(MainMenuScreens.Profile);

                            try
                            {
                                stepSuccessCountForMsg++;
                                //MainMenuController.instance.GetProfilePictureLink(keyStr, profileImage);
                                StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.UploadProfilePicture] + "?key=" + keyStr, (serverResponse, errorBool, error) =>
                                {
                                    bool isError = false;
                                    if (errorBool)
                                    {
                                        Debug.Log("Error in Profile Picture Link: " + error);
                                        isError = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Profile Picture Link response: " + serverResponse);
                                        JsonData data = JsonMapper.ToObject(serverResponse);

                                        IDictionary iData1 = data as IDictionary;

                                        if (iData1.Contains("code"))
                                        {
                                            if (data["code"].ToString() == "200")
                                            {
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(data["data"]["url"].ToString()))
                                                    {
                                                        if (
                                                        (string.IsNullOrEmpty(L_PlayerManager.instance.GetPlayerGameData().avatarURL)) ||
                                                        (!L_PlayerManager.instance.GetPlayerGameData().avatarURL.Equals(data["data"]["url"].ToString()))
                                                        )
                                                        {
                                                            Debug.Log("PIC EditProfile way 1");
                                                            // set new url into Player....avatarURL & load image

                                                            try
                                                            {
                                                                L_MainMenuController.instance.SetImageFromURL(data["data"]["url"].ToString(), profileImage);
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Error in Profile Picture Link", "red", 2f));
                                                                Debug.Log("Error in Load Profile Picture: " + e.Message);
                                                            }
                                                            MyProfile.instance.ShowMyProfile();

                                                            Debug.Log("PIC EditProfile way 1 SET " + L_PlayerManager.instance.GetPlayerGameData().avatarURL);
                                                        }

                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Debug.Log("error in Profile Picture Link " + e.Message);
                                                    isError = true;
                                                }
                                            }
                                            else
                                            {
                                                Debug.Log("Error in Profile Picture Link 2: " + error);
                                                isError = true;
                                            }
                                        }
                                        else
                                        {
                                            Debug.Log("Error in Profile Picture Link 3: " + error);
                                            isError = true;
                                        }
                                    }
                                    if (isError)
                                    {
                                        MyProfile.instance.ShowMyProfile();
                                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Error in Profile Picture Link", "red", 2f));
                                    }
                                }));
                            }
                            catch
                            {
                                MyProfile.instance.ShowMyProfile();
                                Debug.Log("error in display profile picture");
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Error in diplay profile picture", "red", 2f));
                            }
                        }
                        else if (iData1.Contains("errorMessage"))
                        {
                            MyProfile.instance.ShowMyProfile();
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, data["errorMessage"].ToString(), "red", 2f));
                        }
                        else
                        {
                            MyProfile.instance.ShowMyProfile();
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Error in Profile Picture!!", "red", 2f));
                            Debug.Log("Error in Profile Picture 2");
                        }
                    }
                    else
                    {
                        MyProfile.instance.ShowMyProfile();
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(MyProfile.instance.errorText, "Error in Profile Picture!!!", "red", 2f));
                        Debug.Log("Error in Profile Picture 3");
                    }
                }
                MyProfile.instance.inputMaskImage.enabled = false;
                MyProfile.instance.updateButton.interactable = true;
            }));
        }
    }



    

}
