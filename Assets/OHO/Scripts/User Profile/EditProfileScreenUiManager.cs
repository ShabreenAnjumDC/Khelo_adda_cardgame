using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EditProfileScreenUiManager : MonoBehaviour
{
    public static EditProfileScreenUiManager instance;
    public Image avtar, frame, flag;
    //public RawImage avatar;
    public string countrycode, countryname;
    public string avtarurl, flagurl, frameurl;
    public int avtarid;
    public InputField userNameText, mobileText, emailText, dobText, addressText, pinCodeText, stateText, panText, accNameText, accNumText, bankNameText, ifscText, branchAddText;
    string currentGenderValue;
    public Sprite genderSelected, genderUnselected;
    public GameObject[] genders;
    public GameObject mobileOTPScreen, emailOTPScreen, contentObj, mobileGetOTPBtn, emailGetOTPBtn;
    public Text kycStatusText;

    [SerializeField] DatePicker datePicker;

    float timer = 0f;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (PlayerManager.instance.IsLogedIn())
        {
            InitialiseProfileScreen();

     //       if (GlobalGameManager.instance.isKYCDone)
     //           kycStatusText.text = "<color=green>KYC Completed!</color>";
        }
    }

    private void FixedUpdate()
    {
        //if (timer > 1)
        //{
        //    timer -= Time.deltaTime;
        //    mobileGetOTPBtn.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0:00}:{1:00}", 0, timer);
        //}
        //else if (timer < 1 && mobileOTPScreen.activeSelf)
        //{
        //    mobileGetOTPBtn.transform.GetChild(0).GetComponent<Text>().text = "Get OTP";
        //    mobileGetOTPBtn.GetComponent<Button>().enabled = true;
        //}
    }

    public void InitialiseProfileScreen()
    {
        PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
        //coinsText.text = Utility.GetTrimmedAmount("" + playerData.coins);
        //diamondsText.text = Utility.GetTrimmedAmount("" + playerData.diamonds);
        //pointsText.text = Utility.GetTrimmedAmount("" + playerData.points);


        GetProfileURLs(playerData.userId);
    }

    public void GetProfileURLs(string playerid)
    {
        StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/profile", UserDetailsResponse));
    }

    void UserDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => UserDetails: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (bool.Parse(data["status"].ToString()))
        {
            PlayerGameDetails playerData = Utility.ParseUserDetails(data);
            JsonData d = data["data"];
            Debug.Log("json" + d);
            if (d["username"] != null)
            {
                userNameText.text = d["username"].ToString();
                Debug.Log("there is username");
            }
            else
            {
                userNameText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                userNameText.placeholder.GetComponent<Text>().text = "Name";
                Debug.Log("there is not any username");
            }

            if (d["mobile"] != null)
                mobileText.text = d["mobile"].ToString();
            else
            {
                mobileText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                mobileText.placeholder.GetComponent<Text>().text = "Phone Number";
            }

            if (d["email"] != null)
            {
                emailText.text = d["email"].ToString();
                //emailGetOTPBtn.SetActive(true);
            }
            else
            {
                emailText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                emailText.placeholder.GetComponent<Text>().text = "Email Address";
                //emailGetOTPBtn.SetActive(false);
            }

            if (!string.IsNullOrEmpty(d["dob"].ToString()))
            {
                DateTime dt;
                if (DateTime.TryParse(d["dob"].ToString(), out dt))
                {
                    dobText.text = dt.ToString("dd-MM-yyyy");
                }
                else
                    dobText.text = d["dob"].ToString();
            }
            else
            {
                dobText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                dobText.placeholder.GetComponent<Text>().text = "Date of Birth (DD-MM-YYYY)";
            }

            if (!string.IsNullOrEmpty(d["pan_number"].ToString()))
                panText.text = d["pan_number"].ToString();
            else
            {
                panText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                panText.placeholder.GetComponent<Text>().text = "PAN Number";
            }

            if (!string.IsNullOrEmpty(d["bank_details"]["account_holder_name"].ToString()))
                accNameText.text = d["bank_details"]["account_holder_name"].ToString();
            else
            {
                accNameText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                accNameText.placeholder.GetComponent<Text>().text = "Account Name";
            }

            if (!string.IsNullOrEmpty(d["bank_details"]["account_no"].ToString()))
                accNumText.text = d["bank_details"]["account_no"].ToString();
            else
            {
                accNumText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                accNumText.placeholder.GetComponent<Text>().text = "Account Number";
            }

            if (!string.IsNullOrEmpty(d["bank_details"]["bank_name"].ToString()))
                bankNameText.text = d["bank_details"]["bank_name"].ToString();
            else
            {
                bankNameText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                bankNameText.placeholder.GetComponent<Text>().text = "Bank Name";
            }

            if (!string.IsNullOrEmpty(d["bank_details"]["ifsc_code"].ToString()))
                ifscText.text = d["bank_details"]["ifsc_code"].ToString();
            else
            {
                ifscText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                ifscText.placeholder.GetComponent<Text>().text = "IFSC Code";
            }

            if (!string.IsNullOrEmpty(d["bank_details"]["bank_address"].ToString()))
                branchAddText.text = d["bank_details"]["bank_address"].ToString();
            else
            {
                branchAddText.transform.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
                branchAddText.placeholder.GetComponent<Text>().text = "Branch Address";
            }

            if(bool.Parse(data["data"]["is_mobile_verified"].ToString()))
            {
                mobileGetOTPBtn.transform.GetChild(0).gameObject.SetActive(false);
                mobileGetOTPBtn.transform.GetChild(1).gameObject.SetActive(true);
                mobileGetOTPBtn.GetComponent<Button>().enabled = false;
            }
            else if (!bool.Parse(data["data"]["is_mobile_verified"].ToString()))
            {
                mobileGetOTPBtn.transform.GetChild(0).gameObject.SetActive(true);
                mobileGetOTPBtn.transform.GetChild(1).gameObject.SetActive(false);
                mobileGetOTPBtn.GetComponent<Button>().enabled = true;
            }

            if (bool.Parse(data["data"]["is_email_verified"].ToString()))
            {
                emailGetOTPBtn.transform.GetChild(0).gameObject.SetActive(false);
                emailGetOTPBtn.transform.GetChild(1).gameObject.SetActive(true);
                emailGetOTPBtn.GetComponent<Button>().enabled = false;
            }
            else if (!bool.Parse(data["data"]["is_email_verified"].ToString()))
            {
                emailGetOTPBtn.transform.GetChild(0).gameObject.SetActive(true);
                emailGetOTPBtn.transform.GetChild(1).gameObject.SetActive(false);
                emailGetOTPBtn.GetComponent<Button>().enabled = true;
            }                
        }
        else
        {
            Debug.LogError(data["message"].ToString());
        }
    }

    public void InputFieldSelected(Transform field)
    {
        field.Find("Image").GetComponent<Image>().color = new Color(0.90f, 0.76f, 0.14f);
        field.parent.GetComponent<Text>().color = new Color(0.90f, 0.76f, 0.14f, 1f);
        //field.parent.GetComponent<Text>().color = new Color(0.90f, 0.76f, 0.14f);
        field.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "";
    }

    public void InputFieldDeselected(Transform field)
    {
        field.Find("Image").GetComponent<Image>().color = new Color(0.56f, 0.55f, 0.58f);
        field.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f);
        if (field.GetComponent<InputField>().text.Length <= 0)
        {
            field.GetComponent<InputField>().placeholder.GetComponent<Text>().text = field.parent.GetComponent<Text>().text;
            field.parent.GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f, 0f);
        }
    }

    public void GetMobileOTP()
    {
        if (!Utility.IsValidUserMobile(mobileText.text, out string error))
        {
            //MainMenuController.instance.ShowMessage(error);
        }
        //else
            //StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_BASE_URL + "user/changeMobile/sendOTP/" + mobileText.text, GotMobileOTP));
    }

    void GotMobileOTP(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => GotMobileOTP: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (bool.Parse(data["status"].ToString()))
        {
            timer = 30f;
            mobileGetOTPBtn.GetComponent<Button>().enabled = false;
            mobileOTPScreen.SetActive(true);
            contentObj.GetComponent<VerticalLayoutGroup>().enabled = false;
            StartCoroutine(EnableObj(contentObj.GetComponent<VerticalLayoutGroup>()));
        }
    }

    Dictionary<int, bool> mobileOTOFields = new Dictionary<int, bool>();
    public InputField[] mobilePassFields;
    public void VerifyMobileOTP(int otpNum)
    {
        if (!string.IsNullOrEmpty(mobilePassFields[otpNum].text))
        {
            mobileOTOFields.Add(otpNum, true);
            if (otpNum < 5)
                mobilePassFields[otpNum + 1].Select();
        }
        Debug.Log("ZZZZ " + mobileOTOFields.Count);
        //if (mobileOTOFields.Count == 6)
        //{
        //    OnClickOnButton("submit");
        //}
        if (string.IsNullOrEmpty(mobilePassFields[otpNum].text) && mobileOTOFields.ContainsKey(otpNum) && mobileOTOFields[otpNum] == true)
        {
            if (otpNum > 0)
                mobilePassFields[otpNum - 1].Select();
            mobileOTOFields.Remove(otpNum);
        }
    }

    public void GetEmailOTP()
    {
        StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/email-verification/" + emailText.text, GotEmailOTP));
    }

    void GotEmailOTP(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => GotEmailOTP: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (bool.Parse(data["status"].ToString()))
        {
            //emailOTPScreen.SetActive(true);
            //contentObj.GetComponent<VerticalLayoutGroup>().enabled = false;
            //contentObj.GetComponent<VerticalLayoutGroup>().enabled = true;
        }
        MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
    }

    Dictionary<int, bool> emailOTPList = new Dictionary<int, bool>();
    public InputField[] emailOTPFields;
    public string selectedDOB;

    public void VerifyEmailOTP(int otpNum)
    {
        if (!string.IsNullOrEmpty(emailOTPFields[otpNum].text))
        {
            emailOTPList.Add(otpNum, true);
            if (otpNum < 5)
                emailOTPFields[otpNum + 1].Select();
        }

        if (string.IsNullOrEmpty(emailOTPFields[otpNum].text) && emailOTPList.ContainsKey(otpNum) && emailOTPList[otpNum] == true)
        {
            if (otpNum > 0)
                emailOTPFields[otpNum - 1].Select();
            emailOTPList.Remove(otpNum);
        }
        Debug.Log("PPPP " + emailOTPList.Count);
        //if (emailOTPList.Count == 6)
        //{
        //    OnClickOnButton("submit");
        //}
    }

    public void VerifyButtonClick(string otpType)
    {
        if(otpType == "Moblie")
        {
            string otp = mobilePassFields[0].text + mobilePassFields[1].text + mobilePassFields[2].text + mobilePassFields[3].text + mobilePassFields[4].text + mobilePassFields[5].text;
            string requestData = "{\"mobile\":\"" + mobileText.text + "\"," +
                      "\"otp\":\"" + otp + "\"}";
            //StartCoroutine(WebServices.instance.POSTRequestData(GameConstants.API_BASE_URL + "user/changeMobile/verifyOtp", requestData, MobileVerifyResponse));
        }
        else
        {
            string otp = emailOTPFields[0].text + emailOTPFields[1].text + emailOTPFields[2].text + emailOTPFields[3].text + emailOTPFields[4].text + emailOTPFields[5].text;
            string requestData = "{\"email\":\"" + emailText.text + "\"," +
                      "\"otp\":\"" + otp + "\"}";
            Debug.Log("requestData " + requestData);
            //StartCoroutine(WebServices.instance.POSTRequestData(GameConstants.API_BASE_URL + "user/verifyOtpForgetPassword", requestData, MobileVerifyResponse));
        }
    }

    void MobileVerifyResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => MobileVerify: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (bool.Parse(data["status"].ToString()))
        {
            //MainMenuController.instance.ShowMessage(data["message"].ToString());
            if (data["message"].ToString() == "Email verified successfully")
            {
                emailOTPScreen.SetActive(false);
                emailGetOTPBtn.SetActive(false);
            }
            else
            {
                mobileOTPScreen.SetActive(false);
                mobileGetOTPBtn.SetActive(false);
            }
            contentObj.GetComponent<VerticalLayoutGroup>().enabled = false;
            StartCoroutine(EnableObj(contentObj.GetComponent<VerticalLayoutGroup>()));
        }
        else
        {
            Debug.LogError(data["error"].ToString());
            //MainMenuController.instance.ShowMessage(data["error"].ToString());
        }
    }

    void MobileChangedResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => MobileChnaged: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (bool.Parse(data["status"].ToString()))
        {
            
        }
        else
        {
            Debug.LogError(data["error"].ToString());
            //MainMenuController.instance.ShowMessage(data["error"].ToString());
            mobileOTPScreen.SetActive(false);
            contentObj.GetComponent<VerticalLayoutGroup>().enabled = false;
            StartCoroutine(EnableObj(contentObj.GetComponent<VerticalLayoutGroup>()));
        }
    }

    void EmailVerifyResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => EmailVerify: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (bool.Parse(data["status"].ToString()))
        {

        }
        else
        {
            Debug.LogError(data["error"].ToString());
            //MainMenuController.instance.ShowMessage(data["error"].ToString());
        }
    }

    IEnumerator EnableObj(VerticalLayoutGroup group)
    {
        yield return new WaitForSeconds(0.3f);
        group.enabled = true;
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                //  Debug.LogError("111111111111111111111111111111");
                //MainMenuController.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.GetUserDetails)
        {

            //GetUserDetails userdata = JsonUtility.FromJson<GetUserDetails>(serverResponse);


            //Debug.LogError("Status -> " + userdata.status);
            //Debug.LogError("Respnse -> " + userdata.response);
            //Debug.LogError("UserID -> " + userdata.getData.Length);


            //Debug.Log("Response => GetUserDetails :" + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                for (int i = 0; i < data["getData"].Count; i++)
                {
                    //Debug.Log("UserID -> " + userdata.getData[i].userId);

                    //loadImages(data["getData"][i]["profileImage"].ToString(), data["getData"][i]["frameURL"].ToString(), data["getData"][i]["countryFlag"].ToString());
                    //userLevel.text = "Lvl. " + data["getData"][i]["userLevel"].ToString() + ">>";
                    //userId.text = "UserID:" + data["getData"][i]["userId"].ToString();
                    userNameText.text = data["getData"][i]["userName"].ToString();
                    countrycode = data["getData"][i]["countryCode"].ToString();
                    countryname = data["getData"][i]["countryName"].ToString();
                    avtarurl = data["getData"][i]["profileImage"].ToString();
                    frameurl = data["getData"][i]["frameURL"].ToString();
                    flagurl = data["getData"][i]["countryFlag"].ToString();
                    PlayerManager.instance.GetPlayerGameData().coins = float.Parse(data["getData"][i]["coins"].ToString());
                    //if(null!= LobbyUiManager.instance)
                    //{
                    //     LobbyUiManager.instance.coinsText.text = Utility.GetTrimmedAmount("" + data["getData"][i]["coins"].ToString());
                    //}
                    //avtarid = int.Parse(data["getData"][i]["avatarID"].ToString()); //this is not coming in data
                    LoadImages(avtarurl, frameurl, flagurl);
                }
                //MainMenuController.instance.OnClickOnButton("profile");
            }
            else
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.EditProfile)
        {
            Debug.Log("Response => EditProfile :" + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
            if (bool.Parse(data["status"].ToString()))
            {
                Invoke("ShowUserProfileScreen", 1.5f);
            }
        }
    }

    void ShowUserProfileScreen()
    {
        MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.UserProfile);
    }

    public void LoadImages(string urlAvtar,string urlframe,string urlflag)
    {
        //   Debug.Log("Success data send");
        StartCoroutine(LoadSpriteImageFromUrl(urlflag, flag));
        StartCoroutine(LoadSpriteImageFromUrl(urlAvtar, avtar));
        StartCoroutine(LoadSpriteImageFromUrl(urlframe, frame));
    }
    IEnumerator LoadSpriteImageFromUrl(string URL, Image image)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            //Debug.LogError("Download failed");
        }
        else
        {
            var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
            Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);
            image.sprite = sprite;
            
            //Debug.Log("Successfully Set Player Profile");
        }


        //  WWW www = new WWW(URL);
        //  while (!www.isDone)
        //  {
        //      yield return null;
        //  }
        //  if (!string.IsNullOrEmpty(www.error))
        //  {
        //      Debug.Log("Download failed" + image.gameObject.name);
        //  }
        //  else
        //  {
        ////  Debug.Log("Success222222222 data send");
        //      Texture2D texture = new Texture2D(1, 1);
        //      www.LoadImageIntoTexture(texture);
        //      Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        //      image.sprite = sprite;
        //  }
    }

    void ProfileUpdatedResponse(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        Debug.Log("Response => UserDetails: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        if (bool.Parse(data["status"].ToString()))
        {
            //MainMenuController.instance.ShowScreen(MainMenuScreens.Profile);
            //MainMenuController.instance.ShowMessage("Profile successfully updated!");
        }
        else
        {
            //Debug.LogError(data["error"].ToString());
            MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
        }
    }

    public void GenderSelected(string genderValue)
    {
        currentGenderValue = genderValue;
        for (int i = 0; i < genders.Length; i++)
        {
            genders[i].GetComponent<Image>().sprite = genderUnselected;
            genders[i].transform.GetChild(1).GetComponent<Image>().color = new Color(0.56f, 0.55f, 0.58f);
            genders[i].transform.GetChild(0).GetComponent<Text>().color = new Color(0.56f, 0.55f, 0.58f);
        }
        genders[int.Parse(genderValue)].GetComponent<Image>().sprite = genderSelected;
        genders[int.Parse(genderValue)].transform.GetChild(1).GetComponent<Image>().color = new Color(0f, 0f, 0f);
        genders[int.Parse(genderValue)].transform.GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f);
    }

    public void OnClickOnButton(string eventName)
    {
        //SoundManager.instance.PlaySound(SoundType.Click);
        Debug.Log("Click " + eventName);
        switch (eventName)
        {
            case "back":
                //  MainDashboardScreen.instance.DestroyScreen(MainDashboardScreens.EditProfile);
                MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.EditProfile);
                MainDashboardScreen.instance.bottomMenu.SetActive(true);
                break;
            case "submit":
                string error = "";
                if (!Utility.IsValidUserEmail(emailText.text, out error))
                {
                    MainDashboardScreen.instance.ShowMessage(error);
                    break;
                }
                if (!IsValidDate(dobText.text, out error))
                {
                    MainDashboardScreen.instance.ShowMessage(error);
                    break;
                }

                string requestData = "{\"user_name\":\"" + userNameText.text + "\",";
                Debug.Log("Seebu Data Check"+requestData);
                if (!string.IsNullOrEmpty(emailText.text))
                    requestData += "\"email\":\"" + emailText.text + "\",";
                if (!string.IsNullOrEmpty(dobText.text))
                    requestData += "\"dob\":\"" + selectedDOB + "\","; //selectedDOB
                if (!string.IsNullOrEmpty(panText.text))
                    requestData += "\"pan_number\":\"" + panText.text + "\",";
                if (!string.IsNullOrEmpty(accNameText.text))
                    requestData += "\"account_holder_name\":\"" + accNameText.text + "\",";
                if (!string.IsNullOrEmpty(accNumText.text))
                    requestData += "\"account_no\":\"" + accNumText.text + "\",";
                if (!string.IsNullOrEmpty(bankNameText.text))
                    requestData += "\"bank_name\":\"" + bankNameText.text + "\",";
                if (!string.IsNullOrEmpty(ifscText.text))
                    requestData += "\"ifsc_code\":\"" + ifscText.text + "\",";
                if (!string.IsNullOrEmpty(branchAddText.text))
                    requestData += "\"bank_address\":\"" + branchAddText.text + "\",";

                //requestData += "\"city\":\"" + "" + "\",";

                /*if (!string.IsNullOrEmpty(dobText.text))
                {
                    string dobCal = dobText.text;
					string[] dtStr = dobCal.Split('-');
					string utcstr = dtStr[2] + "-" + dtStr[1] + "-" + dtStr[0];

                    DateTime dt;
                    if (DateTime.TryParse(utcstr, out dt))
                    {
                        requestData += "\"dob\":\"" + dt.ToString("yyyy-MM-dd") + "\",";

                    }
                    // requestData += "\"dob\":\"" + dobText.text + "\",";
                }*/



                if (requestData[requestData.Length - 1] == ',') //- 2
                {
                    requestData = requestData.Remove(requestData.Length - 1, 1); //- 2, requestData.Length - 1
                }
                requestData += "}";

                /*if (!string.IsNullOrEmpty(dobText.text))
                {
                    requestData = "{\"userName\":\"" + userNameText.text + "\"," +
                    "\"email\":\"" + emailText.text + "\"," +
                    "\"gender\":\"" + currentGenderValue + "\"," +
                      "\"address\":\"" + addressText.text + "\"," +
                      "\"pinCode\":\"" + pinCodeText.text + "\"," +
                      "\"city\":\"" + "" + "\"," +
                      "\"state\":\"" + stateText.text + "\"," +
                      "\"dob\":\"" + dobText.text + "\"}";
                }
                else
                {
                    requestData = "{\"userName\":\"" + userNameText.text + "\"," +
                    "\"email\":\"" + emailText.text + "\"," +
                    "\"gender\":\"" + currentGenderValue + "\"," +
                      "\"address\":\"" + addressText.text + "\"," +
                      "\"pinCode\":\"" + pinCodeText.text + "\"," +
                      "\"city\":\"" + "" + "\"," +
                      "\"state\":\"" + stateText.text + "\"}";
                }*/
                Debug.Log("requestData " + requestData);
                WebServices.instance.SendRequest(RequestType.EditProfile, requestData, true, OnServerResponseFound);
              //  WebServices.instance.SendRequest(RequestType.EditProfile, requestData, true, EditProfileDetailsResponse);
                break;

            case "backfromeditscreen":
                MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.UserProfile);
                break;

            case "openeditscreen":
                //MainMenuController.instance.ShowScreen(MainMenuScreens.EditProfile);
                break;

            case "hand":
                //MainMenuController.instance.ShowScreen(MainMenuScreens.HandScreen);
                break;

            case "coinShop":
                {
                    //MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
                }
                break;

            case "diamondShop":
                {
                    //MainMenuController.instance.ShowScreen(MainMenuScreens.Shop, new object[] { "diamond" });
                }
                break;

            case "vip":
                {
                    //TODO Show VIP cards Screen
                    //MainMenuController.instance.isVIPFromProfile = true;
                    //MainMenuController.instance.ShowScreen(MainMenuScreens.VIP_Privilege);
                }
                break;
            case "profilemodificataion":
                {
                    //MainMenuController.instance.ShowScreen(MainMenuScreens.ProfileModification);

                }
                break;
            case "Settings":
                {
                    //MainMenuController.instance.ShowScreen(MainMenuScreens.ProfileSetting);
                }
                break;
            case "AboutUS":
                {
                    //MainMenuController.instance.ShowScreen(MainMenuScreens.AboutUs);
                }
                break;
            case "Feedback":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;
            case "Tutorial":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;

            default:
