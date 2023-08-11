using System;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class EditProfile : MonoBehaviour
{
	[SerializeField] InputField nameInputField, emailIdInputField, mobileNumberInputField;
	[SerializeField] Text dobText;
	[SerializeField] DatePicker datePicker;

	private string selectedDOB;

	void Start()
	{
		GetUserProfileDetails();
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
			nameInputField.text = data["data"]["username"].ToString();
			mobileNumberInputField.text = data["data"]["mobile"].ToString();
			emailIdInputField.text = data["data"]["email"].ToString();




			if (data["data"]["dob"] != null)
			{
				dobText.text = data["data"]["dob"].ToString();
			}
			else
			{
				dobText.text = "";
			}
		}
		else
		{
			Debug.LogError(data["error"].ToString());
		}
	}

	public void CloseScreen()
	{
		MainDashboardScreen.instance.DestroyScreen(MainDashboardScreen.MainDashboardScreens.EditProfile);
		MainDashboardScreen.instance.bottomMenu.SetActive(true);
	}

	public void OnPressedConfirmButton()
	{
		string requestData = "{\"full_name\":\"" + "" + "\"," +
							   "\"user_name\":\"" + nameInputField.text + "\"," +
							   "\"dob\":\"" + selectedDOB + "\"," +
							   "\"gender\":\"" + "" + "\"," +
							   "\"profile_image\":\"" + dobText.text + "\"}";
		print(requestData);

		WebServices.instance.SendRequest(RequestType.EditProfile, requestData, true, EditProfileDetailsResponse);

	}

	private void EditProfileDetailsResponse(RequestType requestType, string serverResponse, bool isErrorMessagem, string errorMessage)
	{
		Debug.Log("Response => EditProfileDetails: " + serverResponse);
		JsonData data = JsonMapper.ToObject(serverResponse);
		MainDashboardScreen.instance.ShowMessage(data["message"].ToString());
		CloseScreen();
		MainDashboardScreen.instance.GetUserDetails();
	}

	public void SelectedDate()
	{
		dobText.text = datePicker.SelectedDate.Value.ToString("dd/MM/yyyy");
		selectedDOB = datePicker.SelectedDate.Value.ToString("yyyy/MM/dd");
	}
}
