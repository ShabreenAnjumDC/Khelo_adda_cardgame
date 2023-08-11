using LitJson;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserProfile : MonoBehaviour
{
	[SerializeField] Text displayNameText, mobileNumberText, emailIdText, dobText, panNumText, accountNameText, accountNumText, bankNameText, ifscCodeText, branchAddText;
	[SerializeField] GameObject emailVerifiedTick, mobileVerifiedTick, editBtn;

	// Start is called before the first frame update
	void Start()
	{
		if (SceneManager.GetActiveScene().name == "MainScene")
			editBtn.SetActive(false);

		GetUserProfileDetails();
	}

	public void OnClickOnButton(string eventName)
	{
		//SoundManager.instance.PlaySound(SoundType.Click);

		switch (eventName)
		{
			case "back":
				MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.UserProfile);
				MainDashboardScreen.instance.bottomMenu.SetActive(true);
				break;
		}
	}

	private void GetUserProfileDetails()
	{
		StartCoroutine(WebServices.instance.GETRequestData(GameConstants.API_URL + "/user/profile", UserProfileDetailsResponse));
	}

	private void UserProfileDetailsResponse(string serverResponse, bool isErrorMessage, string errorMessage)
	{
		Debug.Log("Response => UserDetails: " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);

		if (data["statusCode"].ToString() == "200")
		{
			Debug.Log("Seebu user profile " + data);
			displayNameText.text = data["data"]["username"].ToString(); //username
			mobileNumberText.text = data["data"]["mobile"].ToString();
			emailIdText.text = data["data"]["email"].ToString();

			emailVerifiedTick.SetActive(!(bool)data["data"]["is_email_verified"]);
			mobileVerifiedTick.SetActive(!(bool)data["data"]["is_mobile_verified"]);

			if (data["data"]["dob"] != null)
			{
				dobText.text = data["data"]["dob"].ToString();
			}
			else
			{
				dobText.text = "";
			}

			if (data["data"]["pan_number"] != null)
			{
				panNumText.text = data["data"]["pan_number"].ToString();
			}
			else
			{
				panNumText.text = "";
			}

			if (data["data"]["bank_details"]["bank_name"] != null)
			{
				bankNameText.text = data["data"]["bank_details"]["bank_name"].ToString();
			}
			else
			{
				bankNameText.text = "";
			}

			if (data["data"]["bank_details"]["account_holder_name"] != null)
			{
				accountNameText.text = data["data"]["bank_details"]["account_holder_name"].ToString();
			}
			else
			{
				accountNameText.text = "";
			}
			
			if (data["data"]["bank_details"]["ifsc_code"] != null)
			{
				ifscCodeText.text = data["data"]["bank_details"]["ifsc_code"].ToString();
			}
			else
			{
				ifscCodeText.text = "";
			}

			if (data["data"]["bank_details"]["account_no"] != null)
			{
				accountNumText.text = data["data"]["bank_details"]["account_no"].ToString();
			}
			else
			{
				accountNumText.text = "";
			}

			if (data["data"]["bank_details"]["bank_address"] != null)
			{
				branchAddText.text = data["data"]["bank_details"]["bank_address"].ToString();
			}
			else
			{
				branchAddText.text = "";
			}
		}
		else
		{
			Debug.LogError(data["error"].ToString());
		}
	}

	public void CloseScreen()
	{
		if (SceneManager.GetActiveScene().name == "MainScene")
		{
			L_MainMenuController.instance.ShowScreen(MainMenuScreens.Dashboard);
		}
		else
		{
			MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.UserProfile);
			MainDashboardScreen.instance.bottomMenu.SetActive(true);
		}
	} 

	public void OpenEditProfileScreen()
	{
		MainDashboardScreen.instance.ShowScreen(MainDashboardScreen.MainDashboardScreens.EditProfile);
	}

}
