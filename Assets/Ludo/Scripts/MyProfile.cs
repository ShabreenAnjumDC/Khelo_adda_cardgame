using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;
using UnityEngine.Networking;
using System.IO;
using System;

public class MyProfile : MonoBehaviour
{
    public static MyProfile instance;

    [Header("-----InputField-----")]
    public InputField usernameInput;
    public InputField nameInput;
    public InputField mobileInput;
    public InputField emailInput;
    public Image inputMaskImage;
    public Button updateButton;
    public TMP_Text errorText;
    public Button closeButton;

    [Header("-----Text-----")]
    public Text totalMatchesText;
    public Text winMatchesText;
    public Text moneyEarnedText;
    public Text verificationText;
    public Text kycPendingText;

    [Space(10)]
    [Header("-----Email Verify related-----")]
    [Header("-----Parent-----")]
    public GameObject myProfileParent;
    public GameObject otpParent;
    public GameObject otpPopUp;
    public GameObject maskForWaiting;
    public GameObject maskForWaitingMyProfile;
    
    [Header("-----email buttons texts-----")]
    public Button verifyEmailButton;
    public Button verifyEmailOTPButton;
    public Button resendEmailOTPButton;
    public Transform resendOnTextParent;
    public Text otpSecondsText;
    public TMP_Text verifyEmailButtonText;
    public TMP_Text verifyEmailErrorText;
    public TMP_Text maskForWaitingText;
    public Button otpBackButton;

    [Header("-----OTP input fields-----")]
    public InputField[] otpInputsArr;

    //[Header("-----for image-----")]
    //public Image profileImage;
    //public Button profileImageButton;
    //private string profileImagePath = "";
    //private bool isChangingProfile = false;
    //private byte[] newProfilePic;

    Coroutine coroutineRsendOTP;

    bool isEmailVerified;
    string apiUsername;
    string apiName;
    string apiMobile;
    string apiEmail;

    string oldIsEmailVerifiedBtnText;
    bool oldIsEmailVerified;
    bool oldInteractableEmail;
    bool oldVerificationTextActive;

    bool isFromEmailChange = false;


    void Awake()
    {
        instance = this;
    }

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "close":
                //MainMenuController.instance.DestroyScreen(MainMenuScreens.Profile);
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                break;

