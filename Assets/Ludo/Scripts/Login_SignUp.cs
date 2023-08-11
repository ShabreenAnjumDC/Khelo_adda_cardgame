using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class Login_SignUp : MonoBehaviour
{
    public static Login_SignUp instance;

    [Header("-----Parents-----")]
    public GameObject signUpParent;
    public GameObject otpParent;
    public GameObject loginParent;
    public GameObject mobileNoFPWParent;
    public GameObject mobileNoFPWFinalParent;

    [Header("-----Sign up input fields-----")]
    public InputField mobileNoInput;
    public InputField emailInput;
    public InputField passwordInput;

    [Header("-----Login input fields-----")]
    public InputField mobileNoLoginInput;
    public InputField passwordLoginInput;

    [Header("-----Error text-----")]
    public TMP_Text errorSignUpText;
    public TMP_Text errorLoginText;
    public TMP_Text errorOtpText;
    public TMP_Text errorMobileNoForgotPWText;
    public TMP_Text errorForgotPWFinalText;

    [Header("-----Temp Value-----")]
    public string tempMobileReg;
    public string tempEmailReg;
    public string tempPasswordReg;
    public string tempMobileLogin;
    public string tempEmailLogin;
    public string tempTokenLogin;
    public string tempIdLogin;

    [Space(10)]
    [Header("-----Forgot PW Mobile No. input fields-----")]
    public InputField mobileNoForgotPWInput;

    [Header("-----Forgot PW Final input fields-----")]
    public InputField mobileNoForgotPWFinalInput;
    public InputField newPWForgotPWFinalInput;

    [Space(10)]
    [Header("-----OTP input fields-----")]
    public InputField otpInput1;
    public InputField otpInput2;
    public InputField otpInput3;
    public InputField otpInput4;
    public InputField otpInput5;
    public InputField otpInput6;
    public InputField[] otpInputsArr;

    [Header("-----OTP waiting seconds -----")]
    public Text otpSecondsText;

    [Header("-----Show/Hide Password Sprite-----")]
    public Sprite showPasswordSprite;
    public Sprite hidePasswordSprite;

    [Header("-----OTP Confirm-----")]
    public Transform confirmOTPBtn;

    [Header("-----OTP Confirm Sprite-----")]
    public Sprite confirmOnSprite;
    public Sprite confirmOffSprite;

    [Header("-----OTP Confirm Text Color-----")]
    public Color confirmOnTextColor;
    public Color confirmOffTextColor;

    [Header("-----OTP Resend Button-----")]
    public Transform resendOnTextParent;

    [Header("-----OTP/Forgot PW OTP-----")]
    public Text titleOTPText;
    public Transform title2OTP;
    public Text subTitleOTPText;
    public bool isForgotPassword;

    Coroutine coroutineRsendOTP;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //coroutineRsendOTP = StartCoroutine(OtpExipreCo()); //to test otp screen

        confirmOTPBtn.GetComponent<Button>().onClick.AddListener(OtpVerifyProcess); //onclick_verify_otp
    }

    void Update()
    {
        OnEscape();
    }

    void OnEscape()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (otpParent.activeSelf)
            {
                OnClickOnButton("close_otp");
            }
        }
    }

    public void OnClickOnButton(string buttonName)
    {
        L_MainMenuController.instance.PlayButtonSound();

        switch (buttonName)
        {
            case "onclick_send_otp":
                SendOtpProcess();
                break;

            case "show_login_with_mobile":
                ShowLoginScreen();
                break;

            case "show_sign_up":
                ShowSignUpScreen();
                break;

            case "close_otp":
                StopCoroutine(coroutineRsendOTP);
                ResetResend();
                if (isForgotPassword)
                {
                    isForgotPassword = false;
                    ShowMobileNoForgotPWScreen();
                }
                else
                    ShowSignUpScreen();
                break;

            case "terms_n_condition":
                L_MainMenuController.instance.ShowScreen(MainMenuScreens.TermsNCondition);
                break;

            case "onclick_signUp":
                //ShowOtpScreen();
                SignUpProcess();
                break;

            case "onclick_verify_otp":
                //MainMenuController.instance.ShowScreen(MainMenuScreens.ReferralConfirm);
                OtpVerifyProcess();
                break;

            case "onclick_resend_otp":
                if (isForgotPassword)
                    OtpResendProcess();
                else
                    OtpResendProcessForSignUp();
                break;

            case "onclick_login":
                if (L_MainMenuController.instance.IsScreenActive(MainMenuScreens.TermsNCondition))
                {
                    L_MainMenuController.instance.DestroyScreen(MainMenuScreens.TermsNCondition);
                }
                LoginProcess();
                break;

            case "forget_password":
                ShowMobileNoForgotPWScreen();
                break;

            case "forget_password_final":
                ForgotPWFinalProcess();
                break;

            default:
                break;
        }
    }

    public void OnMobileNoInputEndEdit()
    {
        if (mobileNoInput.text.Length == 10)
        {
            Debug.Log("CALL API");
            StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.CheckUserExist] + "/" + mobileNoInput.text, (serverResponse, errorBool, error) =>
            {
                if (errorBool)
                {
                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Error in Check User Exist"));
                    Debug.Log("Error in Check User Exist 1: " + error);
                }
                else
                {
                    Debug.Log("Check User Exist response: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    //{"code":200,"data":{"message":"User available"},"success":true}
                    //{ "code":500,"errorMessage":{ "message":"User exist"},"error":{ },"data":null,"success":false}

                    IDictionary iData1 = data as IDictionary;

                    if (iData1.Contains("code"))
                    {
                        if (data["code"].ToString() == "200")
                        {
                            Debug.Log("check user: User available");
                        }
                        else if (iData1.Contains("errorMessage"))
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, data["errorMessage"].ToString(), "red", 2f));
                        }
                        else
                        {
                            Debug.Log("Error in Check User Exist 3");
                        }
                    }
                    else
                    {
                        Debug.Log("Success in Check User Exist");
                    }


                    //if ((bool)data["success"] == false)
                    //{
                    //    IDictionary iData = data as IDictionary;
                    //    if (iData.Contains("errorMessage"))
                    //    {
                    //        IDictionary iDataM = data["errorMessage"] as IDictionary;
                    //        if (iDataM.Contains("message"))
                    //        {
                    //            StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, data["errorMessage"]["message"].ToString(), "red", 2f));
                    //        }
                    //        else
                    //            Debug.Log("Error in Check User Exist 2");
                    //    }
                    //    else
                    //        Debug.Log("Error in Check User Exist 3");
                    //}
                    //else
                    //    Debug.Log("Success in Check User Exist");
                }
            }));
        }
    }



    void SignUpProcess()
    {
        string error = string.Empty;

        if (string.IsNullOrEmpty(mobileNoInput.text.ToString()))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Please enter mobile no."));
        else if (mobileNoInput.text.ToString().Length != 10)
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Please enter 10 digit mobile no."));
        else if (string.IsNullOrEmpty(emailInput.text.ToString()))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Please enter email"));
        else if (!L_Utility.IsValidUserEmail(emailInput.text, out error))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, error));
        else if (string.IsNullOrEmpty(passwordInput.text.ToString()))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Please enter password"));
        else if (passwordInput.text.ToString().Length < 8)
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Please enter at least 8 digit password"));
        else if (!CheckPassword(passwordInput.text, 8))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Please enter Password with upper-case, lower-case, numbers & special characters", "red", 2f));
        else
        {
            string jsonData = "{\"mobile\":\"" + mobileNoInput.text + "\", \"email\":\"" + emailInput.text + "\", \"password\":\"" + passwordInput.text + "\"}";
            Debug.Log("Registration json: " + jsonData);
            StartCoroutine(L_WebServices.instance.POSTRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.RegisterGame], jsonData, (serverResponse, errorBool, error) =>
            {
                if (errorBool)
                {
                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Error in Registration"));
                    Debug.Log("Error in Registration 1: " + error);
                }
                else
                {
                    Debug.Log("Registration response: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    IDictionary iData1 = data as IDictionary;

                    if (iData1.Contains("code"))
                    {
                        if (data["code"].ToString() == "200")
                        {
                            Debug.Log("Registration Success");
                            tempMobileReg = mobileNoInput.text;
                            tempEmailReg = emailInput.text;
                            tempPasswordReg = passwordInput.text;
                            ClearRegistrationInput();
                            if (coroutineRsendOTP != null)
                                StopCoroutine(coroutineRsendOTP);
                            coroutineRsendOTP = StartCoroutine(OtpExipreCo());
                            ShowOtpScreen();
                        }
                        else if (iData1.Contains("errorMessage"))
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, data["errorMessage"].ToString(), "red", 2f));
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Error in Registration!", "red", 2f));
                            Debug.Log("Error in Registration 2");
                        }
                    }
                    else
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorSignUpText, "Error in Registration!!", "red", 2f));
                        Debug.Log("Error in Registration 3");
                    }
                }
            }));
        }
    }



    void OtpVerifyProcess()
    {
        // mobile, email, otp
        //else if (string.IsNullOrEmpty(tempEmailReg))
        //    StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Can't find your Email"));
        if (string.IsNullOrEmpty(tempMobileReg))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Can't find your entered Mobile no."));
        else if (string.IsNullOrEmpty(tempEmailReg))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Can't find your entered Email"));
        else if (string.IsNullOrEmpty(tempPasswordReg))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Can't find your entered Password"));
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
                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Enter otp")); // + otpErrorStr
            }
            else
            {
                string otpStr = string.Empty;
                for (int i = 0; i < otpInputsArr.Length; i++)
                {
                    otpStr += otpInputsArr[i].text;
                }
                string jsonData = "{\"mobile\":\"" + tempMobileReg + "\", \"email\":\"" + tempEmailReg + "\", \"password\":\"" + tempPasswordReg + "\", \"otp\":\"" + otpStr + "\"}";
                Debug.Log("otp verify json: " + jsonData);
                StartCoroutine(L_WebServices.instance.POSTRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.VerifyRegOtp], jsonData, (serverResponse, errorBool, error) =>
                {
                    if (errorBool)
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in OTP verify"));
                        Debug.Log("Error in otp verify 1: " + error);
                    }
                    else
                    {
                        //string serverResponse = "{\"code\":500,\"errorMessage\":\"Cannot read property 'phoneOTP' of null\",\"error\":{},\"data\":null,\"success\":false}";
                        //string serverResponse = "{\"code\":200,\"data\":{\"message\":\"OTP verified successfully\"},\"success\":true}";
                        Debug.Log("otp verify response: " + serverResponse);
                        JsonData data = JsonMapper.ToObject(serverResponse);

                        IDictionary iData1 = data as IDictionary;

                        if (iData1.Contains("code"))
                        {
                            if (data["code"].ToString() == "200")
                            {
                                Debug.Log("otp verify Success");

                                mobileNoInput.text = tempMobileReg;
                                StopCoroutine(coroutineRsendOTP);
                                ResetResend();


                                L_PlayerGameDetails dataToAssign = new L_PlayerGameDetails();
                                dataToAssign.userId = data["data"]["user"]["id"].ToString();
                                dataToAssign.mobileNo = data["data"]["user"]["mobile"].ToString();
                                dataToAssign.emailId = data["data"]["user"]["email"].ToString();
                                dataToAssign.name = data["data"]["user"]["name"].ToString();
                                if (data["data"]["user"]["username"] != null)
                                    dataToAssign.userName = data["data"]["user"]["username"].ToString();
                                dataToAssign.token = data["data"]["token"].ToString();
                                L_GlobalGameManager.playerToken = dataToAssign.token;
                                L_PlayerManager.instance.SetPlayerGameData(dataToAssign);

                                L_MainMenuController.instance.DestroyScreen(MainMenuScreens.Login_SignUp_Otp);
                                L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                            }
                            else if (iData1.Contains("errorMessage"))
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, data["errorMessage"].ToString(), "red", 2f));
                            }
                            else
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in OTP verify!", "red", 2f));
                                Debug.Log("Error in otp verify 2");
                            }
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in OTP verify!!", "red", 2f));
                            Debug.Log("Error in otp verify 3");
                        }
                    }
                }));
            }
        }
    }


    void OtpResendProcess()
    {
        //if (otpSecondsText.text.Equals("Resend"))
        if (resendOnTextParent.gameObject.activeInHierarchy)
        {
            if (string.IsNullOrEmpty(tempMobileReg))
                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Can't find your Mobile no."));
            else
            {
                if (coroutineRsendOTP != null)
                    StopCoroutine(coroutineRsendOTP);
                coroutineRsendOTP = StartCoroutine(OtpExipreCo());

                Debug.Log("send otp mobile no: " + tempMobileReg);
                StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.SendRegOtp] + "/" + tempMobileReg, (serverResponse, errorBool, error) =>
                {
                    if (errorBool)
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in Send OTP"));
                        Debug.Log("Error in otp send 1: " + error);
                    }
                    else
                    {
                        Debug.Log("otp resend response: " + serverResponse);
                        JsonData data = JsonMapper.ToObject(serverResponse);
                        //{"status":true,"message":"OTP send successfully"}

                        IDictionary iData1 = data as IDictionary;

                        if (iData1.Contains("code"))
                        {
                            if (data["code"].ToString() == "200")
                            {
                                Debug.Log("otp send Success");
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "OTP send successfully", "green"));
                                if (coroutineRsendOTP != null)
                                    StopCoroutine(coroutineRsendOTP);
                                coroutineRsendOTP = StartCoroutine(OtpExipreCo());
                            }
                            else if (iData1.Contains("errorMessage"))
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, data["errorMessage"].ToString(), "red", 2f));
                            }
                            else
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in sending OTP!", "red", 2f));
                                Debug.Log("Error in otp send 2");
                            }
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in Send OTP!!"));
                            Debug.Log("Error in otp send 3");
                        }
                    }
                }));
            }
        }
    }



    void OtpResendProcessForSignUp()
    {
        //if (otpSecondsText.text.Equals("Resend"))
        if (resendOnTextParent.gameObject.activeInHierarchy)
        {
            if (string.IsNullOrEmpty(tempMobileReg))
                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Can't find your Mobile no."));
            else
            {
                if (coroutineRsendOTP != null)
                    StopCoroutine(coroutineRsendOTP);
                coroutineRsendOTP = StartCoroutine(OtpExipreCo());

                Debug.Log("send otp mobile no: " + tempMobileReg);

                StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.ResendOtp] + tempMobileReg, (serverResponse, errorBool, error) =>
                {
                    if (errorBool)
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in Send OTP"));
                        Debug.Log("Error in otp send 1: " + error);
                    }
                    else
                    {
                        Debug.Log("otp resend response: " + serverResponse);
                        JsonData data = JsonMapper.ToObject(serverResponse);

                        IDictionary iData1 = data as IDictionary;

                        if (iData1.Contains("code"))
                        {
                            if (data["code"].ToString() == "200")
                            {
                                Debug.Log("otp send Success");
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "OTP send successfully", "green"));
                                if (coroutineRsendOTP != null)
                                    StopCoroutine(coroutineRsendOTP);
                                coroutineRsendOTP = StartCoroutine(OtpExipreCo());
                            }
                            else if (iData1.Contains("errorMessage"))
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, data["errorMessage"].ToString(), "red", 2f));
                            }
                            else
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in sending OTP!", "red", 2f));
                                Debug.Log("Error in otp send 2");
                            }
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in Send OTP!!"));
                            Debug.Log("Error in otp send 3");
                        }
                    }
                }));
            }
        }
    }


    void LoginProcess()
    {
        string error = string.Empty;

        if (string.IsNullOrEmpty(mobileNoLoginInput.text.ToString()))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, "Please enter mobile no."));
        else if (mobileNoLoginInput.text.ToString().Length != 10)
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, "Please enter 10 digit mobile no."));
        else if (string.IsNullOrEmpty(passwordLoginInput.text.ToString()))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, "Please enter password"));
        else if (passwordLoginInput.text.ToString().Length < 8)
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, "Please enter at least 8 digit password"));
        else
        {
            string jsonData = "{\"mobile\":\"" + mobileNoLoginInput.text + "\", \"password\":\"" + passwordLoginInput.text + "\"}";
            Debug.Log("Login json: " + jsonData);
            StartCoroutine(L_WebServices.instance.POSTRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.Login], jsonData, (serverResponse, errorBool, error) =>
            {
                if (errorBool)
                {
                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, "Error in Login"));
                    Debug.Log("Error in Login 1: " + error);
                }
                else
                {
                    //string serverResponse = "{\"code\":500,\"errorMessage\":\"Incorrect Email Id / Password\",\"error\":{},\"data\":null,\"success\":false}";
                    //string serverResponse = "{\"code\":200,\"data\":{\"user\":{\"id\":4,\"name\":\"User\",\"mobile\":\"9429494201\",\"kyc\":\"No\",\"email\":\"v3.ab@yopmail.com\",\"phoneOTP\":\"\",\"emailOTP\":null,\"profilePic\":null,\"isAdmin\":false,\"verifyToken\":\"uXzTEv1yZl1PB\",\"isVerified\":false,\"isMobileVerified\":true,\"isEmailVerified\":false,\"createdAt\":\"2023 - 02 - 04T06: 01:57.000Z\",\"updatedAt\":\"2023 - 02 - 04T06: 30:36.000Z\"},\"token\":\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyIjp7InVzZXJJZCI6NCwiZW1haWwiOiJ2My5hYkB5b3BtYWlsLmNvbSIsImNyZWF0ZWRBdCI6IjIwMjMtMDItMDRUMDk6MTE6MzguNjMwWiJ9LCJpYXQiOjE2NzU1MDE4OTh9.sinkAIFbGv4E - NgZXdQymLw6gU9SXSOQbZHAKqjMjUg\"},\"success\":true}";
                    Debug.Log("Login response: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    IDictionary iData1 = data as IDictionary;

                    if (iData1.Contains("code"))
                    {
                        if (data["code"].ToString() == "200")
                        {
                            Debug.Log("Login Success");

                            L_PlayerGameDetails dataToAssign = new L_PlayerGameDetails();
                            dataToAssign.userId = data["data"]["user"]["id"].ToString();
                            dataToAssign.mobileNo = data["data"]["user"]["mobile"].ToString();
                            dataToAssign.emailId = data["data"]["user"]["email"].ToString();
                            dataToAssign.name = data["data"]["user"]["name"].ToString();
                            if (data["data"]["user"]["username"] != null)
                                dataToAssign.userName = data["data"]["user"]["username"].ToString();
                            dataToAssign.token = data["data"]["token"].ToString();
                            L_GlobalGameManager.playerToken = dataToAssign.token;
                            L_PlayerManager.instance.SetPlayerGameData(dataToAssign);

                            ClearLoginInput();
                            L_MainMenuController.instance.DestroyScreen(MainMenuScreens.Login_SignUp_Otp);
                            L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
                        }
                        else if (iData1.Contains("errorMessage"))
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, data["errorMessage"].ToString(), "red", 2f));
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, "Error in Login!"));
                            Debug.Log("Error in Login 2");
                        }
                    }
                    else
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, "Error in Login!!"));
                        Debug.Log("Error in Login 3");
                    }
                }
            }));
        }
    }











    void SendOtpProcess()
    {
        string error = string.Empty;

        if (string.IsNullOrEmpty(mobileNoForgotPWInput.text.ToString()))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorMobileNoForgotPWText, "Please enter mobile no."));
        else if (mobileNoForgotPWInput.text.ToString().Length != 10)
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorMobileNoForgotPWText, "Please enter 10 digit mobile no."));
        else
        {
            StartCoroutine(L_WebServices.instance.GETRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.SendRegOtp] + "/" + mobileNoForgotPWInput.text, (serverResponse, errorBool, error) =>
            {
                if (errorBool)
                {
                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorMobileNoForgotPWText, "Error in sending OTP")); //"Error in Registration"
                    Debug.Log("Error in Sending OTP 1: " + error);
                }
                else
                {
                    Debug.Log("Send OTP response: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    IDictionary iData1 = data as IDictionary;

                    if (iData1.Contains("code"))
                    {
                        if (data["code"].ToString() == "200")
                        {
                            Debug.Log("OTP send Success");
                            tempMobileReg = mobileNoForgotPWInput.text;
                            coroutineRsendOTP = StartCoroutine(OtpExipreCo());
                            mobileNoForgotPWInput.text = "";
                            isForgotPassword = true;
                            SetLableForForgotPwOtp();
                            confirmOTPBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                            confirmOTPBtn.GetComponent<Button>().onClick.AddListener(OtpFPWVerifyProcess);

                            ShowOtpScreen();
                        }
                        else if (iData1.Contains("errorMessage"))
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorMobileNoForgotPWText, data["errorMessage"].ToString(), "red", 2f));
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorMobileNoForgotPWText, "Error in sending OTP!", "red", 2f));
                            Debug.Log("Error in Sending OTP 2");
                        }
                    }
                    else
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorMobileNoForgotPWText, "Error in sending OTP!!", "red", 2f));
                        Debug.Log("Error in Sending OTP 3");
                    }
                }
            }));
        }
    }

    void OtpFPWVerifyProcess()
    {
        // mobile
        if (string.IsNullOrEmpty(tempMobileReg))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Can't find your entered Mobile no."));
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
                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Enter otp")); // + otpErrorStr
            }
            else
            {
                string otpStr = string.Empty;
                for (int i = 0; i < otpInputsArr.Length; i++)
                {
                    otpStr += otpInputsArr[i].text;
                }
                string jsonData = "{\"mobile\":\"" + tempMobileReg + "\", \"otp\":\"" + otpStr + "\"}";
                Debug.Log("otp verify json: " + jsonData);
                StartCoroutine(L_WebServices.instance.POSTRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.VerifyFPWOtp], jsonData, (serverResponse, errorBool, error) =>
                {
                    if (errorBool)
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in OTP verify"));
                        Debug.Log("Error in otp verify 1: " + error);
                    }
                    else
                    {
                        Debug.Log("otp verify response: " + serverResponse);
                        JsonData data = JsonMapper.ToObject(serverResponse);

                        IDictionary iData1 = data as IDictionary;

                        if (iData1.Contains("code"))
                        {
                            if (data["code"].ToString() == "200")
                            {
                                // now show screen with mobile no disable & new password
                                mobileNoForgotPWFinalInput.text = tempMobileReg;
                                ShowMobileNoForgotPWFinalScreen();
                            }
                            else if (iData1.Contains("errorMessage"))
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, data["errorMessage"].ToString(), "red", 2f));
                            }
                            else
                            {
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in OTP verify!", "red", 2f));
                                Debug.Log("Error in Sending OTP 2");
                            }
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorOtpText, "Error in OTP verify!!", "red", 2f));
                            Debug.Log("Error in Sending OTP 3");
                        }
                    }
                }));
            }
        }
    }

    void ForgotPWFinalProcess()
    {
        // mobile
        if (string.IsNullOrEmpty(tempMobileReg))
            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorForgotPWFinalText, "Can't find your entered Mobile no."));
        else
        {
            string jsonData = "{\"mobile\":\"" + tempMobileReg + "\", \"newPassword\":\"" + newPWForgotPWFinalInput.text + "\"}";
            Debug.Log("otp verify json: " + jsonData);
            StartCoroutine(L_WebServices.instance.PUTRequestData(L_GameConstant.GAME_URLS[(int)L_RequestType.ForgotPassword], jsonData, (serverResponse, errorBool, error) =>
            {
                if (errorBool)
                {
                    StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorForgotPWFinalText, "Error in Forget Password Process"));
                    Debug.Log("Error in Forget Password Process 1: " + error);
                }
                else
                {
                    Debug.Log("otp verify response: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    IDictionary iData1 = data as IDictionary;

                    if (iData1.Contains("code"))
                    {
                        if (data["code"].ToString() == "200")
                        {
                            ShowLoginScreen();
                            mobileNoForgotPWFinalInput.text = "";
                            newPWForgotPWFinalInput.text = "";
                            mobileNoLoginInput.text = tempMobileReg;
                            tempMobileReg = "";
                            //if (iData1.Contains("data"))
                            //{
                            //    IDictionary iData2 = data["data"] as IDictionary;
                            //    if (iData2.Contains("message"))
                            //    {
                            //        StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, data["data"]["message"].ToString() + ", Please Login", "green", 2.5f));
                            //    }
                            //    else
                            //        StartCoroutine(GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, "Password changed successfully, Please Login", "green", 2.5f));
                            //}
                            //else
                                StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorLoginText, "Password changed successfully, Please Login", "green", 2.5f));
                        }
                        else if (iData1.Contains("errorMessage"))
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorForgotPWFinalText, data["errorMessage"].ToString(), "red", 2f));
                        }
                        else
                        {
                            StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorForgotPWFinalText, "Error in Forget Password Process!", "red", 2f));
                            Debug.Log("Error in Forget Password Process 2");
                        }
                    }
                    else
                    {
                        StartCoroutine(L_GlobalGameManager.instance.ShowPopUpTMP(errorForgotPWFinalText, "Error in Forget Password Process!!", "red", 2f));
                        Debug.Log("Error in Forget Password Process 3");
                    }
                }
            }));
        }
    }










    Dictionary<int, bool> otpFields = new Dictionary<int, bool>();

    public void OnOTPValueChanged(int otpNum)
    {
        Debug.Log("OnOTPValueChanged");

        int nullOrEmptyCount = 0;
        for (int i = 0; i < 6; i++)
        {
            if (string.IsNullOrEmpty(otpInputsArr[i].text))
                nullOrEmptyCount++;
        }
        if (nullOrEmptyCount == 0)
        {
            confirmOTPBtn.GetComponent<Button>().interactable = true;
            confirmOTPBtn.GetComponent<Image>().sprite = confirmOnSprite;
            confirmOTPBtn.GetChild(0).GetComponent<Text>().color = confirmOnTextColor;
        }
        else
        {
            confirmOTPBtn.GetComponent<Button>().interactable = false;
            confirmOTPBtn.GetComponent<Image>().sprite = confirmOffSprite;
            confirmOTPBtn.GetChild(0).GetComponent<Text>().color = confirmOffTextColor;
        }





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
        resendOnTextParent.gameObject.SetActive(true);
        otpSecondsText.gameObject.SetActive(false);
    }

    void Reset6OTPto1()
    {
        for (int i = 0; i < otpInputsArr.Length; i++)
        {
            otpInputsArr[i].text = "1";
        }
    }

    void ResetResend()
    {
        otpSecondsText.text = "Enter the OTP in 60 sec";
        resendOnTextParent.gameObject.SetActive(false);
        otpSecondsText.gameObject.SetActive(true);
    }

    void SetLableForSignUpOtp()
    {
        confirmOTPBtn.GetChild(0).GetComponent<Text>().text = "VERIFY";
        titleOTPText.text = "Welcome!!";
        title2OTP.gameObject.SetActive(true);
        //subTitleOTPText.text = "Please enter OTP sent to your \nRegistered E-mail.";
    }

    void SetLableForForgotPwOtp()
    {
        confirmOTPBtn.GetChild(0).GetComponent<Text>().text = "CONFIRM";
        titleOTPText.text = "Forget Password";
        title2OTP.gameObject.SetActive(false);
        //subTitleOTPText.text = "Please enter OTP sent to your \nRegistered Mobile Number.";
    }

    void ClearRegistrationInput()
    {
        mobileNoInput.text = "";
        emailInput.text = "";
        passwordInput.text = "";
        errorSignUpText.text = "";
    }

    void ClearLoginInput()
    {
        errorLoginText.text = "";
        mobileNoLoginInput.text = "";
        passwordLoginInput.text = "";
    }

    void ShowMobileNoForgotPWScreen()
    {
        mobileNoForgotPWInput.text = "";
        errorMobileNoForgotPWText.text = "";

        mobileNoFPWParent.SetActive(true);
        otpParent.SetActive(false);
        loginParent.SetActive(false);
        signUpParent.SetActive(false);
        mobileNoFPWFinalParent.SetActive(false);
    }

    void ShowMobileNoForgotPWFinalScreen()
    {
        newPWForgotPWFinalInput.text = "";

        mobileNoFPWFinalParent.SetActive(true);
        loginParent.SetActive(false);
        signUpParent.SetActive(false);
        otpParent.SetActive(false);
        mobileNoFPWParent.SetActive(false);
    }

    void ShowOtpScreen()
    {
        Reset6OTPto1();

        otpParent.SetActive(true);
        loginParent.SetActive(false);
        signUpParent.SetActive(false);
        mobileNoFPWParent.SetActive(false);
        mobileNoFPWFinalParent.SetActive(false);
        // clear login & signup input fields
    }

    void ShowSignUpScreen()
    {
        ClearRegistrationInput();
        HidePassword(passwordInput);

        signUpParent.SetActive(true);
        otpParent.SetActive(false);
        loginParent.SetActive(false);
        mobileNoFPWParent.SetActive(false);
        mobileNoFPWFinalParent.SetActive(false);
        // clear login & otp input fields
    }

    void ShowLoginScreen()
    {
        ClearLoginInput();
        HidePassword(passwordLoginInput);

        loginParent.SetActive(true);
        signUpParent.SetActive(false);
        otpParent.SetActive(false);
        mobileNoFPWParent.SetActive(false);
        mobileNoFPWFinalParent.SetActive(false);
        // clear signup & otp input fields
    }

    bool CheckPassword(string input, int minimum)
    {
        bool hasNum = false;
        bool hasCap = false;
        bool hasLow = false;
        bool hasSpec = false;
        char currentCharacter;

        if (!(input.Length >= minimum))
        {
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            currentCharacter = input[i];

            if (char.IsDigit(currentCharacter))
            {
                hasNum = true;
            }
            else if (char.IsUpper(currentCharacter))
            {
                hasCap = true;
            }
            else if (char.IsLower(currentCharacter))
            {
                hasLow = true;
            }
            else if (!char.IsLetterOrDigit(currentCharacter))
            {
                hasSpec = true;
            }

            if (hasNum && hasCap && hasLow && hasSpec)
            {
                return true;
            }
        }

        return false;
    }

    void HidePassword(InputField inputField)
    {
        inputField.contentType = InputField.ContentType.Password;
        inputField.transform.Find("ShowHidePasswordBtn/ShowHidePasswordImage").GetComponent<Image>().sprite = hidePasswordSprite;
        inputField.ForceLabelUpdate();
    }


    public void ShowPassword(InputField inputField)
    {
        if (inputField.contentType == InputField.ContentType.Password)
        {
            inputField.contentType = InputField.ContentType.Standard;
            inputField.transform.Find("ShowHidePasswordBtn/ShowHidePasswordImage").GetComponent<Image>().sprite = showPasswordSprite;
        }
        else
        {
            inputField.contentType = InputField.ContentType.Password;
            inputField.transform.Find("ShowHidePasswordBtn/ShowHidePasswordImage").GetComponent<Image>().sprite = hidePasswordSprite;
        }

        inputField.ForceLabelUpdate();
    }
}