#if ERROR_LOG
            Debug.LogError("unhdnled eventName found in menuHandller = " + eventName);
#endif
            break;
        }
    }
    private void EditProfileDetailsResponse(RequestType requestType, string serverResponse, bool isErrorMessagem, string errorMessage)
    {
        Debug.Log("Response => EditProfileDetails: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);
        MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
        CloseScreen();
        MainDashboardScreen.instance.GetUserDetails();
    }
    public void CloseScreen()
    {
        MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.EditProfile);
        MainDashboardScreen.instance.bottomMenu.SetActive(true);
    }
    bool IsValidDate(string date, out string error)
    {
        error = "";
        if(string.IsNullOrEmpty(date))
        {
            return true;
        }

        error = "Please enter valid date (DD-MM-YYYY)";
        DateTime dt;
        DateTime.TryParseExact(date, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out dt);
        if (dt == DateTime.MinValue)
        {
            return false;
        }
        else
        {   
            DateTime today = DateTime.Today;
            Debug.Log(dt + " - " + today.AddYears(-18));
            if (dt >= today.AddYears(-18))
            {
                error = "Less than 18 years age is not allowed";
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public void SelectedDate()
    {
        dobText.text = datePicker.SelectedDate.Value.ToString("dd-MM-yyyy");
        selectedDOB = datePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
    }
}