            case "kyc":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.KYC);
                break;

            case "referral_history":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.ReferralHistory);
                break;

            case "game_history":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.GameHistory);
                break;

            case "name_update":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.NameUpdate);
                NameUpdate.instance.options = NameUpdate.UpdateOptions.NameUpdate;
                break;

            case "username_update":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.NameUpdate);
                NameUpdate.instance.options = NameUpdate.UpdateOptions.UsernameUpdate;
                break;

            case "update":
                UpdateUser();
                break;

            case "verify_email":  //from profile
                verifyEmailButton.interactable = false;
                myProfileParent.SetActive(false);
                otpParent.SetActive(true);
                otpPopUp.SetActive(false);
                maskForWaiting.SetActive(true);
                if (verifyEmailButtonText.text == "Send OTP")
                    isFromEmailChange = true;
                else
                    isFromEmailChange = false;
                SendOtpToEmail();
                break;


            //otp screen object

            case "close_verify_email_otp":  
                if (coroutineRsendOTP != null)
                    StopCoroutine(coroutineRsendOTP);
                otpParent.SetActive(false);
                otpPopUp.SetActive(false);
                maskForWaiting.SetActive(true);
                myProfileParent.SetActive(true);
                verifyEmailButton.interactable = true;
                ResetOTPText();
                break;

            case "verify_email_otp_btn":
                SendVerifyEmail();
                break;

            case "resend_otp_btn":
                ReSendOtp();
                break;

            case "sent_email_otp":
                break;

            case "change_picture":
                //isChangingProfile = true;
                //OpenGallery(profileImage);
                break;

            case "save_picture":
                //StartCoroutine(SaveProfileData());
                break;
        }
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(L_PlayerManager.instance.GetPlayerGameData().avatarURL))
        {
            Davinci.get().setFadeTime(0).load(L_PlayerManager.instance.GetPlayerGameData().avatarURL).into(MyProfilePicture.instance.profileImage).start();
        }

        StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.Profile], (serverResponse, errorBool, error) =>
        {
            if (errorBool)
            {
                Debug.Log("Error in Profile: " + error);
            }
            else
            {
                Debug.Log("Profile response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("code"))
                {
                    if (data["code"].ToString() == "200")
                    {
                        if (data["data"]["userData"]["username"] != null)
                            usernameInput.text = data["data"]["userData"]["username"].ToString();

                        nameInput.text = data["data"]["userData"]["name"].ToString();
                        mobileInput.text = data["data"]["userData"]["mobile"].ToString();
                        emailInput.text = data["data"]["userData"]["email"].ToString();

                        apiUsername = usernameInput.text;
                        apiName = nameInput.text;
                        apiMobile = mobileInput.text;
                        apiEmail = emailInput.text;

                        emailInput.onValueChanged.AddListener(delegate { EmailOnValueChanged(); });

                        if ((bool)data["data"]["userData"]["isEmailVerified"] == false)
                        {
                            verificationText.gameObject.SetActive(true);
                            isEmailVerified = false;
                            //verifyEmailButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Verify";
                            verifyEmailButtonText.text = "Verify";
                            verifyEmailButton.interactable = true;
                        }
                        else
                        {
                            verificationText.gameObject.SetActive(false);
                            isEmailVerified = true;
                            //verifyEmailButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Verified";
                            verifyEmailButtonText.text = "Verified";
                            verifyEmailButton.interactable = false;
                        }

                        oldIsEmailVerified = isEmailVerified;
                        oldInteractableEmail = verifyEmailButton.interactable;
                        oldVerificationTextActive = verificationText.gameObject.activeSelf;
                        oldIsEmailVerifiedBtnText = verifyEmailButton.transform.GetChild(0).GetComponent<TMP_Text>().text;

                        //if (data["data"]["userData"]["kyc"].ToString() == "No")
                        //    kycPendingText.gameObject.SetActive(true);
                        //else
                        //    kycPendingText.gameObject.SetActive(false);

                        try
                        {
                            if (data["data"]["userData"]["profilePic"].ToString().Length > 0)
                            {
                                if (MyProfilePicture.instance != null)
                                    L_MainMenuController.instance.GetProfilePictureLink(data["data"]["userData"]["profilePic"].ToString(), MyProfilePicture.instance.profileImage);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log("error in display profile pic " + e.Message);
                        }
                    }
                    else
                    {
                        Debug.Log("Error in Profile 2: " + error);
                    }
                }
                else
                {
                    Debug.Log("Error in Profile 3: " + error);
                }
            }
        }));

        StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.GameInfo] + L_PlayerManager.instance.GetPlayerGameData().userId, (serverResponse, errorBool, error) => {
            if (errorBool)
            {
                Debug.Log("Error in Game Info: " + error);
            }
            else
            {
                Debug.Log("Game Info response: " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                IDictionary iData1 = data as IDictionary;

                if (iData1.Contains("code"))
                {
                    if (data["code"].ToString() == "200")
                    {
                        totalMatchesText.text = data["data"]["info"][0]["gamePlayed"].ToString();
                        winMatchesText.text = data["data"]["wins"].ToString();
                        float winAmtFloat = 0f;
                        if (float.TryParse(data["data"]["info"][0]["WinningAmount"].ToString(), out winAmtFloat))
                        {
                            moneyEarnedText.text = "₹" + winAmtFloat.ToString("N2");
                        }
                    }
                }
            }
        }));
    }

    public void EmailOnValueChanged()
    {
        Debug.Log("emailInput.text=" + emailInput.text + ", apiEmail=" + apiEmail);

        verifyEmailButton.interactable = true;
        
        if (emailInput.text == apiEmail)
        {
            verifyEmailButtonText.text = oldIsEmailVerifiedBtnText;
            isEmailVerified = oldIsEmailVerified;
            verificationText.gameObject.SetActive(oldVerificationTextActive);
            verifyEmailButton.interactable = oldInteractableEmail;
            updateButton.interactable = true;
        }
        else
        {
            verificationText.gameObject.SetActive(true);
            isEmailVerified = false;
            verifyEmailButtonText.text = "Send OTP";
            verifyEmailButton.interactable = true;
            updateButton.interactable = false;
        }
    }

    void UpdateUser()
    {
        inputMaskImage.enabled = true;
        updateButton.interactable = false;

        //bool isUsernameChanged, isNameChanged, isEmailChanged;

        //if (!string.IsNullOrEmpty(apiUsername))
        //{
        //    if (!apiUsername.Equals(usernameInput.text))
        //    {
        //        isUsernameChanged = true;
        //    }
        //}
        //if (!string.IsNullOrEmpty(apiName))
        //{
        //    if (!apiName.Equals(nameInput.text))
        //    {
        //        isNameChanged = true;
        //    }
        //}
        //if (!isEmailVerified)
        //{
        //    if(!string.IsNullOrEmpty(apiEmail))
        //    {
        //        if (!apiEmail.Equals(emailInput.text))
        //        {
        //            isEmailChanged = true;
        //        }
        //    }
        //}


        if (string.IsNullOrEmpty(usernameInput.text))
        {
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Enter Username", "red", 2f));
        }
        else if (string.IsNullOrEmpty(nameInput.text))
        {
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Enter Name", "red", 2f));
        }
        //else if (string.IsNullOrEmpty(emailInput.text))
        //{
        //    StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(errorText, "Enter Email", "red", 2f));
        //}
        else
        {
            string jsonData = string.Empty;
            //if (isEmailVerified)
            //{
                jsonData = "{" +
                "\"username\":\"" + usernameInput.text + "\"," +
                "\"name\":\"" + nameInput.text + "\"" +
                "}";
            //}
            //else
            //{
            //    jsonData = "{" +
            //    "\"username\":\"" + usernameInput.text + "\"," +
            //    "\"name\":\"" + nameInput.text + "\"," +
            //    "\"email\":\"" + emailInput.text + "\"" +
            //    "}";
            //}
            Debug.Log("User Update json: " + jsonData);


            StartCoroutine(L_WebServices.instance.PUTRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.UpdateUser] + L_PlayerManager.instance.GetPlayerGameData().userId, jsonData, (serverResponse, errorBool, error) =>
            {
                if (errorBool)
                {
                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Update User", "red", 2f));
                    Debug.Log("Error in Update User: " + error);
                }
                else
                {
                    Debug.Log("Update User response: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    IDictionary iData1 = data as IDictionary;

                    if (iData1.Contains("code"))
                    {
                        if (data["code"].ToString() == "200")
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, data["data"]["message"].ToString(), "green", 2f));
                            if (Dashboard.instance != null)
                                Dashboard.instance.GetProfile("myprofile");
                        }
                        else if (iData1.Contains("errorMessage"))
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, data["errorMessage"].ToString(), "red", 2f));
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Update User.", "red", 2f));
                            Debug.Log("Error in Update User 1");
                        }
                    }
                    else
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Update User!", "red", 2f));
                        Debug.Log("Error in Update User 2");
                    }
                }
                inputMaskImage.enabled = false;
                updateButton.interactable = true;
            }));
        }
    }

    void UpdateUserEmail()
    {
        inputMaskImage.enabled = true;
        updateButton.interactable = false;

       if (string.IsNullOrEmpty(emailInput.text))
        {
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Enter Email", "red", 2f));
        }
        else
        {
            string jsonData = "{\"email\":\"" + emailInput.text + "\"}";
            
            Debug.Log("User Update json: " + jsonData);

            StartCoroutine(L_WebServices.instance.PUTRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.UpdateUser] + L_PlayerManager.instance.GetPlayerGameData().userId, jsonData, (serverResponse, errorBool, error) =>
            {
                if (errorBool)
                {
                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Update User", "red", 2f));
                    Debug.Log("Error in Update User: " + error);
                }
                else
                {
                    Debug.Log("Update User response: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    IDictionary iData1 = data as IDictionary;

                    if (iData1.Contains("code"))
                    {
                        if (data["code"].ToString() == "200")
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, data["data"]["message"].ToString(), "green", 2f));
                            //if (Dashboard.instance != null)
                            //    Dashboard.instance.GetProfile();
                            L_MainMenuController.instance.ShowScreen(MainMenuScreens.Profile);
                        }
                        else if (iData1.Contains("errorMessage"))
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, data["errorMessage"].ToString(), "red", 2f));
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Update User.", "red", 2f));
                            Debug.Log("Error in Update User 1");
                        }
                    }
                    else
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Update User!", "red", 2f));
                        Debug.Log("Error in Update User 2");
                    }
                }
                inputMaskImage.enabled = false;
                updateButton.interactable = true;
            }));
        }
    }

    void SendOtpToEmail()
    {
        if (string.IsNullOrEmpty(emailInput.text))
        {
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Enter Email", "red", 2f));
            ErrorMsgPanelHideInShowSendOtpToEmail();
        }
        else
        {
            otpBackButton.interactable = false;
            StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.EmailSendOtp] + emailInput.text, (serverResponse, errorBool, error) =>
            {
                otpBackButton.interactable = true;
                if (errorBool)
                {
                    Debug.Log("Error in Send OTP to Email: " + error);
                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Send OTP to Email", "red", 2f));
                    ErrorMsgPanelHideInShowSendOtpToEmail();
                }
                else
                {
                    Debug.Log("Send OTP to Email response: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    IDictionary iData1 = data as IDictionary;

                    if (iData1.Contains("code"))
                    {
                        if (data["code"].ToString() == "200")
                        {
                            verifyEmailButton.interactable = true;
                            myProfileParent.SetActive(false);
                            otpParent.SetActive(true);
                            maskForWaiting.SetActive(false);
                            otpPopUp.SetActive(true);
                            coroutineRsendOTP = StartCoroutine(OtpExipreCo());
                        }
                        else if (iData1.Contains("errorMessage"))
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, data["errorMessage"].ToString(), "red", 2f));
                            ErrorMsgPanelHideInShowSendOtpToEmail();
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Send OTP to Email.", "red", 2f));
                            Debug.Log("Error in Send OTP to Email 1");
                            ErrorMsgPanelHideInShowSendOtpToEmail();
                        }
                    }
                    else
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Send OTP to Email!", "red", 2f));
                        Debug.Log("Error in Send OTP to Email 2");
                        ErrorMsgPanelHideInShowSendOtpToEmail();
                    }
                }
            }));
        }
    }

    void ErrorMsgPanelHideInShowSendOtpToEmail()
    {
        verifyEmailButton.interactable = true;
        otpParent.SetActive(false);
        myProfileParent.SetActive(true);
    }

    void SendVerifyEmail()
    {
        verifyEmailOTPButton.interactable = false;
        resendEmailOTPButton.interactable = false;

        if (string.IsNullOrEmpty(emailInput.text))
        {
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(verifyEmailErrorText, "Enter Email", "red", 2f));
        }
        else
        {
            string otpErrorStr = string.Empty;
            for (int i = 0; i < otpInputsArr.Length; i++)
            {
                if (string.IsNullOrEmpty(otpInputsArr[i].text))
                {
                    otpErrorStr += ("otp " + (i + 1) + "  ");
                }
            }

            if (!string.IsNullOrEmpty(otpErrorStr))
            {
                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(verifyEmailErrorText, "Enter otp")); // + otpErrorStr
                verifyEmailOTPButton.interactable = true;
                resendEmailOTPButton.interactable = true;
            }
            else
            {
                string otpStr = string.Empty;
                for (int i = 0; i < otpInputsArr.Length; i++)
                {
                    otpStr += otpInputsArr[i].text;
                }
                string jsonData = "{\"email\": \"" + emailInput.text + "\", \"otp\": \"" + otpStr + "\" }";
                Debug.Log("Email verify jsonData=" + jsonData);

                StartCoroutine(L_WebServices.instance.POSTRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.EmailVerifyOtp], jsonData, (serverResponse, errorBool, error) =>
                {
                    if (errorBool)
                    {
                        Debug.Log("Error in Verify OTP to Email: " + error);
                        verifyAndSendOtpBtnInteractable();
                    }
                    else
                    {
                        Debug.Log("Verify OTP to Email response: " + serverResponse);
                        JsonData data = JsonMapper.ToObject(serverResponse);

                        IDictionary iData1 = data as IDictionary;

                        if (iData1.Contains("code"))
                        {
                            if (data["code"].ToString() == "200")
                            {
                                StopCoroutine(coroutineRsendOTP);
                                otpPopUp.SetActive(false);
                                otpPopUp.SetActive(false);
                                maskForWaiting.SetActive(true);
                                maskForWaitingText.text = data["data"]["message"].ToString();
                                StartCoroutine(OnVerifyOtpSuccess());
                            }
                            else if (iData1.Contains("errorMessage"))
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(verifyEmailErrorText, data["errorMessage"].ToString(), "red", 2f));
                                verifyAndSendOtpBtnInteractable();
                            }
                            else
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(verifyEmailErrorText, "Error in Verify OTP to Email.", "red", 2f));
                                Debug.Log("Error in Verify OTP to Email 1");
                                verifyAndSendOtpBtnInteractable();
                            }
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(verifyEmailErrorText, "Error in Verify OTP to Email!", "red", 2f));
                            Debug.Log("Error in Verify OTP to Email 2");
                            verifyAndSendOtpBtnInteractable();
                        }
                    }
                }));
            }
        }
    }

    IEnumerator OnVerifyOtpSuccess()
    {
        yield return new WaitForSeconds(3f);
        if (isFromEmailChange)
        {
            UpdateUserEmail();
        }
        else
        {
            L_MainMenuController.instance.ShowScreen(MainMenuScreens.Profile);
        }
    }

    void ReSendOtp()
    {
        if (resendOnTextParent.gameObject.activeInHierarchy)
        {
            ResetOTPText();
            verifyEmailOTPButton.interactable = false;
            resendEmailOTPButton.interactable = false;

            if (string.IsNullOrEmpty(emailInput.text))
            {
                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(verifyEmailErrorText, "Enter Email", "red", 2f));
            }
            else
            {
                StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.EmailSendOtp] + emailInput.text, (serverResponse, errorBool, error) =>
                {
                    if (errorBool)
                    {
                        Debug.Log("Error in Resend OTP to Email: " + error);
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorText, "Error in Resend OTP to Email", "red", 2f));
                        verifyAndSendOtpBtnInteractable();
                    }
                    else
                    {
                        Debug.Log("Resend OTP to Email response: " + serverResponse);
                        JsonData data = JsonMapper.ToObject(serverResponse);

                        IDictionary iData1 = data as IDictionary;

                        if (iData1.Contains("code"))
                        {
                            if (data["code"].ToString() == "200")
                            {
                                verifyEmailOTPButton.interactable = true;
                                resendEmailOTPButton.interactable = true;
                                coroutineRsendOTP = StartCoroutine(OtpExipreCo());
                            }
                            else if (iData1.Contains("errorMessage"))
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(verifyEmailErrorText, data["errorMessage"].ToString(), "red", 2f));
                                verifyAndSendOtpBtnInteractable();
                            }
                            else
                            {
                                Debug.Log("Error in Resend OTP to Email 1");
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(verifyEmailErrorText, "Error in Resend OTP to Email.", "red", 2f));
                                verifyAndSendOtpBtnInteractable();
                            }
                        }
                        else
                        {
                            Debug.Log("Error in Resend OTP to Email 2");
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(verifyEmailErrorText, "Error in Resend OTP to Email!", "red", 2f));
                            verifyAndSendOtpBtnInteractable();
                        }
                    }
                }));
            }
        }
    }

    Dictionary<int, bool> otpFields = new Dictionary<int, bool>();
    public void VerifyOTP(int otpNum)
    {
        if (!string.IsNullOrEmpty(otpInputsArr[otpNum].text))
        {
            otpFields.Add(otpNum, true);
            if (otpNum < 5)
                otpInputsArr[otpNum + 1].Select();
        }

        if (otpFields.Count == 6)
        {
            string otp = otpInputsArr[0].text + otpInputsArr[1].text + otpInputsArr[2].text + otpInputsArr[3].text + otpInputsArr[4].text + otpInputsArr[5].text;
            Debug.Log("otp: " + otp);
        }
        if (string.IsNullOrEmpty(otpInputsArr[otpNum].text) && otpFields.ContainsKey(otpNum) && otpFields[otpNum] == true)
        {
            if (otpNum > 0)
                otpInputsArr[otpNum - 1].Select();
            otpFields.Remove(otpNum);
        }
    }

    void ResetOTPText()
    {
        for (int i = 0; i < otpInputsArr.Length; i++)
        {
            otpInputsArr[i].text = "";
        }
    }

    void verifyAndSendOtpBtnInteractable()
    {
        verifyEmailOTPButton.interactable = true;
        resendEmailOTPButton.interactable = true;
    }

    IEnumerator OtpExipreCo()
    {
        int timer = 60;
        if (resendOnTextParent.gameObject.activeInHierarchy)
        {
            ResetResend();
        }
        for (int i = 1; i <= 60; i++)
        {
            yield return new WaitForSeconds(1f);
            timer--;
            if (timer == 0)
            {
                SetResendParentOn();
            }
            else
            {
                otpSecondsText.text = "Enter the OTP in " + timer.ToString() + " sec";
            }
        }
    }

    void SetResendParentOn()
    {
        otpSecondsText.text = "Enter the OTP in 60 sec";
        otpSecondsText.gameObject.SetActive(false);
        resendOnTextParent.gameObject.SetActive(true);
    }

    void ResetResend()
    {
        otpSecondsText.text = "Enter the OTP in 60 sec";
        resendOnTextParent.gameObject.SetActive(false);
        otpSecondsText.gameObject.SetActive(true);
    }

    public void ShowMaskForWaitingMyProfile()
    {
        myProfileParent.SetActive(false);
        otpParent.SetActive(false);
        otpPopUp.SetActive(false);
        maskForWaiting.SetActive(false);
        maskForWaitingMyProfile.SetActive(true);
    }

    public void ShowMyProfile()
    {
        otpParent.SetActive(false);
        otpPopUp.SetActive(false);
        maskForWaiting.SetActive(false);
        maskForWaitingMyProfile.SetActive(false);
        myProfileParent.SetActive(true);
    }

}
