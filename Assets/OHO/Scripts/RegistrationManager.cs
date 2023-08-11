using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RegistrationManager : MonoBehaviour
{
    public static RegistrationManager instance = null;

    public InputField registrationMobileNumber, createPassword, loginMobileNumber, loginWithOTPMobileNumber, loginPassword, forgetMobileNumber, newPassword, confirmPassword;
    public Text forgetMobileInfoText;

    public GameObject registrationScreen, registrationVerifyScreen, createPasswordScreen, loginScreenWithPassword, loginScreenWithOTP, forgotPassword, forgotPasswordVerify, forgotPasswordSuccess, createNewPassword;
    public InputField registrationEmail, registrationPassword, registrationConfirmPassword;
    public InputField loginUserName;
    //public InputField newPassword;
    public Button resendEmailBtnLoginScreen, resendEmailBtnRegSuccess, regSuccessDoneBtn;

    public Text popUpText, wrongPasswordText, statusText;

    public string registrationType = "";

    [Header("Forgot Password")]
    public InputField forgotEmailInputField;
    public InputField verificationCodeInputField;
    public Button getVerificationCodeBtn;
    public Button forgotpwSubmitBtn;
    public InputField resetPasswordInputField;
    public InputField resetConfirmPasswordInputField;
    public GameObject forgotpwWrongEmail;
    public GameObject wrongVeriCode;
    public GameObject imgForDisableEmailObjs;
    public GameObject imgForDisableCodeObjs;

    private float timer = 0;
    private string verificationCode = string.Empty;
    private string userId;
    private string deviceId = "", deviceType = "", ipAddress = "";
    private string registrationEmailSuccess, registrationPasswordSuccess, registrationUserIdSuccess;
    private bool isLoginAfterRegistration;
    string otp;
    public Text registerTimerText, registertOtpCountText, forgetTimerText, forgetOtpCountText;
    Text currentTimerText;
    Dictionary<int, bool> mobileOTOFields = new Dictionary<int, bool>();
    public InputField[] mobilePassFields;
    public InputField[] loginWithOTPFields;
    public InputField[] forgetOTPFields;

    IEnumerator GetIpAddress()
    { 
        using (UnityWebRequest request = UnityWebRequest.Get("https://api.bigdatacloud.net/data/client-ip"))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);

                JsonData data = JsonMapper.ToObject(request.downloadHandler.text);
                Debug.Log("Response => IpAddress: " + data["ipString"].ToString());
                ipAddress = data["ipString"].ToString();
            }
        } 
    }

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        //Getting Android Device ID
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
        deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");
        deviceType = "Android";
        string dId = SystemInfo.deviceUniqueIdentifier;

        //statusText.text = "Android ID: " + deviceId + " ******** Device Unique ID: " + dId;

// #elif UNITY_IPHONE
//         deviceId = UnityEngine.iOS.Device.vendorIdentifier;
#elif UNITY_IPHONE
        deviceId = SystemInfo.deviceUniqueIdentifier;
        deviceType = "iOS";
#elif UNITY_EDITOR
        deviceId = SystemInfo.deviceUniqueIdentifier;
        deviceType = "Android"; /*"UnityEditor";*/
#endif
		Debug.Log(deviceId + " - " + deviceType);

        currentTimerText = registerTimerText;
    }

    private void OnEnable()
    {
        //registrationMobileNumber.onSelect.AddListener(OnSelect);

        //verificationCodeInputField.interactable = true;
        //popUpText.gameObject.SetActive(false);
        //wrongPasswordText.gameObject.SetActive(false);
        //forgotPassword.SetActive(false);

        if (GlobalGameManager.instance.isLoginShow)
        {
            registrationScreen.SetActive(false);
            //loginScreen.SetActive(true);
            //signUpScreen.SetActive(false);
        }
        else {
            registrationScreen.SetActive(true);
            //loginScreen.SetActive(false);
            //signUpScreen.SetActive(true);
        }
        StartCoroutine(GetIpAddress());
    }

    private void OnDisable()
    {
        Debug.Log("Game - " + gameObject.name);
    }


    private void FixedUpdate()
    {
        /*if(forgotPassword.activeInHierarchy)
        {
            if (timer > 1)
            {
                verificationCodeInputField.interactable = true;
                timer -= Time.deltaTime;
                getVerificationCodeBtn.transform.GetChild(0).GetComponent<Text>().text = "Resend After " + timer.ToString("f0") + "s";
            }
            else if (timer < 1)
            {
                getVerificationCodeBtn.interactable = true;
                if (wrongVeriCode.GetComponent<Text>().text == "Verification code sent")
                    wrongVeriCode.SetActive(false);
                getVerificationCodeBtn.transform.GetChild(0).GetComponent<Text>().text = "Resend";
                // getVerificationCodeBtn.transform.GetChild(0).GetComponent<Text>().color = new Color32(140, 224, 240, 255);
            }

            if (verificationCodeInputField.text.Length > 0 && forgotEmailInputField.text.Length > 0)
            {
                forgotpwSubmitBtn.GetComponent<Button>().interactable = true;
            }
            else
            {
                forgotpwSubmitBtn.GetComponent<Button>().interactable = false;
            }
        }*/

        if (timer > 1)
        {
            timer -= Time.deltaTime;
            currentTimerText.text = "Request for a new one in <color=#D64F4F>" + timer.ToString("f0") + " Second</color>.";
            /*if (loginOtpScreen.activeSelf || forgotPassword.transform.Find("OTPScreen").gameObject.activeSelf)
                currentTimeText.text = "Request for a new one in <color=#D64F4F>" + timer.ToString("f0") + " Second</color>.";
            else
                currentTimeText.text = string.Format("{0:00}:{1:00}", 0, timer);*/
        }
        else if (timer < 1 && currentTimerText != null)
        {
            timer = 0f;
            currentTimerText.text = "";
        }
    }


    public void OnValueChangedEmail()
    {
        if (forgotpwWrongEmail.activeSelf)
        {
            forgotpwWrongEmail.GetComponent<Text>().text = "";
            forgotpwWrongEmail.SetActive(false);
        }
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    OnClickOnBack();
                }
                break;

            case "requestotp":
                {
                    if (timer > 0)
                        return;

                    string error;
                    registrationType = "custom";
                    if (!Utility.IsValidUserMobile(registrationMobileNumber.text, out error))
                    {
                        Debug.Log(error);
                        MainDashboardScreen.instance.ShowMessage(error);
                    }
                    else
                    {
                        string requestData = "{\"mobile\":\"" + registrationMobileNumber.text + "\"," +
                               "\"device_id\":\"" + deviceId + "\"," +
                               "\"device_type\":\"" + deviceType + "\"}";
                        //PlayerPrefs.SetString("Password", loginPassword.text);
                        //MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                        print("Request Data : " + requestData);
                        WebServices.instance.SendRequest(RequestType.Registration, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            case "verifyotp":
                {
                    if (string.IsNullOrEmpty(otp) || otp.Length < 6)
                    {
                        Debug.Log("Please enter otp");
                        MainDashboardScreen.instance.ShowMessage("Please enter otp");
                        return;
                    }

                    string requestData = "{\"mobile\":\"" + registrationMobileNumber.text + "\"," +
                        "\"type\":\"" + "1" + "\"," +
                           "\"otp\":\"" + otp + "\"}";
                    WebServices.instance.SendRequest(RequestType.VerifyOtp, requestData, true, OnServerResponseFound);
                }
                break;

            case "resendotp":
                {
                    Debug.Log("Timer " + timer);
                    if (timer > 0)
                        return;

                    string requestData = "{\"mobile\":\"" + registrationMobileNumber.text + "\"," +
                           "\"type\":\"" + "1" + "\"}";
                    WebServices.instance.SendRequest(RequestType.ResendOtp, requestData, true, OnServerResponseFound);
                }
                break;

            case "createpassword":
                {
                    string error;
                    if (!Utility.IsValidPassword(createPassword.text, out error))
                    {
                        Debug.Log(error);
                        MainDashboardScreen.instance.ShowMessage(error);
                    }
                    else
                    {
                        string requestData = "{\"mobile\":\"" + registrationMobileNumber.text + "\"," +
                            "\"type\":\"" + "1" + "\"," +
                               "\"password\":\"" + createPassword.text + "\"}";
                        WebServices.instance.SendRequest(RequestType.ResetPassword, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            case "createnewpassword":
                {
                    string error;
                    if (!Utility.IsValidPassword(newPassword.text, out error))
                    {
                        Debug.Log(error);
                        MainDashboardScreen.instance.ShowMessage(error);
                    }
                    else if (!Utility.IsValidPassword(confirmPassword.text, out error))
                    {
                        Debug.Log(error);
                        MainDashboardScreen.instance.ShowMessage(error);
                    }
                    else if (confirmPassword.text != newPassword.text)
                    {
                        Debug.Log("Passwords are not matched");
                        MainDashboardScreen.instance.ShowMessage("Passwords are not matched");
                    }
                    else
                    {
                        string requestData = "{\"mobile\":\"" + forgetMobileNumber.text + "\"," +
                            "\"type\":\"" + "2" + "\"," +
                               "\"password\":\"" + newPassword.text + "\"}";
                        WebServices.instance.SendRequest(RequestType.ResetPassword, requestData, true, OnServerResponseFound);
                        
                    }
                }
                break;

            case "loginwithpassword":
                {
                    string error;
                    registrationType = "custom";
                    if (!Utility.IsValidUserMobile(loginMobileNumber.text, out error))
                    {
                        Debug.Log(error);
                        MainDashboardScreen.instance.ShowMessage(error);
                    }
                    else if (!Utility.IsValidPassword(loginPassword.text, out error))
                    {
                        Debug.Log(error);
                        MainDashboardScreen.instance.ShowMessage(error);
                    }
                    else
                    {
                        string requestData = "{\"mobile\":\"" + loginMobileNumber.text + "\"," +
                               "\"password\":\"" + loginPassword.text + "\"," +
                               "\"type\":\"" + "2" + "\"," +
                               "\"device_id\":\"" + deviceId + "\"," +
                               "\"os_version\":\"" + SystemInfo.operatingSystem + "\"," +
                               "\"mac_address\":\"" + ipAddress + "\"," +
                               "\"app_version\":\"" + "0.0.1" + "\"," +
                               "\"device_type\":\"" + deviceType + "\"}";
                        //PlayerPrefs.SetString("Password", loginPassword.text);
                        //MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                        WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            case "sendotp":
                {
                    string error;
                    registrationType = "custom";
                    if (!Utility.IsValidUserMobile(loginWithOTPMobileNumber.text, out error))
                    {
                        Debug.Log(error);
                        MainDashboardScreen.instance.ShowMessage(error);
                    }
                    else
                    {
                        string requestData = "{\"mobile\":\"" + loginWithOTPMobileNumber.text + "\"," +
                               //"\"password\":\"" + loginPassword.text + "\"," +
                               "\"type\":\"" + "1" + "\"}";
                        //PlayerPrefs.SetString("Password", loginPassword.text);
                        //MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                        WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            case "resendotpforlogin":
                {
                    string error;
                    registrationType = "custom";
                    if (!Utility.IsValidUserMobile(loginWithOTPMobileNumber.text, out error))
                    {
                        Debug.Log(error);
                    }
                    else
                    {
                        string requestData = "{\"mobile\":\"" + loginWithOTPMobileNumber.text + "\"," +
                               //"\"password\":\"" + loginPassword.text + "\"," +
                               "\"type\":\"" + "2" + "\"}";
                        //PlayerPrefs.SetString("Password", loginPassword.text);
                        //MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                        WebServices.instance.SendRequest(RequestType.ResendOtp, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            case "loginwithotp":
                {
                    string error;
                    registrationType = "custom";
                    if (!Utility.IsValidUserMobile(loginWithOTPMobileNumber.text, out error))
                    {
                        Debug.Log(error);
                        MainDashboardScreen.instance.ShowMessage(error);
                    }
                    else if (string.IsNullOrEmpty(otp) || otp.Length < 6)
                    {
                        Debug.Log("Enter otp");
                        MainDashboardScreen.instance.ShowMessage("Enter otp");
                    }
                    else
                    {
                        string requestData = "{\"mobile\":\"" + loginWithOTPMobileNumber.text + "\"," +
                            "\"type\":\"" + "2" + "\"," +
                            "\"otp\":\"" + otp + "\"," +
                            "\"device_id\":\"" + deviceId + "\"," +
                            "\"os_version\":\"" + SystemInfo.operatingSystem + "\"," +
                            "\"mac_address\":\"" + ipAddress + "\"," +
                            "\"app_version\":\"" + "0.0.1" + "\"," +
                            "\"device_type\":\"" + deviceType + "\"}";

                        WebServices.instance.SendRequest(RequestType.VerifyOtp, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            case "requestotpforforget":
                {
                    if (timer > 0)
                        return;

                    string error;
                    registrationType = "custom";
                    if (!Utility.IsValidUserMobile(forgetMobileNumber.text, out error))
                    {
                        Debug.Log(error);
                        MainDashboardScreen.instance.ShowMessage(error);
                    }
                    else
                    {
                        string requestData = "{\"mobile\":\"" + forgetMobileNumber.text + "\"}";

                        WebServices.instance.SendRequest(RequestType.ForgotPassword, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            case "verifyforgetotp":
                {
                    if (string.IsNullOrEmpty(otp) || otp.Length < 6)
                    {
                        Debug.Log("Please enter otp");
                        MainDashboardScreen.instance.ShowMessage("Please enter otp");
                        return;
                    }

                    string requestData = "{\"mobile\":\"" + forgetMobileNumber.text + "\"," +
                        "\"type\":\"" + "3" + "\"," +
                           "\"otp\":\"" + otp + "\"}";
                    WebServices.instance.SendRequest(RequestType.VerifyOtpForForgotPassword, requestData, true, OnServerResponseFound);
                }
                break;

            case "resendotpforforget":
                {
                    if (timer > 0)
                        return;

                    string error;
                    if (!Utility.IsValidUserMobile(forgetMobileNumber.text, out error))
                    {
                        Debug.Log(error);
                    }
                    else
                    {
                        string requestData = "{\"mobile\":\"" + forgetMobileNumber.text + "\"," +
                               //"\"password\":\"" + loginPassword.text + "\"," +
                               "\"type\":\"" + "3" + "\"}";
                        //PlayerPrefs.SetString("Password", loginPassword.text);
                        //MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                        WebServices.instance.SendRequest(RequestType.ResendOtp, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            case "submit":
                {
                    if (loginScreenWithOTP.activeInHierarchy)
                    {
                        string error;
                        registrationType = "custom";

                        if (!Utility.IsValidUserName(/*tmp_loginUserName*/loginUserName.text, out error))
                        {
                            /*wrongPasswordText.gameObject.SetActive(true);*/
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            /*MainMenuController.instance.ShowMessage(error);*/
                            /*return;*/
                            break;
                        }
                        else if (!Utility.IsValidPassword(/*tmp_loginPassword*/loginPassword.text, out error))
                        {
                            /*wrongPasswordText.gameObject.SetActive(true);*/
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            /*MainMenuController.instance.ShowMessage(error);*/
                            /*return;*/
                            break;
                        }
                        else
                        {
                            //string requestData = "{\"userName\":\"" + /*tmp_loginUserName.text*/loginUserName.text + "\"," +
                            //   "\"userPassword\":\"" + /*tmp_loginPassword*/loginPassword.text + "\"," +
                            //   "\"registrationType\":\"" + registrationType + "\"," +
                            //   "\"socialId\":\"\"}";

                            string requestData = "{\"userEmail\":\"" + /*tmp_loginUserName.text*/loginUserName.text/*"pradeep"*/ + "\"," +
                               "\"userPassword\":\"" + /*tmp_loginPassword*/loginPassword.text/*"123456"*/ + "\"," +
                               "\"registrationType\":\"" + registrationType + "\"}";
                            PlayerPrefs.SetString("Password", loginPassword.text);
                            MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                            WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
                        }
                    }
                    else if (registrationScreen.activeInHierarchy)
                    {
                        string error;
                        registrationType = "custom";
                        Debug.Log("Validation " + Utility.IsValidPassword(registrationPassword.text, out error));
                        if (!Utility.IsValidUserName(/*tmp_registrationUserName*/registrationMobileNumber.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            /*MainMenuController.instance.ShowMessage(error);*/
                            /*return;*/
                            break;
                        }
                        else if (!Utility.IsValidUserEmail(/*tmp_registrationUserName*/registrationEmail.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            /*MainMenuController.instance.ShowMessage(error);*/
                            /*return;*/
                            break;
                        }
                        else if (!Utility.IsValidPassword(/*tmp_registrationPassword*/registrationPassword.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            /*MainMenuController.instance.ShowMessage(error);*/
                            /*return;*/
                            break;
                        }
                        //no confirm password in register anymore
                        //else if (/*tmp_registrationConfirmPassword*/registrationConfirmPassword.text != /*tmp_registrationPassword*/registrationPassword.text)
                        //{
                        //    StartCoroutine(MsgForVideo("password does not matched", 1.5f));
                        //    /*MainMenuController.instance.ShowMessage("password does not matched");*/
                        //    /*return;*/
                        //    break;
                        //}
                        else
                        {
                            string requestData = "{\"userName\":\"" + /*tmp_registrationUserName*/registrationMobileNumber.text + "\"," +
                            "\"userEmail\":\"" + /*tmp_registrationPassword*/registrationEmail.text + "\"," +
                            "\"userPassword\":\"" + /*tmp_registrationPassword*/registrationPassword.text + "\"," +
                            "\"registrationType\":\"" + registrationType + "\"," +
                            "\"socialId\":\"\"," +
                             "\"deviceId\":\"" + deviceId + "\"}";

                            MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                            WebServices.instance.SendRequest(RequestType.Registration, requestData, true, OnServerResponseFound);
                        }
                    }
                    else if (forgotPassword.activeInHierarchy)
                    {
                        if (verificationCodeInputField.text.Equals(verificationCode) && forgotEmailInputField.text.Length > 0)
                        {
                            //resetPassword.SetActive(true);
                            ResetForgotPwdScreen();
                            forgotPassword.SetActive(false);
                        }
                        else
                        {
                            wrongVeriCode.GetComponent<Text>().text = "Wrong verification code";
                            wrongVeriCode.GetComponent<Text>().color = Color.red;
                            wrongVeriCode.SetActive(true);
                        }
                    }
                    /*else if(resetPassword.activeInHierarchy)
                    {
                        //MainDashboardScreen.instance.ShowMessage("Going To Reset Password...");
                        Debug.LogError("UserID " + userId);
                        string error;
                        if (!Utility.IsValidPassword(resetPasswordInputField.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            break;
                        }
                        else if (!Utility.IsValidPassword(resetConfirmPasswordInputField.text, out error))
                        {
                            StartCoroutine(MsgForVideo("Please enter confirm password!", 1.5f));
                            break;
                        }
                        else if (resetPasswordInputField.text != resetConfirmPasswordInputField.text)
                        {
                            StartCoroutine(MsgForVideo("Passwords are not matched!", 1.5f));
                            break;
                        }

                        string requestData = "{\"userId\":\"" + userId + "\"," +
                        "\"newPassword\":\"" + resetConfirmPasswordInputField.text + "\"," +
                        "\"confirmPassword\":\"" + resetConfirmPasswordInputField.text + "\"," +
                        "\"otp\":\"" + verificationCode + "\"}";

                        MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                        WebServices.instance.SendRequest(RequestType.changePassword, requestData, true, OnServerResponseFound);
                    }*/
                    else
                    {
#if ERROR_LOG
                        Debug.LogError("Unhandled case in submit in registrationManager");
#endif
                    }
                }
                break;

            case "openSignUp":
                {
                    ResetRegistrationScreen();
                    registrationScreen.SetActive(true);
                    registrationVerifyScreen.SetActive(false);
                    createPasswordScreen.SetActive(false);
                }
                break;

            case "openSignUpVerification":
                {
                    registrationVerifyScreen.SetActive(true);
                    registrationScreen.SetActive(false);
                    createPasswordScreen.SetActive(false);
                }
                break;


            case "openLogin":
                {
                    ResetLoginScreen();
                    //resetPassword.SetActive(false);
                    forgotPassword.SetActive(false);
                    loginScreenWithPassword.SetActive(true);
                    registrationScreen.SetActive(false);
                    loginScreenWithOTP.SetActive(false);
                    createPasswordScreen.SetActive(false);
                    forgotPasswordSuccess.SetActive(false);
                }
                break;

            case "openLoginOtp":
                {
                    ResetLoginWithOtpScreen();
                    mobileOTOFields = new Dictionary<int, bool>();
                    loginScreenWithOTP.SetActive(true);
                    loginScreenWithPassword.SetActive(false);
                }
                break;

            case "openCreatePassword":
                {
                    createPasswordScreen.SetActive(true);
                    registrationVerifyScreen.SetActive(false);
                }
                break;

            case "openForgetPasswordOtp":
                {
                    mobileOTOFields = new Dictionary<int, bool>();
                    forgotPasswordVerify.SetActive(true);
                    forgotPassword.SetActive(false);
                }
                break;

            case "openCreateNewPassword":
                {
                    createNewPassword.SetActive(true);
                    forgotPasswordVerify.SetActive(false);
                }
                break;

            case "openForgetPasswordsuccess":
                {
                    forgotPasswordSuccess.SetActive(true);
                    forgotPasswordVerify.SetActive(false);
                }
                break;

            case "openRegistration":
                {
                    ResetRegistrationScreen();
                    forgotPassword.SetActive(false);
                    loginScreenWithPassword.SetActive(false);
                    loginScreenWithOTP.SetActive(false);
                    registrationScreen.SetActive(true);
                }
                break;

            case "forgotpwd":
                {
                    ResetForgotPwdScreen();
                    forgotPasswordVerify.SetActive(false);
                    createNewPassword.SetActive(false);
                    loginScreenWithOTP.SetActive(false);
                    loginScreenWithPassword.SetActive(false);
                    registrationScreen.SetActive(false);
                    forgotPassword.SetActive(true);
                }
                break;

            case "closeForgotPwd":
                {
                    //resetPassword.SetActive(false);
                    forgotPassword.SetActive(false);
                    ResetForgotPwdScreen();
                    //loginScreen.SetActive(true);
                    registrationScreen.SetActive(false);
                    //signUpScreen.SetActive(false);
                }
                break;

            case "closeLogin":
                {
                    resendEmailBtnLoginScreen.gameObject.SetActive(false);
                    forgotPassword.SetActive(false);
                    ResetLoginScreen();
                    //loginScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    //signUpScreen.SetActive(true);
                    //resetPassword.SetActive(false);
                }
                break;

            case "closeRegistration":
                {
                    resendEmailBtnLoginScreen.gameObject.SetActive(false);
                    forgotPassword.SetActive(false);
                    ResetRegistrationScreen();
                    //loginScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    //signUpScreen.SetActive(true);
                    //resetPassword.SetActive(false);
                }
                break;

            case "closeResetPassword":
                {
                    forgotPassword.SetActive(false);
                    ResetResetPwdScreen();
                    //loginScreen.SetActive(true);
                    registrationScreen.SetActive(false);
                    //signUpScreen.SetActive(false);
                    //resetPassword.SetActive(false);
                }
                break;

            case "registrationSuccessGoToLogin":
                {
                    string requestData = "{\"userEmail\":\"" + registrationEmailSuccess + "\"," +
                             "\"userPassword\":\"" + registrationPasswordSuccess + "\"," +
                             "\"registrationType\":\"custom\"}";
                    PlayerPrefs.SetString("Password", registrationPasswordSuccess);
                    MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                    WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
                    regSuccessDoneBtn.interactable = false;
                }
                break;

            case "resendVerificationEmail":
                {
                    string requestData = "{\"userId\":\"" + registrationUserIdSuccess + "\"}";
                    MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                    WebServices.instance.SendRequest(RequestType.SendRegVerificationEmail, requestData, true, OnServerResponseFound);

                    //if (loginScreen.activeInHierarchy)
                    //{
                    //    resendEmailBtnLoginScreen.interactable = false;
                    //}
                    //else if (registrationSuccess.activeInHierarchy)
                    //{
                    //    resendEmailBtnLoginScreen.interactable = false;
                    //}
                }
                break;

            case "closeRegistrationSuccess":
                {
                    //registrationSuccess.SetActive(false);
                    registrationScreen.SetActive(false);

                    registrationEmailSuccess = string.Empty;
                    registrationPasswordSuccess = string.Empty;
                    PlayerPrefs.DeleteKey("Password");
                    isLoginAfterRegistration = false;
                    regSuccessDoneBtn.interactable = true;
                    resendEmailBtnLoginScreen.gameObject.SetActive(false);

                    //loginScreen.SetActive(true);
                }
                break;

            default:
                Debug.LogError("Unhandled eventName found = " + eventName);
                break;
        }
    }

    public void LoginAsGuest()
    {
        if (deviceId.Length > 0)
        {
            string requestData = "{\"deviceId\":\"guest_" + deviceId + "\"," +
                                 "\"registrationType\":\"guest\"}";
            Debug.Log("guestdata1 requestdata=" + requestData);
            //statusText.text = requestData;

            MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
            Debug.Log("guestdata2");
            WebServices.instance.SendRequest(RequestType.GuestLogin, requestData, true, OnServerResponseFound);
            Debug.Log("guestdata3");
        }
    }

    public void LoginWithSocialID(string email, string authId, string registrationType)
    {
        this.registrationType = registrationType;
        //string requestData = "{\"userName\":\"" + userName + "\"," +
        //                   "\"userPassword\":\"" + "" + "\"," +
        //                   "\"registrationType\":\"" + registrationType + "\"," +
        //                   "\"socialId\":\"" + socialId + "\"}";

        string requestData = "{\"deviceId\":\"" + deviceId + "\"," +
                           "\"registrationType\":\"" + registrationType + "\"," +
                           "\"emailId\":\"" + email + "\"," +
                           "\"authId\":\"" + authId + "\"}";
        Debug.Log("requestData=" + requestData);
        //statusText.text = requestData;

        MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
        if (registrationType == "google")
        {
            Debug.Log("LoginWithSocialID: google if");
            StartCoroutine(GoogleLoginPostReq(RequestType.Login, email, authId, registrationType, OnServerResponseFound));
        }
        else
        {
            WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
        }
    }

    IEnumerator GoogleLoginPostReq(RequestType requestCode, string email, string authId, string registrationType, System.Action<RequestType, string, bool, string> callbackOnFinish)
    {
        WWWForm form = new WWWForm();
        form.AddField("deviceId", deviceId);
        form.AddField("registrationType", registrationType);
        form.AddField("emailId", email);
        form.AddField("authId", authId);
        Debug.Log("Request url=" + GameConstants.GAME_URLS[(int)requestCode]);
        using (UnityWebRequest www = UnityWebRequest.Post(GameConstants.GAME_URLS[(int)requestCode], form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                callbackOnFinish(requestCode, "", true, www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log("RESULT!" + www.downloadHandler.text);
                callbackOnFinish(requestCode, www.downloadHandler.text, false, "");
            }
        }
    }

    public void LoginWithGoogle()
    {
        //GoogleManager.instance.SignInWithGoogle();
    }

    public void LoginWithFacebook()
    {
        //FacebookLogin.instance.FBlogin();
        //FacebookManager.instance.SignInWithFB();
    }

    public void GetVerificationCodeOnEmail()
    {
        if (forgotEmailInputField.text.Length > 0)
        {
            getVerificationCodeBtn.interactable = false;
            forgotEmailInputField.interactable = false;
            forgotpwWrongEmail.SetActive(false);
            FetchOTPOnEmail();
        }
        else
        {
            forgotpwWrongEmail.GetComponent<Text>().text = "Email id cannot be empty";
            forgotpwWrongEmail.SetActive(true);
        }
    }

    void FetchOTPOnEmail()
    {
        string requestData = "{\"email\":\"" + forgotEmailInputField.text + "\"}";

        MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
        WebServices.instance.SendRequest(RequestType.ForgotPassword, requestData, true, OnServerResponseFound);
    }

    private void ResetLoginScreen()
    {
        loginMobileNumber.text = "";
        loginPassword.text = "";
    }
    
    private void ResetLoginWithOtpScreen()
    {
        loginWithOTPMobileNumber.text = "";
        loginPassword.text = "";
        loginWithOTPFields[0].text = loginWithOTPFields[1].text = loginWithOTPFields[2].text = loginWithOTPFields[3].text = loginWithOTPFields[4].text = loginWithOTPFields[5].text = "";
    }

    private void ResetRegistrationScreen()
    {
        timer = 0f;
        registrationMobileNumber.text = "";
        mobilePassFields[0].text = mobilePassFields[1].text = mobilePassFields[2].text = mobilePassFields[3].text = mobilePassFields[4].text = mobilePassFields[5].text = "";
        createPassword.text = "";
    }

    private void ResetResetPwdScreen()
    {
        newPassword.text = "";
    }

    private void ResetForgotPwdScreen()
    {
        timer = 0;
        forgetMobileNumber.text = "";
        forgetOTPFields[0].text = forgetOTPFields[1].text = forgetOTPFields[2].text = forgetOTPFields[3].text = forgetOTPFields[4].text = forgetOTPFields[5].text = "";
    }

    public Sprite EyeOff, EyeOn;
    public Image CreatePasswordEye, LoginPasswordEye, NewPasswordEye;//, NewPasswordEye;

    public void RegisterEyeClick()
    {
        if (this.createPassword != null)
        {
            if (this.createPassword.contentType == InputField.ContentType.Password)
            {
                CreatePasswordEye.sprite = EyeOn;
                this.createPassword.contentType = InputField.ContentType.Standard;
            }
            else
            {
                CreatePasswordEye.sprite = EyeOff;
                this.createPassword.contentType = InputField.ContentType.Password;
            }
            CreatePasswordEye.SetNativeSize();
            this.createPassword.ForceLabelUpdate();
        }
    }

    public void LoginEyeClick()
    {
        if (this.loginPassword != null)
        {
            if (this.loginPassword.contentType == InputField.ContentType.Password)
            {
                LoginPasswordEye.sprite = EyeOn;
                this.loginPassword.contentType = InputField.ContentType.Standard;
            }
            else
            {
                LoginPasswordEye.sprite = EyeOff;
                this.loginPassword.contentType = InputField.ContentType.Password;
            }
            LoginPasswordEye.SetNativeSize();
            this.loginPassword.ForceLabelUpdate();
        }
    }

    public void NewPwdEyeClick(InputField passInputField)
    {
        if (passInputField != null)
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject.name);
            NewPasswordEye = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
            if (passInputField.contentType == InputField.ContentType.Password)
            {
                NewPasswordEye.sprite = EyeOn;
                passInputField.contentType = InputField.ContentType.Standard;
            }
            else
            {
                NewPasswordEye.sprite = EyeOff;
                passInputField.contentType = InputField.ContentType.Password;
            }
            NewPasswordEye.SetNativeSize();
            passInputField.ForceLabelUpdate();
        }
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.Loading);
        Debug.Log("Reg " + errorMessage + " " + requestType);
        Debug.Log("guestdata2.1 requestType=" + requestType);
        Debug.Log("guestdata2.2 serverResponse=" + serverResponse);
        Debug.Log("guestdata2.3 requestType=" + isShowErrorMessage);
        Debug.Log("guestdata2.4 errorMessage=" + errorMessage);
        Debug.Log("guestdata2.5 errorMessage.Length=" + errorMessage.Length);
        if (errorMessage.Length > 0 && requestType != RequestType.FantasySignUp)
        {
            Debug.Log("guestdata2.6");
            if (isShowErrorMessage)
            {
                Debug.Log("guestdata2.7");
                MainDashboardScreen.instance.ShowMessage(errorMessage);
                Debug.Log("guestdata2.8");
            }
            Debug.Log("guestdata2.9");
            return;
        }

        if (requestType == RequestType.Registration)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            Debug.Log("Response => Registration: " + JsonMapper.ToJson(data));
            Debug.Log("Response => Registration: " + data["status"].ToString());

            if (data["statusCode"].ToString() == "200")
            {
                //registrationEmailSuccess = registrationEmail.text;
                //registrationPasswordSuccess = registrationPassword.text;
                //registrationUserIdSuccess = data["result"]["userId"].ToString();
                //isLoginAfterRegistration = true;
                //SignUpInFantasyGame(data["result"]["userName"].ToString(), registrationEmail.text, registrationPassword.text);

                //ResetLoginScreen();
                //ResetRegistrationScreen();

                // StartCoroutine(MsgForVideo("Registered Successfully", 1.5f));

                //loginScreen.SetActive(false);

                //PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);
                //PlayerManager.instance.SetPlayerGameData(playerData);

                OnClickOnButton("openSignUpVerification");
                currentTimerText = registerTimerText;
                timer = 20f;
            }
            else
            {
                //Debug.Log("Uesr already exist");
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
                //StartCoroutine(MsgForVideo("User already exist", 1.5f));
                //ResetLoginScreen();
                //ResetRegistrationScreen();
                //registrationScreen.SetActive(false);
                //loginScreen.SetActive(true);
                MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.VerifyOtp)
        {
            Debug.Log("Response => VerifyOtp: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["statusCode"].ToString() == "200")
            {
                Debug.Log("Response => type: " + data["type"].ToString());

                PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);
                PlayerManager.instance.SetPlayerGameData(playerData);
                MainDashboardScreen.instance.UpdateUserDetails(data);

                if (data["type"].ToString() == "2")
                {
                    MainDashboardScreen.instance.bottomMenu.SetActive(true);
                    MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.Registration);
                    MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
                    //GlobalGameManager.token = (string) data["data"]["token"];
                }
                else
                {
                    OnClickOnButton("openCreatePassword");
                }
            }
            else
            {
                MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.ResendOtp)
        {
            Debug.Log("Response => ResendOtp: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["statusCode"].ToString() == "200")
            {
                timer = 20f;

                if (data["type"].ToString() == "1")
                    registertOtpCountText.text = "OTP send only <color=black>" + data["data"]["resendCount"].ToString() + " Times</color>";
                else if (data["type"].ToString() == "3")
                    forgetOtpCountText.text = "OTP send only <color=black>" + data["data"]["resendCount"].ToString() + " Times</color>";

                if (data["type"].ToString() == "2")
                    MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
            }
            else
            {
                MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.ResetPassword)
        {
            Debug.Log("Response => ResetPassword: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["statusCode"].ToString() == "200")
            {
                //OnClickOnButton("openLogin");
             //    MainDashboardScreen.instance.bottomMenu.SetActive(true);
             //     MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.Registration);
                if (data["type"].ToString() == "2")
                {
                    createNewPassword.SetActive(false);
                    forgotPasswordSuccess.SetActive(true);
                } 
                 //     MainDashboardScreen.instance.ShowMessage("Password changed");
            }
            else
            {
                MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.Login)
        {
            Debug.Log("Response => Login: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["statusCode"].ToString() == "200")
            {
                MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
                if (data["type"].ToString() == "1")
                { }
                else
                {
                    PlayerPrefs.DeleteKey("GuestLogin");
                    PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);
                    PlayerManager.instance.SetPlayerGameData(playerData);
                    MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.Registration);
                    MainDashboardScreen.instance.bottomMenu.SetActive(true);
                    MainDashboardScreen.instance.UpdateUserDetails(data);
                }
            }
            else
            {
                MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.GuestLogin)
        {
            Debug.Log("Response => GuestLogin: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);

                PlayerManager.instance.SetPlayerGameData(playerData);
                MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.Loading);
                //MainMenuController.instance.SwitchToMainMenu();
                GlobalGameManager.instance.LoadScene(Scenes.MainDashboard);
                PlayerPrefs.SetString("GuestLogin", "Yes");
            }
            else if (data["success"].ToString() == "0")
            {
                wrongPasswordText.gameObject.SetActive(true);
                StartCoroutine(MsgForVideo(data["message"].ToString(), 1.5f));
                //loginScreen.SetActive(false);
            }
            else
            {
                wrongPasswordText.gameObject.SetActive(true);
                StartCoroutine(MsgForVideo("Incorrect password or username does not exist", 1.5f));
                //loginScreen.SetActive(false);
            }
        }
        else if (requestType == RequestType.ForgotPassword)
        {
            Debug.Log("Response => ForgotPassword: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["statusCode"].ToString() == "200")
            {
                currentTimerText = forgetTimerText;
                timer = 20f;
                forgetMobileInfoText.text = "OTP send to +91-" + forgetMobileNumber.text;
                OnClickOnButton("openForgetPasswordOtp");
            }
            else
            {
                //forgotPassword.SetActive(false);
                //MainMenuController.instance.ShowMessage(data["response"].ToString());
                ////ResetLoginScreen();
                //OnClickOnButton("forgotpwd");
                /*getVerificationCodeBtn.interactable = true;
                forgotEmailInputField.interactable = true;
                forgotpwWrongEmail.GetComponent<Text>().text = "Account has been not linked yet"; //"Email address has yet to be linked";
                forgotpwWrongEmail.SetActive(true);*/
                MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.VerifyOtpForForgotPassword)
        {
            Debug.Log("Response => VerifyOtpForForgotPassword: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["statusCode"].ToString() == "200")
            {
                MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
                OnClickOnButton("openCreateNewPassword");
            }
            else
            {
                MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.changePassword)
        {
            OnClickOnButton("openLogin");
        }
        else if (requestType == RequestType.SendRegVerificationEmail)
        {
            Debug.Log("email sended");
            JsonData data = JsonMapper.ToObject(serverResponse);
            StartCoroutine(MsgForVideo(data["message"].ToString(), 1.5f));
            //if (loginScreen.activeInHierarchy)
            //{
            //    resendEmailBtnLoginScreen.interactable = true;
            //}
            //else if (registrationSuccess.activeInHierarchy)
            //{
            //    resendEmailBtnLoginScreen.interactable = true;
            //}
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhandled server requestType found  " + requestType);
#endif
        }

    }


    public void OnClickOnBack()
    {
        if (PlayerManager.instance.IsLogedIn())
        {
            //MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
        }
        else
        {
            /*MainDashboardScreen.instance.ShowMessage("Do you really want to quit?", () => {
                GlobalGameManager.instance.CloseApplication();
            }, () => { });*/
        }
    }

    IEnumerator MsgForVideo(string msg, float delay)
    {
        popUpText.gameObject.SetActive(true);
        popUpText.text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.gameObject.SetActive(false);
        wrongPasswordText.gameObject.SetActive(false);
    }

    void SignUpInFantasyGame(string userName, string email, string password)
    {
        string requestData = "{\"Email\":\"" + email + "\"," +
                         "\"Username\":\"" + userName + "\"," +
                          "\"Password\":\"" + password + "\"," +
                          "\"UserTypeID\":\"" + "2" + "\"," +
                          "\"PhoneNumber\":\"" + "" + "\"," +
                          "\"Source\":\"" + "Direct" + "\"," +
                          "\"SourceGUID\":\"" + "" + "\"," +
                          "\"DeviceType\":\"" + "Native" + "\"," +
                          "\"IPAddress\":\"" + "" + "\"}";

        WebServices.instance.SendRequest(RequestType.FantasySignUp, requestData, true, OnServerResponseFound);
    }

    public void DisplayStatusText(string msg)
    {
        statusText.gameObject.SetActive(true);
        statusText.text = msg;
        StartCoroutine(GlobalGameManager.instance.RunAfterDelay(2f, () => {
            statusText.gameObject.SetActive(false);
            statusText.text = string.Empty;
        }));
    }

    
    public void VerifyMobileOTP(int otpNum)
    {
        if (!string.IsNullOrEmpty(mobilePassFields[otpNum].text))
        {
            mobileOTOFields.Add(otpNum, true);
            if (otpNum < 5)
                mobilePassFields[otpNum + 1].Select();
        }
        Debug.Log("ZZZZ " + mobileOTOFields.Count);
        if (mobileOTOFields.Count == 6)
        {
            otp = mobilePassFields[0].text + mobilePassFields[1].text + mobilePassFields[2].text + mobilePassFields[3].text + mobilePassFields[4].text + mobilePassFields[5].text;
            //OnClickOnButton("verifyotp");
        }
        if (string.IsNullOrEmpty(mobilePassFields[otpNum].text) && mobileOTOFields.ContainsKey(otpNum) && mobileOTOFields[otpNum] == true)
        {
            if (otpNum > 0)
                mobilePassFields[otpNum - 1].Select();
            mobileOTOFields.Remove(otpNum);
        }
    }

    public void VerifyMobileOTPForLogin(int otpNum)
    {
        if (!string.IsNullOrEmpty(loginWithOTPFields[otpNum].text))
        {
            mobileOTOFields.Add(otpNum, true);
            if (otpNum < 5)
                loginWithOTPFields[otpNum + 1].Select();
        }
        Debug.Log("ZZZZ " + mobileOTOFields.Count);
        if (mobileOTOFields.Count == 6)
        {
            otp = loginWithOTPFields[0].text + loginWithOTPFields[1].text + loginWithOTPFields[2].text + loginWithOTPFields[3].text + loginWithOTPFields[4].text + loginWithOTPFields[5].text;
            //OnClickOnButton("verifyotp");
        }
        if (string.IsNullOrEmpty(loginWithOTPFields[otpNum].text) && mobileOTOFields.ContainsKey(otpNum) && mobileOTOFields[otpNum] == true)
        {
            if (otpNum > 0)
                loginWithOTPFields[otpNum - 1].Select();
            mobileOTOFields.Remove(otpNum);
        }
    }

    public void VerifyMobileOTPForForget(int otpNum)
    {
        if (!string.IsNullOrEmpty(forgetOTPFields[otpNum].text))
        {
            mobileOTOFields.Add(otpNum, true);
            if (otpNum < 5)
                forgetOTPFields[otpNum + 1].Select();
        }
        Debug.Log("ZZZZ " + mobileOTOFields.Count);
        if (mobileOTOFields.Count == 6)
        {
            otp = forgetOTPFields[0].text + forgetOTPFields[1].text + forgetOTPFields[2].text + forgetOTPFields[3].text + forgetOTPFields[4].text + forgetOTPFields[5].text;
            //OnClickOnButton("verifyotp");
        }
        if (string.IsNullOrEmpty(forgetOTPFields[otpNum].text) && mobileOTOFields.ContainsKey(otpNum) && mobileOTOFields[otpNum] == true)
        {
            if (otpNum > 0)
                forgetOTPFields[otpNum - 1].Select();
            mobileOTOFields.Remove(otpNum);
        }
    }

    public void ShowPrefixInInputField(InputField inputField)
    {
        //Debug.Log("AA " + inputField.text.Length);
        if (inputField.text.Length > 0)
        {
            inputField.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(55f, 0f);
            inputField.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            inputField.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(12.5f, 0f);
            inputField.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " was selected");
    }
}